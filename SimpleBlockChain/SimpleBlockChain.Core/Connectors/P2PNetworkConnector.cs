using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Messages;
using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Core.Messages.DataMessages;
using SimpleBlockChain.Core.Storages;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBlockChain.Core.Connectors
{
    public class P2PNetworkConnector : IDisposable
    {
        private const int RETRY_P2P_CONNECTION_INTERVAL = 10000;
        private const int MIN_NB_PEERS = 3;
        private readonly PeersRepository _peersRepository;
        private Networks _network;
        private ConcurrentBag<PeerConnector> _peers;

        public P2PNetworkConnector()
        {
            _peers = new ConcurrentBag<PeerConnector>();
            _peersRepository = new PeersRepository();
            var instance = PeerEventStore.Instance();
            instance.NewPeerEvt += ListenPeer;
        }

        public event EventHandler ConnectEvent;
        public event EventHandler DisconnectEvent;

        public async Task Listen(Networks network, bool keepConnectionsAlive = true)
        {
            _network = network;
            await DiscoverNodes();
        }

        public void Broadcast(BaseTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var message = new TransactionMessage(transaction, _network);
            foreach (var activePeer in _peers.Where(p => p.GetServiceFlag() == ServiceFlags.NODE_NETWORK))
            {
                activePeer.Execute(message.Serialize());
            }
        }

        public void Broadcast(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            foreach (var activePeer in _peers)
            {
                activePeer.Execute(message.Serialize());
            }
        }

        public void Dispose()
        {
            foreach (var activePeer in _peers)
            {
                activePeer.Dispose();
            }
        }

        private async Task DiscoverNodes()
        {
            var peers = _peersRepository.GetAll();
            if (peers.Any())
            {
                await DiscoverNodesViaStore(peers);
                return;
            }

            await DiscoverNodesViaSeeds();
        }

        private Task DiscoverNodesViaStore(IEnumerable<IpAddress> nodes)
        {
            var result = nodes.Select(n => (new IPAddress(n.Ipv6)).MapToIPv4().ToString());
            return ConnectToPeers(result);
        }

        private async Task DiscoverNodesViaSeeds()
        {
            await ConnectToPeers(GetSeedNodes());
            var instance = PeersStore.Instance();
            var currentIpAddress = instance.GetMyIpAddress();
            var addrMessage = new AddrMessage(new CompactSize { Size = 1 }, _network);
            addrMessage.IpAddresses.Add(currentIpAddress);
            Broadcast(addrMessage);
            var getAddrMessage = new GetAddressMessage(_network);
            Broadcast(getAddrMessage);
        }        

        public bool ContainsPeer(IpAddress adr)
        {
            if (adr == null)
            {
                throw new ArgumentNullException(nameof(adr));
            }

            foreach (var activePeer in _peers)
            {
                if (activePeer.GetCurrentIpAddress().Equals(adr))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsPeer(byte[] ipAdr)
        {
            if (ipAdr == null)
            {
                throw new ArgumentNullException(nameof(ipAdr));
            }

            foreach (var activePeer in _peers)
            {
                if (activePeer.GetCurrentIpAddress().Ipv6.SequenceEqual(ipAdr))
                {
                    return true;
                }
            }

            return false;
        }

        private void ListenPeer(object sender, IpAddressEventArgs ipAddr)
        {
            if (ipAddr == null)
            {
                throw new ArgumentNullException(nameof(ipAddr));
            }

            if (ContainsPeer(ipAddr.Data))
            {
                return;
            }

            var ipAdr = new IPAddress(ipAddr.Data.Ipv6);
            ConnectToPeer(ipAdr.MapToIPv4().ToString());
        }
        
        private async Task ConnectToPeers(IEnumerable<string> hosts) // PASS THE SERVICE_FLAGS.
        {
            if (hosts == null)
            {
                throw new ArgumentNullException(nameof(hosts));
            }

            var tasks = new List<Task>();
            foreach (var seedNode in hosts)
            {
                var ip = IPAddress.Parse(seedNode);
                var myIp = PeersStore.Instance().GetMyIpAddress();
                if (myIp.Ipv6.SequenceEqual(ip.MapToIPv6().GetAddressBytes()))
                {
                    continue;
                }

                tasks.Add(ConnectToPeer(seedNode));
            }

            var result = Task.WhenAll(tasks.ToArray());
            try
            {
                await result;
            }
            catch (Exception) { }

            if (result.Exception != null)
            {
                var ex = result.Exception as AggregateException;
                if (ex.InnerExceptions.Count == hosts.Count())
                {
                    if (DisconnectEvent != null)
                    {
                        DisconnectEvent(this, EventArgs.Empty);
                    }

                    System.Threading.Thread.Sleep(RETRY_P2P_CONNECTION_INTERVAL);
                    await DiscoverNodes();
                    return;
                }
            }

            if (ConnectEvent != null)
            {
                ConnectEvent(this, EventArgs.Empty);
            }
        }

        private Task ConnectToPeer(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentNullException(nameof(host));
            }

            return Task.Factory.StartNew(() =>
            {
                var peerConnector = new PeerConnector(_network);
                try
                {
                    var manualResetEvent = new ManualResetEvent(false);
                    peerConnector.ConnectEvent += (s, i) =>
                    {
                        AddPeer(s, i);
                        manualResetEvent.Set();
                    };
                    peerConnector.TimeOutEvent += RemovePeer;
                    peerConnector.Connect(host, ServiceFlags.NODE_NETWORK);
                    manualResetEvent.WaitOne();
                }
                catch (PeerConnectorException)
                {
                    _peers = new ConcurrentBag<PeerConnector>(_peers.Except(new[] { peerConnector }));
                    throw;
                }
            });
        }

        private void AddPeer(object sender, IpAddressEventArgs ipAdr)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (ipAdr == null)
            {
                throw new ArgumentNullException(nameof(ipAdr));
            }

            _peers.Add(sender as PeerConnector);
            var seedNodes = GetSeedNodes().Select(s => IPAddress.Parse(s).MapToIPv6().GetAddressBytes());
            if (seedNodes.Any(s => s.SequenceEqual(ipAdr.Data.Ipv6)))
            {
                return;
            }

            if (ipAdr.Data.ServiceFlag != ServiceFlags.NODE_NETWORK)
            {
                return;
            }

            _peersRepository.AddPeer(ipAdr.Data).Wait();
        }

        private void RemovePeer(object sender, IpAddressEventArgs ipAdr)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (ipAdr == null)
            {
                throw new ArgumentNullException(nameof(ipAdr));
            }

            var peerConnector = sender as PeerConnector;
            _peers = new ConcurrentBag<PeerConnector>(_peers.Except(new[] { peerConnector }));
            _peersRepository.RemovePeer(ipAdr.Data).Wait();
            peerConnector.Dispose();
            if (!_peers.Where(p => p.GetServiceFlag() ==  ServiceFlags.NODE_NETWORK).Any())
            {
                _peersRepository.Empty();
                if (DisconnectEvent != null)
                {
                    DisconnectEvent(this, EventArgs.Empty);
                }

                DiscoverNodes();
            }
        }
        
        private static IEnumerable<string> GetSeedNodes()
        {
            return new []
            {
                Constants.DNS_IP_ADDRESS
            };
        }
    }
}

using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Exceptions;
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
            var seedNodes = GetSeedNodes();
            var tasks = new List<Task>();
            foreach(var seedNode in seedNodes)
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
                if (ex.InnerExceptions.Count == seedNodes.Count())
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
            ConnectToPeer(ipAdr.ToString());
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
                    peerConnector.Connect(host, ServiceFlags.NODE_NETWORK);
                    peerConnector.ConnectEvent += (s, i) =>
                    {
                        _peers.Add(peerConnector);
                        AddPeer(s, i);
                        manualResetEvent.Set();
                    };
                    peerConnector.TimeOutEvent += RemovePeer;
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

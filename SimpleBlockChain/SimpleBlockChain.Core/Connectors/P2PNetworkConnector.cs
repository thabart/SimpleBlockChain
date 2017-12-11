using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Messages;
using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Core.Messages.DataMessages;
using SimpleBlockChain.Core.Parsers;
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
        private MessageParser _messageParser;
        private MessageCoordinator _messageCoordinator;
        private bool _isSeedNode = false;

        public P2PNetworkConnector()
        {
            _peers = new ConcurrentBag<PeerConnector>();
            _messageParser = new MessageParser();
            _peersRepository = new PeersRepository();
            _messageCoordinator = new MessageCoordinator();
            var instance = PeerEventStore.Instance();
            instance.NewPeerEvt += ListenPeer;
            P2PConnectorEventStore.Instance().NewBlockEvt += BroadcastNewBlock;
            P2PConnectorEventStore.Instance().NewTransactionEvt += BroadcastNewTransaction;
        }

        public bool IsRunning { get; private set; }

        public event EventHandler ConnectEvent;
        public event EventHandler DisconnectEvent;

        public ConcurrentBag<PeerConnector> GetActivePeers()
        {
            return _peers;
        }

        public async Task Listen(Networks network, bool keepConnectionsAlive = true)
        {
            _network = network;
            var instance = PeersStore.Instance();
            var currentIpAddress = instance.GetMyIpAddress();
            _isSeedNode = IsSeedNode(currentIpAddress, _network);
            await DiscoverNodes();
        }

        public void Broadcast(BaseTransaction transaction, IEnumerable<byte> excludedIp = null)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var inventory = new Inventory(InventoryTypes.MSG_TX, transaction.GetTxId());
            var inventoryMessage = new InventoryMessage(new[] { inventory }, _network);
            foreach (var activePeer in _peers.Where(p => excludedIp != null && !p.GetCurrentIpAddress().Ipv6.SequenceEqual(excludedIp)))
            {
                activePeer.Execute(inventoryMessage.Serialize());
            }
        }

        public IEnumerable<Message> Broadcast(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var result = new List<Message>();
            foreach (var activePeer in _peers)
            {
                result.Add(_messageCoordinator.Launch(activePeer, message));
            }
            
            return result;
        }

        public void Dispose()
        {
            foreach (var activePeer in _peers)
            {
                activePeer.Dispose();
            }

            P2PConnectorEventStore.Instance().NewBlockEvt -= BroadcastNewBlock;
            P2PConnectorEventStore.Instance().NewTransactionEvt -= BroadcastNewTransaction;
        }

        public void Stop()
        {
            IsRunning = false;
            foreach (var activePeer in _peers)
            {
                activePeer.Dispose();
            }

            _peers = new ConcurrentBag<PeerConnector>();
            if (DisconnectEvent != null)
            {
                DisconnectEvent(this, EventArgs.Empty);
            }
        }

        private async Task DiscoverNodes()
        {
            IsRunning = false;
            var callback = new Action(() =>
            {
                if (ConnectEvent != null)
                {
                    IsRunning = true;
                    ConnectEvent(this, EventArgs.Empty);
                }
            });
            try
            {
                var peers = _peersRepository.GetAll();
                if (peers.Any()) // DISCOVER THE PEERS FROM THE REPOSITORY.
                {
                    await DiscoverNodesViaStore(peers);
                    callback();
                    return;
                }

                if (!_isSeedNode) // DISCOVER THE PEERS FROM THE SEEDS.
                {
                    await DiscoverNodesViaSeeds();
                }
            }
            catch (Exception) { if (_isSeedNode) { callback(); } return; }

            callback();
        }

        private Task DiscoverNodesViaStore(IEnumerable<IpAddress> nodes)
        {
            var result = nodes.Select(n => new KeyValuePair<string, ServiceFlags>((new IPAddress(n.Ipv6)).MapToIPv4().ToString(), n.ServiceFlag)).ToList();
            if (!_isSeedNode)
            {
                var dic = GetSeedNodes(_network).Select(s => new KeyValuePair<string, ServiceFlags>(s, ServiceFlags.NODE_NETWORK));
                result.AddRange(dic);
            }

            return ConnectToPeers(result);
        }

        private async Task DiscoverNodesViaSeeds()
        {
            var dic = GetSeedNodes(_network).Select(s => new KeyValuePair<string, ServiceFlags>(s, ServiceFlags.NODE_NETWORK));
            await ConnectToPeers(dic);
            var instance = PeersStore.Instance();
            var currentIpAddress = instance.GetMyIpAddress();
            var addrMessage = new AddrMessage(new CompactSize { Size = 1 }, _network);
            addrMessage.IpAddresses.Add(currentIpAddress);
            Broadcast(addrMessage);
            var getAddrMessage = new GetAddressMessage(_network);
            var getAddrLst = Broadcast(getAddrMessage);
            if (getAddrLst != null && getAddrLst.Any())
            {
                foreach(var getAddr in getAddrLst)
                {
                    var getAdrMsg = getAddr as AddrMessage;
                    if (getAdrMsg == null)
                    {
                        continue;
                    }

                    _peersRepository.AddPeers(getAdrMsg.IpAddresses);
                }
            }

            var peers = _peersRepository.GetAll();
            if (peers.Any())
            {
                await DiscoverNodesViaStore(peers);
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
            var peer = GetPeer(ipAdr);
            return peer != null;
        }

        public PeerConnector GetPeer(byte[] ipAdr)
        {
            if (ipAdr == null)
            {
                throw new ArgumentNullException(nameof(ipAdr));
            }

            foreach (var activePeer in _peers)
            {
                if (activePeer.GetCurrentIpAddress().Ipv6.SequenceEqual(ipAdr))
                {
                    return activePeer;
                }
            }

            return null;
        }

        private void ListenPeer(object sender, IpAddressEventArgs ipAddr)
        {
            if (ipAddr == null)
            {
                throw new ArgumentNullException(nameof(ipAddr));
            }

            if (ipAddr.Data == null)
            {
                throw new ArgumentNullException(nameof(ipAddr.Data));
            }

            if (ContainsPeer(ipAddr.Data))
            {
                return;
            }

            var ipAdr = new IPAddress(ipAddr.Data.Ipv6);
            var ip = ipAdr.MapToIPv4().ToString();
            var serviceFlag = ipAddr.Data.ServiceFlag;
            ConnectToPeer(ip, serviceFlag);
        }
        
        private async Task ConnectToPeers(IEnumerable<KeyValuePair<string, ServiceFlags>> hosts) // PASS THE SERVICE_FLAGS.
        {
            if (hosts == null)
            {
                throw new ArgumentNullException(nameof(hosts));
            }

            var tasks = new List<Task>();
            foreach (var kvp in hosts)
            {
                var ip = IPAddress.Parse(kvp.Key);
                var myIp = PeersStore.Instance().GetMyIpAddress();
                if (myIp.Ipv6.SequenceEqual(ip.MapToIPv6().GetAddressBytes())) // NOT SAME ADDRESS.
                {
                    continue;
                }

                if (ContainsPeer(ip.MapToIPv6().GetAddressBytes())) // CONNECTION DOESNT EXIST.
                {
                    continue;
                }

                tasks.Add(ConnectToPeer(kvp.Key, kvp.Value));
            }

            var result = Task.WhenAll(tasks.ToArray());
            await result;
        }

        private Task ConnectToPeer(string host, ServiceFlags serviceFlag)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentNullException(nameof(host));
            }

            return Task.Factory.StartNew(() =>
            {
                var peerConnector = new PeerConnector(_network, this);
                try
                {
                    var manualResetEvent = new ManualResetEvent(false);
                    peerConnector.ConnectEvent += (s, i) =>
                    {
                        AddPeer(s, i);
                        manualResetEvent.Set();
                    };
                    if (_isSeedNode) { peerConnector.TimeOutEvent += RemovePeer; }
                    else { peerConnector.TimeOutEvent += Disconnect; }                    
                    peerConnector.Connect(host, serviceFlag);
                    manualResetEvent.WaitOne();
                }
                catch (PeerConnectorException)
                {
                    if (_isSeedNode)
                    {
                        RemovePeer(peerConnector, peerConnector.GetCurrentIpAddress());
                    }
                    else
                    {
                        Disconnect();
                    }

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

            if (ipAdr.Data == null)
            {
                throw new ArgumentNullException(nameof(ipAdr.Data));
            }

            _peers.Add(sender as PeerConnector);
            if (IsSeedNode(ipAdr.Data, _network))
            {
                return;
            }

            _peersRepository.AddPeer(ipAdr.Data);
        }

        private void Disconnect(object sender, IpAddressEventArgs ipAdr)
        {
            Disconnect();
        }

        private void Disconnect()
        {
            _peersRepository.Empty();
            Stop();
            System.Threading.Thread.Sleep(RETRY_P2P_CONNECTION_INTERVAL);
            DiscoverNodes();
        }

        private void BroadcastNewBlock(object sender, BlockEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (e.Data == null)
            {
                throw new ArgumentNullException(nameof(e.Data));
            }

            var blockMessage = new BlockMessage(e.Data, _network);
            Broadcast(blockMessage);
        }

        private void BroadcastNewTransaction(object sender, TransactionEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (e.Data == null)
            {
                throw new ArgumentNullException(nameof(e.Data));
            }

            Broadcast(e.Data);
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

            RemovePeer(sender as PeerConnector, ipAdr.Data);
        }
        
        private void RemovePeer(PeerConnector peerConnector, IpAddress ipAdr)
        {
            if (peerConnector == null)
            {
                throw new ArgumentNullException(nameof(peerConnector));
            }

            if (ipAdr == null)
            {
                throw new ArgumentNullException(nameof(ipAdr));
            }
            
            _peers = new ConcurrentBag<PeerConnector>(_peers.Except(new[] { peerConnector }));
            _peersRepository.RemovePeer(ipAdr);
            peerConnector.Dispose();
        }

        private static bool IsSeedNode(IpAddress ipAddress, Networks network)
        {
            if (ipAddress == null)
            {
                throw new ArgumentNullException(nameof(ipAddress));
            }

            if (ipAddress.Ipv6 == null)
            {
                throw new ArgumentNullException(nameof(ipAddress.Ipv6));
            }

            var seedNodes = GetSeedNodes(network).Select(s => IPAddress.Parse(s).MapToIPv6().GetAddressBytes());
            return seedNodes.Any(sn => sn.SequenceEqual(ipAddress.Ipv6));
        }

        private static IEnumerable<string> GetSeedNodes(Networks network)
        {
            return GetSeedNodes()[network];
        }

        private static Dictionary<Networks, IEnumerable<string>> GetSeedNodes()
        {
            return new Dictionary<Networks, IEnumerable<string>>
            {
                { Networks.MainNet, new [] { Constants.DNS_IP_ADDRESS } },
                { Networks.TestNet, new string[0] }
            };
        }
    }
}

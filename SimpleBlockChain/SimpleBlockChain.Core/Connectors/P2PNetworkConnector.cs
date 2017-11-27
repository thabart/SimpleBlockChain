using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Messages.DataMessages;
using SimpleBlockChain.Core.Storages;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SimpleBlockChain.Core.Connectors
{
    public class P2PNetworkConnector : IDisposable
    {
        private const int RETRY_P2P_CONNECTION_INTERVAL = 10000;
        private readonly PeersRepository _peersRepository;
        private Networks _network;
        private IList<PeerConnector> _activePeers;

        public P2PNetworkConnector()
        {
            _activePeers = new List<PeerConnector>();
            _peersRepository = new PeersRepository();
            var instance = PeerEventStore.Instance();
            instance.NewPeerEvt += ListenPeer;
        }

        public event EventHandler ConnectEvent;
        public event EventHandler DisconnectEvent;

        public void Listen(Networks network, bool keepConnectionsAlive = true)
        {
            _network = network;
            DiscoverNodes();
        }

        public void Broadcast(BaseTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var message = new TransactionMessage(transaction, _network);
            foreach (var activePeer in _activePeers)
            {
                activePeer.Execute(message.Serialize());
            }
        }

        public void Dispose()
        {
            foreach (var activePeer in _activePeers)
            {
                activePeer.Dispose();
            }
        }

        private void DiscoverNodes()
        {
            var seedNodes = GetSeedNodes();
            int nbErrors = 0;
            foreach(var seedNode in seedNodes)
            {
                try
                {
                    ConnectToPeer(seedNode);
                }
                catch (PeerConnectorException)
                {
                    nbErrors++;
                }
            }

            if (nbErrors == seedNodes.Count())
            {
                if (DisconnectEvent != null)
                {
                    DisconnectEvent(this, EventArgs.Empty);
                }

                System.Threading.Thread.Sleep(RETRY_P2P_CONNECTION_INTERVAL);
                DiscoverNodes();
            }
            else
            {
                if (ConnectEvent != null)
                {
                    ConnectEvent(this, EventArgs.Empty);
                }   
            }
        }

        private void ListenPeer(object sender, IpAddressEventArgs ipAddr)
        {
            if (ipAddr == null)
            {
                throw new ArgumentNullException(nameof(ipAddr));
            }

            var ipAdr = new IPAddress(ipAddr.Data.Ipv6);
            ConnectToPeer(ipAdr.ToString());
        }

        private void ConnectToPeer(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentNullException(nameof(host));
            }

            var peerConnector = new PeerConnector(_network);
            peerConnector.Connect(host, ServiceFlags.NODE_NETWORK);
            peerConnector.ConnectEvent += AddPeer;
            peerConnector.TimeOutEvent += RemovePeer;
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

            var peerConnector = sender as PeerConnector;
            _activePeers.Add(peerConnector);
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
            _activePeers.Remove(peerConnector);
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

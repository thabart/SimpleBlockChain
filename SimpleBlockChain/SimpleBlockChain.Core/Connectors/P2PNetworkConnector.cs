using SimpleBlockChain.Core.Messages.DataMessages;
using SimpleBlockChain.Core.Messages.RpcMessages;
using SimpleBlockChain.Core.Storages;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Connectors
{
    public class P2PNetworkConnector : IDisposable
    {
        private Networks _network;
        private PeersStorage _peersStorage;
        private IList<PeerConnector> _peerConnectorLst;

        public P2PNetworkConnector()
        {
            _peersStorage = new PeersStorage();
            _peerConnectorLst = new List<PeerConnector>();
        }
        
        public void Listen(Networks network)
        {
            _network = network;
            DiscoverNodes();
        }

        public void Broadcast(Transaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var inventoryMessage = new SendRawTransactionMessage(transaction, _network);
            foreach(var peerConnector in _peerConnectorLst)
            {
                peerConnector.Execute(inventoryMessage.Serialize());
            }
        }

        private void DiscoverNodes()
        {
            var seedNodes = GetSeedNodes();
            foreach(var seedNode in seedNodes)
            {
                var peerConnector = new PeerConnector(_network);
                peerConnector.Connect(seedNode, ServiceFlags.NODE_NETWORK);
                _peerConnectorLst.Add(peerConnector);
            }
        }

        /// <summary>
        /// Get the SEED NODES.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<string> GetSeedNodes()
        {
            return new []
            {
                "192.168.1.6"
            };
        }

        public void Dispose()
        {
            foreach(var peerConnector in _peerConnectorLst)
            {
                peerConnector.Dispose();
            }
        }
    }
}

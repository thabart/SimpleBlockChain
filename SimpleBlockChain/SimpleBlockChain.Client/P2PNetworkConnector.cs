using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Storages;
using System;
using System.Collections.Generic;

namespace SimpleBlockChain.Client
{
    public class P2PNetworkConnector : IDisposable
    {
        private readonly Networks _network;
        private PeersStorage _peersStorage;
        private IList<PeerConnector> _peerConnectorLst;

        public P2PNetworkConnector(Networks network)
        {
            _peersStorage = new PeersStorage();
            _peerConnectorLst = new List<PeerConnector>();
            _network = network;
        }
        
        public void Listen()
        {
            DiscoverNodes();
        }

        private void DiscoverNodes()
        {
            var seedNodes = GetSeedNodes();
            var peerConnector = new PeerConnector(_network);
            foreach(var seedNode in seedNodes)
            {
                peerConnector.Connect(seedNode);
            }

            _peerConnectorLst.Add(peerConnector);
        }

        /// <summary>
        /// Get the SEED NODES.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<string> GetSeedNodes()
        {
            return new []
            {
                "127.0.0.1"
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

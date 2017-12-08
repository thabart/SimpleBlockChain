using SimpleBlockChain.Core;
using System;

namespace SimpleBlockChain.WalletUI.Events
{
    public class NetworkEventHandler : EventArgs
    {
        private Networks _network;

        public NetworkEventHandler(Networks network)
        {
            _network = network;
        }

        public Networks GetNework()
        {
            return _network;
        }
    }
}

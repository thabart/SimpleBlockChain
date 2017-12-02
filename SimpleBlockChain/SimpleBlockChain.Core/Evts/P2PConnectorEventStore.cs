using SimpleBlockChain.Core.Blocks;
using System;

namespace SimpleBlockChain.Core.Evts
{
    public class P2PConnectorEventStore
    {
        private static P2PConnectorEventStore _instance;

        public static P2PConnectorEventStore Instance()
        {
            if (_instance == null)
            {
                _instance = new P2PConnectorEventStore();
            }

            return _instance;
        }

        public void Broadcast(Block block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            if (NewBlockEvt != null)
            {
                NewBlockEvt(this, new BlockEventArgs(block));
            }
        }

        public event EventHandler<BlockEventArgs> NewBlockEvt;
    }
}

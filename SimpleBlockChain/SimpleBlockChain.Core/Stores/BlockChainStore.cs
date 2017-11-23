using SimpleBlockChain.Core.Blocks;

namespace SimpleBlockChain.Core.Stores
{
    public class BlockChainStore
    {
        private BlockChain _blockChain;
        private static BlockChainStore _instance = null;

        private BlockChainStore()
        {
            _blockChain = new BlockChain();
        }

        public static BlockChainStore Instance()
        {
            if (_instance == null)
            {
                _instance = new BlockChainStore();
            }

            return _instance;
        }

        public BlockChain GetBlockChain()
        {
            return _blockChain;
        }
    }
}

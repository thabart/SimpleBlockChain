using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Factories;

namespace SimpleBlockChain.Core.Stores
{
    public interface IBlockChainStore
    {
        BlockChain GetBlockChain();
        void Switch(Networks network);
    }

    internal class BlockChainStore : IBlockChainStore
    {
        private readonly IBlockChainFactory _blockChainFactory;
        private BlockChain _blockChain;

        public BlockChainStore(IBlockChainFactory blockChainFactory)
        {
            _blockChainFactory = blockChainFactory;
        }

        public BlockChain GetBlockChain()
        {
            return _blockChain;
        }

        public void Switch(Networks network)
        {
            if (_blockChain != null)
            {
                _blockChain.Dispose();
                _blockChain = null;
            }

            _blockChain = _blockChainFactory.Build(network);
        }
    }
}

using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Helpers;

namespace SimpleBlockChain.Core.Factories
{
    public interface IBlockChainFactory
    {
        BlockChain Build(Networks network);
    }

    internal class BlockChainFactory : IBlockChainFactory
    {
        private readonly IAssemblyHelper _assemblyHelper;

        public BlockChainFactory(IAssemblyHelper assemblyHelper)
        {
            _assemblyHelper = assemblyHelper;
        }

        public BlockChain Build(Networks network)
        {
            return new BlockChain(_assemblyHelper, network);
        }
    }
}

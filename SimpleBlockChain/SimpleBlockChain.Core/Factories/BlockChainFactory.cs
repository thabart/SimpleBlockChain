using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Compiler;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Stores;

namespace SimpleBlockChain.Core.Factories
{
    public interface IBlockChainFactory
    {
        BlockChain Build(Networks network);
    }

    internal class BlockChainFactory : IBlockChainFactory
    {
        private readonly IAssemblyHelper _assemblyHelper;
        private readonly ISmartContractStore _smartContractStore;

        public BlockChainFactory(IAssemblyHelper assemblyHelper, ISmartContractStore smartContractStore)
        {
            _assemblyHelper = assemblyHelper;
            _smartContractStore = smartContractStore;
        }

        public BlockChain Build(Networks network)
        {
            return new BlockChain(_assemblyHelper, network, _smartContractStore);
        }
    }
}

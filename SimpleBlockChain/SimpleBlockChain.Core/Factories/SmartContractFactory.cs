using SimpleBlockChain.Core.Compiler;
using SimpleBlockChain.Core.Helpers;

namespace SimpleBlockChain.Core.Factories
{
    public interface ISmartContractFactory
    {
        SmartContracts Build(Networks network);
    }

    internal class SmartContractFactory : ISmartContractFactory
    {
        private readonly IAssemblyHelper _assemblyHelper;

        public SmartContractFactory(IAssemblyHelper assemblyHelper)
        {
            _assemblyHelper = assemblyHelper;
        }

        public SmartContracts Build(Networks network)
        {
            return new SmartContracts(_assemblyHelper, network);
        }
    }
}

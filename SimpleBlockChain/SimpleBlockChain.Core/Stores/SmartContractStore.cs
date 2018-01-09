using SimpleBlockChain.Core.Compiler;
using SimpleBlockChain.Core.Factories;

namespace SimpleBlockChain.Core.Stores
{
    public interface ISmartContractStore
    {
        SmartContracts GetSmartContracts();
        void Switch(Networks network);
    }

    internal class SmartContractStore : ISmartContractStore
    {
        private readonly ISmartContractFactory _smartContractFactory;
        private SmartContracts _smartContract;

        public SmartContractStore(ISmartContractFactory smartContractFactory)
        {
            _smartContractFactory = smartContractFactory;
        }

        public SmartContracts GetSmartContracts()
        {
            return _smartContract;
        }

        public void Switch(Networks network)
        {
            if (_smartContract != null)
            {
                _smartContract.Dispose();
                _smartContract = null;
            }

            _smartContract = _smartContractFactory.Build(network);
        }
    }
}

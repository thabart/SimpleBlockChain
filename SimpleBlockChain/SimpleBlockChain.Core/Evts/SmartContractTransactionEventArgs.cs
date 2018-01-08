using SimpleBlockChain.Core.Transactions;

namespace SimpleBlockChain.Core.Evts
{
    internal class SmartContractTransactionEventArgs
    {
        private readonly SmartContractTransaction _transaction;

        public SmartContractTransactionEventArgs(SmartContractTransaction transaction)
        {
            _transaction = transaction;
        }

        public SmartContractTransaction Data
        {
            get
            {
                return _transaction;
            }
        }
    }
}

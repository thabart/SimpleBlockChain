using SimpleBlockChain.Core.Transactions;

namespace SimpleBlockChain.Core.Evts
{
    internal class TransactionEventArgs
    {
        private readonly BaseTransaction _transaction;

        public TransactionEventArgs(BaseTransaction transaction)
        {
            _transaction = transaction;
        }

        public BaseTransaction Data
        {
            get
            {
                return _transaction;
            }
        }
    }
}

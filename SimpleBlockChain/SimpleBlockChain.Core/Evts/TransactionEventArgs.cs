﻿using SimpleBlockChain.Core.Transactions;

namespace SimpleBlockChain.Core.Evts
{
    internal class TransactionEventArgs
    {
        private readonly BcBaseTransaction _transaction;

        public TransactionEventArgs(BcBaseTransaction transaction)
        {
            _transaction = transaction;
        }

        public BcBaseTransaction Data
        {
            get
            {
                return _transaction;
            }
        }
    }
}

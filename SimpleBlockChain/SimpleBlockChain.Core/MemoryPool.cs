using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;

namespace SimpleBlockChain.Core
{
    public class MemoryPool
    {
        private static MemoryPool _instance;
        private IList<Transaction> _transactions;

        private MemoryPool()
        {
            _transactions = new List<Transaction>();
        }

        public static MemoryPool Instance()
        {
            if (_instance == null)
            {
                _instance = new MemoryPool();
            }

            return _instance;
        }

        public void AddTransaction(Transaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            _transactions.Add(transaction);
        }

        public IList<Transaction> GetTransactions()
        {
            return _transactions;
        }
    }
}

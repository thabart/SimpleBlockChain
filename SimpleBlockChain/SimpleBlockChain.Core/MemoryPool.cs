using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core
{
    public class MemoryPool
    {
        private static MemoryPool _instance;
        private IList<BaseTransaction> _transactions;

        private MemoryPool()
        {
            _transactions = new List<BaseTransaction>();
        }

        public static MemoryPool Instance()
        {
            if (_instance == null)
            {
                _instance = new MemoryPool();
            }

            return _instance;
        }

        public void AddTransaction(BaseTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            _transactions.Add(transaction);
        }

        public IList<BaseTransaction> GetTransactions()
        {
            return _transactions;
        }

        public void Remove(BaseTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            Remove(new[] { transaction.GetTxId() });
        }

        public void Remove(IEnumerable<IEnumerable<byte>> txIds)
        {
            if (txIds == null)
            {
                throw new ArgumentNullException(nameof(txIds));
            }

            var removedTxs = _transactions.Where(t => txIds.Any(s => s.SequenceEqual(t.GetTxId()))).ToList();
            foreach(var removedTx in removedTxs)
            {
                _transactions.Remove(removedTx);
            }
        }
    }
}

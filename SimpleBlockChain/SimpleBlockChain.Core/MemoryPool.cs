using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core
{
    public class MemoryPoolRecord
    {
        public MemoryPoolRecord(BaseTransaction transaction, DateTime insertTime, int blockHeight)
        {
            Transaction = transaction;
            InsertTime = insertTime;
            BlockHeight = blockHeight;
        }

        public BaseTransaction Transaction { get; set; }
        public DateTime InsertTime { get; set; }
        public int Height { get; set; }
        public int BlockHeight { get; set; }
    }

    public class MemoryPool
    {
        private static MemoryPool _instance;
        private IList<MemoryPoolRecord> _transactions;

        private MemoryPool()
        {
            _transactions = new List<MemoryPoolRecord>();
        }

        public static MemoryPool Instance()
        {
            if (_instance == null)
            {
                _instance = new MemoryPool();
            }

            return _instance;
        }

        public void AddTransaction(BaseTransaction transaction, int blockHeight)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            _transactions.Add(new MemoryPoolRecord(transaction, DateTime.UtcNow, blockHeight));
        }

        public IList<MemoryPoolRecord> GetTransactions()
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

            var removedTxs = _transactions.Where(t => txIds.Any(s => s.SequenceEqual(t.Transaction.GetTxId()))).ToList();
            foreach(var removedTx in removedTxs)
            {
                _transactions.Remove(removedTx);
            }
        }
    }
}

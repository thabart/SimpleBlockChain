using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Extensions;
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
        public MemoryPoolRecord ParentMemoryPool { get; set; }

        public void GetDepends(List<string> result)
        {
            if (ParentMemoryPool == null || ParentMemoryPool.Transaction == null)
            {
                return;
            }

            result.Add(ParentMemoryPool.Transaction.GetTxId().ToHexString());
            ParentMemoryPool.GetDepends(result);
        }
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

            var record = new MemoryPoolRecord(transaction, DateTime.UtcNow, blockHeight);
            var noneCoinBaseTx = transaction as NoneCoinbaseTransaction;
            if (noneCoinBaseTx != null)
            {
                var txIn = noneCoinBaseTx.TransactionIn.First() as TransactionInNoneCoinbase;
                var parent = GetUnspentMemoryRecord(txIn.Outpoint.Hash, txIn.Outpoint.Index);
                if (parent != null)
                {
                    record.ParentMemoryPool = parent;
                }
            }

            _transactions.Add(record);
        }

        public IList<MemoryPoolRecord> GetTransactions()
        {
            return _transactions;
        }

        public int CountDescendants(MemoryPoolRecord memPoolRecord)
        {
            if (memPoolRecord == null)
            {
                throw new ArgumentNullException(nameof(memPoolRecord));
            }

            if (memPoolRecord.Transaction == null)
            {
                throw new ArgumentNullException(nameof(memPoolRecord.Transaction));
            }

            var descendants = new List<MemoryPoolRecord>();
            GetDescendants(memPoolRecord, descendants);
            if (descendants == null)
            {
                return 1;
            }

            return descendants.Count() + 1;
        }

        public void GetDescendants(MemoryPoolRecord memPoolRecord, List<MemoryPoolRecord> records)
        {
            if (memPoolRecord == null)
            {
                throw new ArgumentNullException(nameof(memPoolRecord));
            }

            if (memPoolRecord.Transaction == null)
            {
                throw new ArgumentNullException(nameof(memPoolRecord.Transaction));
            }

            var transaction = memPoolRecord.Transaction;
            if (transaction.TransactionOut != null && transaction.TransactionOut.Any())
            {
                var txs = _transactions.Where(t =>
                {
                    var noneCbTx = t.Transaction as NoneCoinbaseTransaction;
                    if (noneCbTx == null)
                    {
                        return false;
                    }

                    var txIn = noneCbTx.TransactionIn.First() as TransactionInNoneCoinbase;
                    return txIn.Outpoint.Hash.SequenceEqual(transaction.GetTxId());

                });
                if (txs == null || !txs.Any())
                {
                    return;
                }

                records.AddRange(txs);
                foreach(var m in txs)
                {
                    GetDescendants(m, records);
                }
            }
        }

        public int CountAncestors(MemoryPoolRecord memPoolRecord)
        {
            if (memPoolRecord == null)
            {
                throw new ArgumentNullException(nameof(memPoolRecord));
            }
            
            var ancestors = new List<MemoryPoolRecord>();
            GetAncestors(memPoolRecord, ancestors);
            if (ancestors == null)
            {
                return 1;
            }

            return ancestors.Count() + 1;
        }

        public void GetAncestors(MemoryPoolRecord memPoolRecord, List<MemoryPoolRecord> records)
        {
            if (memPoolRecord == null)
            {
                throw new ArgumentNullException(nameof(memPoolRecord));
            }
            
            if (memPoolRecord.ParentMemoryPool != null)
            {
                records.Add(memPoolRecord.ParentMemoryPool);
                GetAncestors(memPoolRecord.ParentMemoryPool, records);
            }
        }

        public bool ContainsTransactions(IEnumerable<byte> txId, uint index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }

            return _transactions.Any(t => {
                var noneCoinBaseTransaction = t.Transaction as NoneCoinbaseTransaction;
                if (noneCoinBaseTransaction == null)
                {
                    return false;
                }

                var txIn = noneCoinBaseTransaction.TransactionIn.First() as TransactionInNoneCoinbase;
                return txIn.Outpoint.Hash.SequenceEqual(txId) && txIn.Outpoint.Index == index;
            });
        }

        public MemoryPoolRecord GetUnspentMemoryRecord(IEnumerable<byte> txId)
        {
            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }

            var referencedTx = _transactions.FirstOrDefault(t => t.Transaction.GetTxId().SequenceEqual(txId));
            if (referencedTx == null)
            {
                return null;
            }

            return referencedTx;
        }

        public MemoryPoolRecord GetUnspentMemoryRecord(IEnumerable<byte> txId, uint index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var referencedTx = GetUnspentMemoryRecord(txId);
            if (referencedTx == null || index >= referencedTx.Transaction.TransactionOut.Count())
            {
                return null;
            }

            return referencedTx;
        }

        public TransactionOut GetUnspentTransaction(IEnumerable<byte> txId, uint index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }

            var memoryPoolRecord = GetUnspentMemoryRecord(txId, index);
            if (memoryPoolRecord == null)
            {
                return null;
            }

            return memoryPoolRecord.Transaction.TransactionOut.ElementAt((int)index);
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

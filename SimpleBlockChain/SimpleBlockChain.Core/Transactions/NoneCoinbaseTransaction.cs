using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Transactions
{
    public class NoneCoinbaseTransaction : BaseTransaction
    {
        public NoneCoinbaseTransaction() { }

        public NoneCoinbaseTransaction(uint version, uint lockTime) : base(version, lockTime) { }

        public override KeyValuePair<List<BaseTransactionIn>, int> DeserializeInputs(IEnumerable<byte> payload, int size)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var result = new List<BaseTransactionIn>();
            var currentStartIndex = 0;
            for(var i = 0; i < size; i++)
            {
                var kvp = TransactionInNoneCoinbase.Deserialize(payload.Skip(currentStartIndex));
                currentStartIndex += kvp.Value;
                result.Add(kvp.Key);
            }

            return new KeyValuePair<List<BaseTransactionIn>, int>(result, currentStartIndex);
        }

        public override KeyValuePair<BaseTransactionOut, int> DeserializeOutput(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            return Transactions.TransactionOut.Deserialize(payload);
        }

        public IEnumerable<BaseTransactionOut> GetReferencedTransactionOut(NoneCoinbaseTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var trIns = transaction.TransactionIn.Select(ti => ti as TransactionInNoneCoinbase);
            var result = new List<BaseTransactionOut>();
            foreach (var trIn in trIns)
            {
                if (!GetTxId().SequenceEqual(trIn.Outpoint.Hash))
                {
                    continue;
                }

                if (trIn.Outpoint.Index >= TransactionOut.Count())
                {
                    continue;
                }

                result.Add(TransactionOut.ElementAt((int)trIn.Outpoint.Index));
            }

            return result;
        }

        public override int CompareTo(BaseTransaction obj)
        {
            var noneCoinBaseTransaction = obj as NoneCoinbaseTransaction;
            if (noneCoinBaseTransaction == null)
            {
                return 1;
            }

            var r = noneCoinBaseTransaction.GetReferencedTransactionOut(this);
            if (r.Any())
            {
                return 1;
            }

            r = GetReferencedTransactionOut(noneCoinBaseTransaction);
            if (r.Any())
            {
                return -1;
            }

            return 0;
        }
    }
}

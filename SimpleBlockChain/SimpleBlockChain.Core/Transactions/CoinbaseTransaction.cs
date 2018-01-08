using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Transactions
{
    public class CoinbaseTransaction : BcBaseTransaction
    {
        public CoinbaseTransaction() { }

        public CoinbaseTransaction(uint version, uint lockTime) : base(version, lockTime) { }

        public override KeyValuePair<List<BaseTransactionIn>, int> DeserializeInputs(IEnumerable<byte> payload, int size)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var result = new List<BaseTransactionIn>();
            var currentStartIndex = 0;
            for (var i = 0; i < size; i++)
            {
                var kvp = TransactionInCoinbase.Deserialize(payload.Skip(currentStartIndex));
                currentStartIndex += kvp.Value;
                result.Add(kvp.Key);
            }

            return new KeyValuePair<List<BaseTransactionIn>, int>(result, currentStartIndex);
        }

        public override KeyValuePair<TransactionOut, int> DeserializeOutput(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            return Transactions.TransactionOut.Deserialize(payload);
        }

        public void SetTransactionIn(TransactionInCoinbase transactionIn)
        {
            if (transactionIn == null)
            {
                throw new ArgumentNullException(nameof(transactionIn));
            }

            TransactionIn = new List<BaseTransactionIn> { transactionIn };
        }

        public override int CompareTo(BcBaseTransaction obj)
        {
            return -1;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Transactions
{
    public class CoinbaseTransaction : BaseTransaction
    {
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

        public void SetTransactionIn(TransactionInCoinbase transactionIn)
        {
            if (transactionIn == null)
            {
                throw new ArgumentNullException(nameof(transactionIn));
            }

            TransactionIn = new List<BaseTransactionIn> { transactionIn };
        }
    }
}

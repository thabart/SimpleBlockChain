using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Transactions
{
    public class NoneCoinbaseTransaction : BaseTransaction
    {
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
    }
}

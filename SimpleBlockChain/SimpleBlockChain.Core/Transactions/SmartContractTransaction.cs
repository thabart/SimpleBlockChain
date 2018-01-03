using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Transactions
{
    public class SmartContractTransaction : BaseTransaction
    {
        public Script Script { get; set; }
        public string Author { get; set; }
        public string Name { get; set; }
        public IList<string> Parameters { get; set; }
        public long Gas { get; set; }

        public override int CompareTo(BaseTransaction obj)
        {
            return 0;
        }

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

        public override KeyValuePair<BaseTransactionOut, int> DeserializeOutput(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            return TransactionOutSmartContract.Deserialize(payload);
        }
    }
}

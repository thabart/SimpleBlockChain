using SimpleBlockChain.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Transactions
{
    public class TransactionOut : BaseTransactionOut
    {
        public long Value { get; private set; }

        public TransactionOut(long value, Script script) : base(script)
        {
            Value = value;
        }

        public override IEnumerable<byte> Serialize()
        {
            var result = new List<byte>();
            var scriptPayload = Script.Serialize();
            var compactSize = new CompactSize();
            compactSize.Size = (ulong)scriptPayload.Count();
            result.AddRange(BitConverter.GetBytes(Value));
            result.AddRange(compactSize.Serialize());
            result.AddRange(scriptPayload);
            return result;
        }

        public static KeyValuePair<BaseTransactionOut, int> Deserialize(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            int startIndex = 0;
            var value = BitConverter.ToInt64(payload.Take(8).ToArray(), 0);
            startIndex = 8;
            var compactSize = CompactSize.Deserialize(payload.Skip(startIndex).ToArray());
            startIndex += compactSize.Value;
            var script = Script.Deserialize(payload.Skip(startIndex).Take((int)compactSize.Key.Size));
            startIndex += (int)compactSize.Key.Size;
            return new KeyValuePair<BaseTransactionOut, int>(new TransactionOut(value, script), startIndex);
        }
    }
}

using SimpleBlockChain.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Transactions
{
    public class TransactionOut
    {
        public long Value { get; private set; }
        public Script Script { get; private set; }

        public TransactionOut(long value, Script script)
        {
            Value = value;
            Script = script;
        }

        public IEnumerable<byte> Serialize()
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
    }
}

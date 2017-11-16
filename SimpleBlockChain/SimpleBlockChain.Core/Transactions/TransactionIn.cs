using SimpleBlockChain.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Transactions
{
    public class TransactionIn
    {
        public Outpoint Outpoint { get; set; } // outpoint
        public IEnumerable<byte> SignatureScript { get; set; } // signature script
        private const UInt32 _sequence = 0xffffffff;

        public IEnumerable<byte> Serialize()
        {
            var result = new List<byte>();
            result.AddRange(Outpoint.Serialize());
            var compactSize = new CompactSize();
            compactSize.Size = 0;
            if (SignatureScript != null)
            {
                compactSize.Size = (ulong)SignatureScript.Count();
                result.AddRange(compactSize.Serialize());
                result.AddRange(SignatureScript);
            } else
            {
                result.AddRange(compactSize.Serialize());
            }

            result.AddRange(BitConverter.GetBytes(_sequence));
            return result;
        }
    }
}

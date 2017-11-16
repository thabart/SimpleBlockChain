using SimpleBlockChain.Core.Common;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Transactions
{
    public class TransactionIn
    {
        public Outpoint Outpoint { get; set; } // outpoint
        public CompactSize ScriptBytes { get; set; } // script bytes
        public char[] SignatureScript { get; set; } // signature script
        public uint Sequence { get; set; } // sequence

        public IEnumerable<byte> Serialize()
        {
            return null;
        }
    }
}

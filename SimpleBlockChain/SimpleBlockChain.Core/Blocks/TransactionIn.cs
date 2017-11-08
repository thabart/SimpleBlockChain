using SimpleBlockChain.Core.Common;

namespace SimpleBlockChain.Core.Blocks
{
    public class TransactionIn
    {
        public Outpoint Outpoint { get; set; } // outpoint
        public CompactSize ScriptBytes { get; set; } // script bytes
        public char[] SignatureScript { get; set; } // signature script
        public uint Sequence { get; set; } // sequence
    }
}

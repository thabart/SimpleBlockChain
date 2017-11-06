using System;

namespace SimpleBlockChain.Core.Blocks
{
    public class TransactionOut
    {
        public Int64 Value { get; set; } // value
        public CompactSize PkScriptBytes { get; set; } // pk_script bytes
        public char[] PkScript { get; set; } // pk_script
    }
}

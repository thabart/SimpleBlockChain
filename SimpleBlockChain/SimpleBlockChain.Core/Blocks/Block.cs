using SimpleBlockChain.Core.Common;

namespace SimpleBlockChain.Core.Blocks
{
    public class Block
    {
        public BlockHeader Header { get; set; }
        public CompactSize TxnCount { get; set; }
        public RawTransaction RawTransaction { get; set; }
    }
}

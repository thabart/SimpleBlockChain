using SimpleBlockChain.Core.Transactions;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Blocks
{
    public class UTXO
    {
        public IEnumerable<byte> TxId { get; set; }
        public int Index { get; set; }
        public long Value { get; set; }
        public Script Script { get; set; }
    }
}

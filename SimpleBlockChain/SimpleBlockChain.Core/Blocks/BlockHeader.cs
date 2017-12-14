using System.Collections.Generic;

namespace SimpleBlockChain.Core.Blocks
{
    public class BlockHeader
    {
        public const int CURRENT_VERSION = 4;
        public const int SIZE = 80;

        public BlockHeader()
        {
            Version = CURRENT_VERSION;
        }

        public int Version { get; set; }
        public IEnumerable<byte> PreviousBlockHeader { get; set; }
        public IEnumerable<byte> MerkleRoot { get; set; }
        public uint Time { get; set; }
        public uint NBits { get; set; }
        public uint Nonce { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Blocks
{
    public class BlockHeader
    {
        private static int CURRENT_VERSION = 4;
        public const int SIZE = 80;

        public uint Version { get; set; }
        public IEnumerable<byte> PreviousBlockHeader { get; set; }
        public IEnumerable<byte> MerkleRoot { get; set; }
        public DateTime Time { get; set; }
        public uint NBits { get; set; }
        public uint Nonce { get; set; }
    }
}

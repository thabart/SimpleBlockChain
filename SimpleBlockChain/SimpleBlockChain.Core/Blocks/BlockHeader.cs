using System;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Blocks
{
    public class BlockHeader
    {
        private static int CURRENT_VERSION = 4;

        public int Version { get; set; }
        public char[] PreviousBlockHeader { get; set; }
        public char[] MerkleRoot { get; set; }
        public int Time { get; set; }
        public uint NBits { get; set; }
        public uint Nonce { get; set; }

        public byte[] Serialize()
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(CURRENT_VERSION));
            return result.ToArray();
        }
    }
}

using System;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Transactions
{
    public class Outpoint
    {
        public IEnumerable<byte> Hash { get; private set; } // hash
        public uint Index { get; private set; } // index

        public Outpoint(IEnumerable<byte> hash, uint index)
        {
            Hash = hash;
            Index = index;
        }

        public IEnumerable<byte> Serialize()
        {
            var result = new List<byte>();
            result.AddRange(Hash);
            result.AddRange(BitConverter.GetBytes(Index));
            return result;
        }
    }
}

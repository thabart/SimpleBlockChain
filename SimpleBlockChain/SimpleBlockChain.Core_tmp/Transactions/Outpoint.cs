using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Transactions
{
    public class Outpoint
    {
        public const int SIZE = 36;
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

        public static Outpoint Deserialize(byte[] payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (payload.Count() != SIZE)
            {
                // TODO : Throw an exception.
            }

            var hash = payload.Take(32);
            var index = BitConverter.ToUInt32(payload.Skip(32).Take(4).ToArray(), 0);
            return new Outpoint(hash, index);
        }
    }
}

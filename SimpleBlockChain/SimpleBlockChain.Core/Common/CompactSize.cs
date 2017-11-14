using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Common
{
    public class CompactSize
    {
        public ulong Size { get; set; }

        public byte[] Serialize()
        {
            if (Size >= 0 && Size <= 252)
            {
                var sb = Convert.ToSByte(Size);
                return new byte[] { (byte)sb };
            }
            else if (Size >= 253 && Size <= 0xffff)
            {
                var result = new List<byte> { 0xfd };
                result.AddRange(BitConverter.GetBytes(Convert.ToUInt16(Size)));
                return result.ToArray();
            }
            else if (Size >= 0x10000 && Size <= 0xffffffff)
            {
                var result = new List<byte> { 0xfe };
                result.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Size)));
                return result.ToArray();
            }
            else if (Size >= 0x100000000 && Size <= 0xffffffffffffffff)
            {
                var result = new List<byte> { 0xff };
                result.AddRange(BitConverter.GetBytes(Size));
                return result.ToArray();
            }

            return null;
        }

        public static KeyValuePair<CompactSize, int> Deserialize(byte[] payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var firstB = payload.First();
            ulong result = 0;
            int nbBytes = 0;
            if (firstB == 0xfd)
            {
                var val = payload.Skip(1).Take(2);
                result = Convert.ToUInt16(val.ToArray());
                nbBytes = 3;
            }
            else if (firstB == 0xfe)
            {
                var val = payload.Skip(1).Take(4);
                result = Convert.ToUInt32(val.ToArray());
                nbBytes = 5;
            }
            else if (firstB == 0xff)
            {
                var val = payload.Skip(1).Take(8);
                result = Convert.ToUInt64(val.ToArray());
                nbBytes = 9;
            }
            else
            {
                result = Convert.ToUInt16((sbyte)firstB);
                nbBytes = 1;
            }

            var r = new CompactSize();
            r.Size = result;
            return new KeyValuePair<CompactSize, int>(r, nbBytes);
        }
    }
}

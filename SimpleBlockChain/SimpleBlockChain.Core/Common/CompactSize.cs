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
    }
}

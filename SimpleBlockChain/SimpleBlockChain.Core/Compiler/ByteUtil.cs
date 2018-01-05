using Org.BouncyCastle.Math;
using System;

namespace SimpleBlockChain.Core.Compiler
{
    public static class ByteUtil
    {
        public static byte[] EMPTY_BYTE_ARRAY = new byte[0];

        public static int FirstNonZeroByte(byte[] data)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                if (data[i] != 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public static byte[] CopyToArray(BigInteger value)
        {
            byte[] src = BigIntegerToBytes(value);
            byte[] dest = new byte[32];;
            Array.Copy(src, 0, dest, dest.Length - src.Length, src.Length);
            return dest;
        }

        public static byte[] BigIntegerToBytes(BigInteger value)
        {
            if (value == null) { return null; }
            byte[] data = value.ToByteArray();
            if (data.Length != 1 && data[0] == 0)
            {
                byte[] tmp = new byte[data.Length - 1];
                Array.Copy(data, 1, tmp, 0, tmp.Length);
                data = tmp;
            }
            return data;
        }

        public static byte[] BigIntegerToBytes(BigInteger b, int numBytes)
        {
            if (b == null) { return null; }
            byte[] bytes = new byte[numBytes];
            byte[] biBytes = b.ToByteArray();
            int start = (biBytes.Length == numBytes + 1) ? 1 : 0;
            int length = Math.Min(biBytes.Length, numBytes);
            Array.Copy(biBytes, start, bytes, numBytes - length, length);
            return bytes;
        }
    }
}

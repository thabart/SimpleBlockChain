using SimpleBlockChain.Core.Helpers;
using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

namespace SimpleBlockChain.Core.Encoding
{
    public static class Base58Encoding
    {
        private const int CHECK_SUM_SIZE = 4;
        private const string DIGITS = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        
        public static string Encode(byte[] data)
        {
            return EncodePlain(_AddCheckSum(data));
        }
        
        public static string EncodePlain(byte[] data)
        {
            var intData = data.Aggregate<byte, BigInteger>(0, (current, t) => current * 256 + t);
            var result = string.Empty;
            while (intData > 0)
            {
                var remainder = (int)(intData % 58);
                intData /= 58;
                result = DIGITS[remainder] + result;
            }            
            for (var i = 0; i < data.Length && data[i] == 0; i++)
            {
                result = '1' + result;
            }

            return result;
        }
        
        public static byte[] Decode(string data)
        {
            var dataWithCheckSum = DecodePlain(data);
            var dataWithoutCheckSum = _VerifyAndRemoveCheckSum(dataWithCheckSum);
            if (dataWithoutCheckSum == null)
            {
                throw new FormatException("Base58 checksum is invalid");
            }

            return dataWithoutCheckSum;
        }

        public static byte[] DecodePlain(string data)
        {
            BigInteger intData = 0;
            for (var i = 0; i < data.Length; i++)
            {
                var digit = DIGITS.IndexOf(data[i]);

                if (digit < 0)
                {
                    throw new FormatException(string.Format("Invalid Base58 character `{0}` at position {1}", data[i], i));
                }

                intData = intData * 58 + digit;
            }
            
            var leadingZeroCount = data.TakeWhile(c => c == '1').Count();
            var leadingZeros = Enumerable.Repeat((byte)0, leadingZeroCount);
            var bytesWithoutLeadingZeros =
              intData.ToByteArray()
              .Reverse()
              .SkipWhile(b => b == 0);
            var result = leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();
            return result;
        }

        private static byte[] _AddCheckSum(byte[] data)
        {
            var checkSum = _GetCheckSum(data);
            var dataWithCheckSum = ArrayHelpers.ConcatArrays(data, checkSum);
            return dataWithCheckSum;
        }
        
        private static byte[] _VerifyAndRemoveCheckSum(byte[] data)
        {
            var result = ArrayHelpers.SubArray(data, 0, data.Length - CHECK_SUM_SIZE);
            var givenCheckSum = ArrayHelpers.SubArray(data, data.Length - CHECK_SUM_SIZE);
            var correctCheckSum = _GetCheckSum(result);
            return givenCheckSum.SequenceEqual(correctCheckSum) ? result : null;
        }

        private static byte[] _GetCheckSum(byte[] data)
        {
            SHA256 sha256 = new SHA256Managed();
            var hash1 = sha256.ComputeHash(data);
            var hash2 = sha256.ComputeHash(hash1);
            var result = new byte[CHECK_SUM_SIZE];
            Buffer.BlockCopy(hash2, 0, result, 0, result.Length);
            return result;
        }
    }
}

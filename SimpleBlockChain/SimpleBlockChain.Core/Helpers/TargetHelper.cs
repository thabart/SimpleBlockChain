using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SimpleBlockChain.Core.Helpers
{
    public class TargetHelper
    {
        public static byte[] GetTarget(uint nbits)
        {
            // TODO : DECOMMENT THE CODE BELOW.
            var result = new List<byte>();
            result.Add(0);
            for (var i = 1; i < 32; i++)
            {
                result.Add(1);
            }

            return result.ToArray();
            /*
            var hexStr = string.Format("0x{0:X}", nbits);
            hexStr = hexStr.Replace("0x", "");
            var nbBytesStr = string.Join("", hexStr.Take(2));
            var prefixStr = string.Join("", hexStr.Skip(2));
            var nbBytes = int.Parse(nbBytesStr, System.Globalization.NumberStyles.HexNumber);
            var prefix = int.Parse(prefixStr, System.Globalization.NumberStyles.HexNumber);
            var result = BitConverter.GetBytes(prefix * Math.Pow(256, (nbBytes - 3))).ToList();
            for (var i = result.Count(); i < 32; i++)
            {
                result.Add(0);
            }

            return result.ToArray();
            */
        }

        public static bool IsValid(byte[] blockHash, byte[] target)
        {
            return true;
            /*
            var nbZero = 0;
            foreach (var b in target)
            {
                if (b == 0)
                {
                    nbZero++;
                    continue;
                }

                break;
            }

            int nbHashZero = 0;
            for (int i = blockHash.Count() - 1; i >= 0; i--)
            {
                if (nbZero == nbHashZero)
                {
                    return true;
                }

                if (blockHash[i] == 0)
                {
                    nbHashZero++;
                    continue;
                }

                return false;
            }

            return false;
            */
        }
    }
}

﻿using System;
using System.Linq;
using System.Numerics;

namespace SimpleBlockChain.Core.Helpers
{
    public class TargetHelper
    {
        public static byte[] GetTarget(uint nbits)
        {
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
        }
    }
}

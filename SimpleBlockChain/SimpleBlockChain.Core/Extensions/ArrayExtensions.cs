using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleBlockChain.Core.Extensions
{
    public static class ArrayExtensions
    {
        public static string ToHexString(this IEnumerable<byte> arr)
        {
            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            var sb = new StringBuilder();
            foreach(var s in arr)
            {
                sb.Append(s.ToString("x2"));
            }

            return sb.ToString();
        }

        public  static IEnumerable<byte> FromHexString(this string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                throw new ArgumentNullException(nameof(hex));
            }

            return Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16));
        }
    }
}

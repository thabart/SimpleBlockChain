using System;
using System.Collections.Generic;
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
    }
}

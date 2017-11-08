using System;

namespace SimpleBlockChain.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToUnixTime(this DateTime dateTime)
        {
            if (dateTime == null)
            {
                throw new ArgumentNullException(nameof(dateTime));
            }

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((dateTime - epoch).TotalSeconds);
        }
    }
}

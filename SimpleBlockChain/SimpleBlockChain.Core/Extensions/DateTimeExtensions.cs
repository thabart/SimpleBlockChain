using System;

namespace SimpleBlockChain.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static UInt32 ToUnixTime(this DateTime dateTime)
        {
            if (dateTime == null)
            {
                throw new ArgumentNullException(nameof(dateTime));
            }

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToUInt32((dateTime - epoch).TotalSeconds);
        }

        public static DateTime ToDateTime(this UInt32 number)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(number).ToLocalTime();
            return dateTime;
        }
    }
}

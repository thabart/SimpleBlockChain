using System;

namespace SimpleBlockChain.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static Int64 ToUnixTime(this DateTime dateTime)
        {
            if (dateTime == null)
            {
                throw new ArgumentNullException(nameof(dateTime));
            }

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((dateTime - epoch).TotalSeconds);
        }

        public static DateTime ToDateTime(this Int64 number)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(number).ToLocalTime();
            return dateTime;
        }

        public static DateTime ToDateTime(this uint number)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(number).ToLocalTime();
            return dateTime;
        }
    }
}

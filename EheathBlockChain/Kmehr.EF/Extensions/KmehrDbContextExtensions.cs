using System;

namespace Kmehr.EF.Extensions
{
    internal static class KmehrDbContextExtensions
    {
        public static void EnsureSeedData(this KmehrDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }


        }
    }
}

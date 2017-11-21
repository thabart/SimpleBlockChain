using System;

namespace SimpleBlockChain.Core.Helpers
{
    public static class NonceHelper
    {
        public static ulong GetNonce()
        {
            var random = new Random();
            byte[] buffer = new byte[8];
            random.NextBytes(buffer);
            short hi = (short)random.Next(4, 0x10000);
            buffer[7] = (byte)(hi >> 8);
            buffer[6] = (byte)hi;
            return (ulong)BitConverter.ToInt64(buffer, 0);
        }
    }
}

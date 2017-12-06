using System;
using System.Runtime.InteropServices;
using System.Security;

namespace SimpleBlockChain.Core.Extensions
{
    public static class SecureStringExtensions
    {
        public static string SecureStringToString(this SecureString value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            IntPtr bstr = Marshal.SecureStringToBSTR(value);
            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }
    }
}

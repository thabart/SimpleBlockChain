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

        public static SecureString ToSecureString(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return null;
            }
            SecureString result = new SecureString();
            foreach (char c in source.ToCharArray())
            {
                result.AppendChar(c);
            }

            return result;
        }
    }
}

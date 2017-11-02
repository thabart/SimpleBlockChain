using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

namespace SimpleBlockChain.Interop
{
    [DebuggerDisplay("{Domain}\\{User}")]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct SEC_WINNT_AUTH_IDENTITY
    {
        private const uint SEC_WINNT_AUTH_IDENTITY_UNICODE = 2;
        private readonly string User;
        private readonly uint UserLength;
        private readonly string Domain;
        private readonly uint DomainLength;
        private readonly string Password;
        private readonly uint PasswordLength;
        private readonly uint Flags;

        public SEC_WINNT_AUTH_IDENTITY(NetworkCredential cred)
        {
            this = new SEC_WINNT_AUTH_IDENTITY(cred.Domain, cred.UserName, cred.Password);
        }

        public SEC_WINNT_AUTH_IDENTITY(string domain, string user, string password)
        {
            this.User = user;
            this.UserLength = (uint)user.Length;
            this.Domain = domain;
            this.DomainLength = (uint)domain.Length;
            this.Password = password;
            this.PasswordLength = (uint)password.Length;
            this.Flags = 2U;
        }
    }
}

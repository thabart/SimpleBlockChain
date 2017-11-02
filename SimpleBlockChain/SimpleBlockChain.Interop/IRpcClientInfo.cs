using System;
using System.Security.Principal;

namespace SimpleBlockChain.Interop
{
    public interface IRpcClientInfo
    {
        bool IsClientLocal { get; }
        byte[] ClientAddress { get; }
        RpcProtoseqType ProtocolType { get; }
        RpcProtectionLevel ProtectionLevel { get; }
        RpcAuthentication AuthenticationLevel { get; }
        IntPtr ClientPid { get; }
        bool IsAuthenticated { get; }
        string ClientPrincipalName { get; }
        WindowsIdentity ClientUser { get; }
        bool IsImpersonating { get; }
        IDisposable Impersonate();
    }
}

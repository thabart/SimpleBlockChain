using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SimpleBlockChain.Interop
{
    [DebuggerDisplay("{_handle} @{_binding}")]
    public class RpcClientApi : IDisposable
    {
        public readonly Guid IID;
        private readonly RpcProtseq _protocol;
        private readonly string _binding;
        private readonly RpcHandle _handle;
        private bool _authenticated;

        public static NetworkCredential Anonymous
        {
            get
            {
                return new NetworkCredential("ANONYMOUS LOGON", "", "NT_AUTHORITY");
            }
        }

        public static NetworkCredential Self
        {
            get
            {
                return (NetworkCredential)null;
            }
        }

        public RpcProtseq Protocol
        {
            get
            {
                return this._protocol;
            }
        }

        public RpcClientApi(Guid iid, RpcProtseq protocol, string server, string endpoint)
        {
            this._handle = (RpcHandle)new RpcClientApi.RpcClientHandle();
            this.IID = iid;
            this._protocol = protocol;
            this._binding = RpcClientApi.StringBindingCompose(protocol, server, endpoint, (string)null);
            this.Connect();
        }

        public void Dispose()
        {
            this._handle.Dispose();
        }

        private void Connect()
        {
            RpcClientApi.BindingFromStringBinding(this._handle, this._binding);
        }

        public void AuthenticateAs(NetworkCredential credentials)
        {
            this.AuthenticateAs((string)null, credentials);
        }

        public void AuthenticateAs(string serverPrincipalName, NetworkCredential credentials)
        {
            RpcAuthentication[] rpcAuthenticationArray = new RpcAuthentication[2]
            {
        RpcAuthentication.RPC_C_AUTHN_GSS_NEGOTIATE,
        RpcAuthentication.RPC_C_AUTHN_WINNT
            };
            RpcProtectionLevel level = RpcProtectionLevel.RPC_C_PROTECT_LEVEL_PKT_PRIVACY;
            if (credentials != null && credentials.UserName == RpcClientApi.Anonymous.UserName && credentials.Domain == RpcClientApi.Anonymous.Domain)
            {
                level = RpcProtectionLevel.RPC_C_PROTECT_LEVEL_DEFAULT;
                rpcAuthenticationArray = new RpcAuthentication[1];
                credentials = (NetworkCredential)null;
            }
            this.AuthenticateAs(serverPrincipalName, credentials, level, rpcAuthenticationArray);
        }

        public void AuthenticateAs(string serverPrincipalName, NetworkCredential credentials, RpcProtectionLevel level, params RpcAuthentication[] authTypes)
        {
            if (this._authenticated)
                return;
            RpcClientApi.BindingSetAuthInfo(level, authTypes, this._handle, serverPrincipalName, credentials);
            this._authenticated = true;
        }

        public byte[] Execute(byte[] input)
        {
            if (!this._authenticated)
                this.AuthenticateAs(RpcClientApi.Anonymous);
            return RpcClientApi.InvokeRpc(this._handle, this.IID, input);
        }

        [DllImport("Rpcrt4.dll", EntryPoint = "RpcStringFreeW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern RpcError RpcStringFree(ref IntPtr lpString);

        [DllImport("Rpcrt4.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern RpcError RpcBindingFree(ref IntPtr lpString);

        [DllImport("Rpcrt4.dll", EntryPoint = "RpcStringBindingComposeW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern RpcError RpcStringBindingCompose(string ObjUuid, string ProtSeq, string NetworkAddr, string Endpoint, string Options, out IntPtr lpBindingString);

        private static string StringBindingCompose(RpcProtseq ProtSeq, string NetworkAddr, string Endpoint, string Options)
        {
            IntPtr lpBindingString;
            RpcException.Assert(RpcClientApi.RpcStringBindingCompose((string)null, ProtSeq.ToString(), NetworkAddr, Endpoint, Options, out lpBindingString));
            try
            {
                return Marshal.PtrToStringUni(lpBindingString);
            }
            finally
            {
                RpcException.Assert(RpcClientApi.RpcStringFree(ref lpBindingString));
            }
        }

        [DllImport("Rpcrt4.dll", EntryPoint = "RpcBindingFromStringBindingW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern RpcError RpcBindingFromStringBinding(string bindingString, out IntPtr lpBinding);

        private static void BindingFromStringBinding(RpcHandle handle, string bindingString)
        {
            RpcException.Assert(RpcClientApi.RpcBindingFromStringBinding(bindingString, out handle.Handle));
        }

        [DllImport("Rpcrt4.dll", EntryPoint = "RpcBindingSetAuthInfoW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern RpcError RpcBindingSetAuthInfo(IntPtr Binding, string ServerPrincName, RpcProtectionLevel AuthnLevel, RpcAuthentication AuthnSvc, [In] ref SEC_WINNT_AUTH_IDENTITY AuthIdentity, uint AuthzSvc);

        [DllImport("Rpcrt4.dll", EntryPoint = "RpcBindingSetAuthInfoW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern RpcError RpcBindingSetAuthInfo2(IntPtr Binding, string ServerPrincName, RpcProtectionLevel AuthnLevel, RpcAuthentication AuthnSvc, IntPtr p, uint AuthzSvc);

        private static void BindingSetAuthInfo(RpcProtectionLevel level, RpcAuthentication[] authTypes, RpcHandle handle, string serverPrincipalName, NetworkCredential credentails)
        {
            if (credentails == null)
            {
                foreach (RpcAuthentication AuthnSvc in authTypes)
                {
                    int num = (int)RpcClientApi.RpcBindingSetAuthInfo2(handle.Handle, serverPrincipalName, level, AuthnSvc, IntPtr.Zero, 0U);
                }
            }
            else
            {
                SEC_WINNT_AUTH_IDENTITY AuthIdentity = new SEC_WINNT_AUTH_IDENTITY(credentails);
                foreach (RpcAuthentication AuthnSvc in authTypes)
                {
                    int num = (int)RpcClientApi.RpcBindingSetAuthInfo(handle.Handle, serverPrincipalName, level, AuthnSvc, ref AuthIdentity, 0U);
                }
            }
        }

        [DllImport("Rpcrt4.dll", EntryPoint = "NdrClientCall2", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr NdrClientCall2x86(IntPtr pMIDL_STUB_DESC, IntPtr formatString, IntPtr args);

        [DllImport("Rpcrt4.dll", EntryPoint = "NdrClientCall2", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr NdrClientCall2x64(IntPtr pMIDL_STUB_DESC, IntPtr formatString, IntPtr Handle, int DataSize, IntPtr Data, out int ResponseSize, out IntPtr Response);

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static byte[] InvokeRpc(RpcHandle handle, Guid iid, byte[] input)
        {
            Ptr<MIDL_STUB_DESC> ptr1;
            if (!handle.GetPtr<Ptr<MIDL_STUB_DESC>>(out ptr1))
                ptr1 = handle.CreatePtr<MIDL_STUB_DESC>(new MIDL_STUB_DESC(handle, handle.Pin<RPC_CLIENT_INTERFACE>(new RPC_CLIENT_INTERFACE(iid)), RpcApi.TYPE_FORMAT, false));
            int ResponseSize = 0;
            IntPtr Response;
            IntPtr num;
            using (Ptr<byte[]> ptr2 = new Ptr<byte[]>(input))
            {
                if (RpcApi.Is64BitProcess)
                {
                    try
                    {
                        num = RpcClientApi.NdrClientCall2x64(ptr1.Handle, RpcApi.FUNC_FORMAT_PTR.Handle, handle.Handle, input.Length, ptr2.Handle, out ResponseSize, out Response);
                    }
                    catch (SEHException ex)
                    {
                        RpcException.Assert(ex.ErrorCode);
                        throw;
                    }
                }
                else
                {
                    using (Ptr<int[]> ptr3 = new Ptr<int[]>(new int[10]))
                    {
                        ptr3.Data[0] = handle.Handle.ToInt32();
                        ptr3.Data[1] = input.Length;
                        ptr3.Data[2] = ptr2.Handle.ToInt32();
                        ptr3.Data[3] = ptr3.Handle.ToInt32() + 24;
                        ptr3.Data[4] = ptr3.Handle.ToInt32() + 32;
                        ptr3.Data[5] = 0;
                        ptr3.Data[6] = 0;
                        ptr3.Data[8] = 0;
                        try
                        {
                            num = RpcClientApi.NdrClientCall2x86(ptr1.Handle, RpcApi.FUNC_FORMAT_PTR.Handle, ptr3.Handle);
                        }
                        catch (SEHException ex)
                        {
                            RpcException.Assert(ex.ErrorCode);
                            throw;
                        }
                        ResponseSize = ptr3.Data[6];
                        Response = new IntPtr(ptr3.Data[8]);
                    }
                }
                GC.KeepAlive((object)ptr2);
            }

            RpcException.Assert(num.ToInt32());
            byte[] destination = new byte[ResponseSize];
            if (ResponseSize > 0 && Response != IntPtr.Zero)
                Marshal.Copy(Response, destination, 0, destination.Length);
            RpcApi.Free(Response);
            return destination;
        }

        private class RpcClientHandle : RpcHandle
        {
            protected override void DisposeHandle(ref IntPtr handle)
            {
                if (!(handle != IntPtr.Zero))
                    return;
                RpcException.Assert(RpcClientApi.RpcBindingFree(ref this.Handle));
                handle = IntPtr.Zero;
            }
        }
    }
}

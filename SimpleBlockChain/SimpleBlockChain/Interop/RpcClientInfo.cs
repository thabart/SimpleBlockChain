using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace SimpleBlockChain.Interop
{
    internal class RpcClientInfo : IRpcClientInfo, IDisposable
    {
        private readonly IntPtr _clientHandle;
        private WindowsIdentity _user;
        private bool _impersonating;
        private RPC_CALL_ATTRIBUTES_V2 _callAttrs;
        private byte[] _clientAddress;
        private string _clientPrincipalName;
        private bool _isAuthenticated;

        public bool IsImpersonating
        {
            get
            {
                return this._impersonating;
            }
        }

        public WindowsIdentity ClientUser
        {
            get
            {
                if (this._user == null)
                {
                    if (this.IsAuthenticated)
                    {
                        using (this.Impersonate())
                            this._user = WindowsIdentity.GetCurrent(true);
                    }
                    else
                        this._user = WindowsIdentity.GetAnonymous();
                }
                return this._user;
            }
        }

        public string ClientPrincipalName
        {
            get
            {
                this.GetCallInfo();
                return this._clientPrincipalName;
            }
        }

        public byte[] ClientAddress
        {
            get
            {
                this.GetCallInfo();
                if (this._clientAddress != null)
                    return (byte[])this._clientAddress.Clone();
                return new byte[0];
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                if (this.GetCallInfo().AuthenticationService == RpcAuthentication.RPC_C_AUTHN_NONE)
                    return this._isAuthenticated;
                return true;
            }
        }

        public bool IsClientLocal
        {
            get
            {
                if (this.GetCallInfo().ProtocolSequence != RpcProtoseqType.LRPC)
                    return this.GetCallInfo().IsClientLocal == RpcCallClientLocality.Local;
                return true;
            }
        }

        public RpcProtoseqType ProtocolType
        {
            get
            {
                return this.GetCallInfo().ProtocolSequence;
            }
        }

        public RpcProtectionLevel ProtectionLevel
        {
            get
            {
                return this.GetCallInfo().AuthenticationLevel;
            }
        }

        public RpcAuthentication AuthenticationLevel
        {
            get
            {
                return this.GetCallInfo().AuthenticationService;
            }
        }

        public IntPtr ClientPid
        {
            get
            {
                return this.GetCallInfo().ClientPID;
            }
        }

        public RpcClientInfo(IntPtr clientHandle)
        {
            this._user = (WindowsIdentity)null;
            this._impersonating = false;
            this._clientHandle = clientHandle;
        }

        public IDisposable Impersonate()
        {
            if (this._impersonating)
                return (IDisposable)new RpcClientInfo.IgnoreOnDispose();
            if (!this.IsAuthenticated)
                throw new UnauthorizedAccessException();
            // ISSUE: reference to a compiler-generated method
            RpcException.Assert(RpcClientInfo.RpcImpersonateClient(this._clientHandle));
            this._impersonating = true;
            return (IDisposable)new RpcClientInfo.RpcImpersonationContext(this);
        }

        private void RevertToSelf()
        {
            if (this._impersonating)
            {
                // ISSUE: reference to a compiler-generated method
                RpcException.Assert(RpcClientInfo.RpcRevertToSelfEx(this._clientHandle));
            }
            this._impersonating = false;
        }

        public void Dispose()
        {
            this.RevertToSelf();
            if (this._user != null)
                this._user.Dispose();
            this._user = (WindowsIdentity)null;
        }

        private RPC_CALL_ATTRIBUTES_V2 GetCallInfo()
        {
            if ((int)this._callAttrs.Version != 0)
                return this._callAttrs;
            RPC_CALL_ATTRIBUTES_V2 attributes = new RPC_CALL_ATTRIBUTES_V2();
            attributes.Version = 2U;
            attributes.Flags = RPC_CALL_ATTRIBUTES_FLAGS.RPC_QUERY_NO_AUTH_REQUIRED;
            RpcError rpcError1 = RpcClientInfo.RpcServerInqCallAttributes(this._clientHandle, ref attributes);
            if (rpcError1 == RpcError.RPC_S_INVALID_ARG)
            {
                attributes.Version = 1U;
                rpcError1 = RpcClientInfo.RpcServerInqCallAttributes(this._clientHandle, ref attributes);
            }
            if (rpcError1 == RpcError.RPC_S_OK)
            {
                this._callAttrs = attributes;
                this._isAuthenticated = false;
                attributes.Flags = RPC_CALL_ATTRIBUTES_FLAGS.RPC_QUERY_IS_CLIENT_LOCAL | RPC_CALL_ATTRIBUTES_FLAGS.RPC_QUERY_NO_AUTH_REQUIRED;
                RpcError rpcError2;
                if ((rpcError2 = RpcClientInfo.RpcServerInqCallAttributes(this._clientHandle, ref attributes)) == RpcError.RPC_S_OK)
                {
                    this._callAttrs.IsClientLocal = attributes.IsClientLocal;
                    if (this._callAttrs.ProtocolSequence == RpcProtoseqType.LRPC)
                    {
                        attributes.Flags = RPC_CALL_ATTRIBUTES_FLAGS.RPC_QUERY_CLIENT_PID;
                        if ((rpcError2 = RpcClientInfo.RpcServerInqCallAttributes(this._clientHandle, ref attributes)) == RpcError.RPC_S_OK)
                            this._callAttrs.ClientPID = attributes.ClientPID;
                    }
                }
                if (this._callAttrs.ProtocolSequence != RpcProtoseqType.LRPC)
                {
                    using (Ptr<byte[]> ptr1 = new Ptr<byte[]>(new byte[1024]))
                    {
                        RPC_CALL_LOCAL_ADDRESS_V1 data = new RPC_CALL_LOCAL_ADDRESS_V1();
                        data.Version = 1U;
                        data.Buffer = ptr1.Handle;
                        data.BufferSize = 1024;
                        data.AddressFormat = RpcLocalAddressFormat.Invalid;
                        this._callAttrs = attributes;
                        using (Ptr<RPC_CALL_LOCAL_ADDRESS_V1> ptr2 = new Ptr<RPC_CALL_LOCAL_ADDRESS_V1>(data))
                        {
                            attributes.CallLocalAddress = ptr2.Handle;
                            attributes.Flags = RPC_CALL_ATTRIBUTES_FLAGS.RPC_QUERY_CALL_LOCAL_ADDRESS | RPC_CALL_ATTRIBUTES_FLAGS.RPC_QUERY_NO_AUTH_REQUIRED;
                            if ((rpcError2 = RpcClientInfo.RpcServerInqCallAttributes(this._clientHandle, ref attributes)) == RpcError.RPC_S_OK)
                            {
                                this._clientAddress = new byte[ptr2.Data.BufferSize];
                                Array.Copy((Array)ptr1.Data, (Array)this._clientAddress, this._clientAddress.Length);
                            }
                        }
                    }
                }
                using (Ptr<byte[]> ptr = new Ptr<byte[]>(new byte[1024]))
                {
                    attributes.ClientPrincipalName = ptr.Handle;
                    attributes.ClientPrincipalNameBufferLength = 1024;
                    attributes.Flags = RPC_CALL_ATTRIBUTES_FLAGS.RPC_QUERY_CLIENT_PRINCIPAL_NAME;
                    if ((rpcError2 = RpcClientInfo.RpcServerInqCallAttributes(this._clientHandle, ref attributes)) == RpcError.RPC_S_OK)
                    {
                        this._clientPrincipalName = Marshal.PtrToStringUni(ptr.Handle);
                        if (!string.IsNullOrEmpty(this._clientPrincipalName))
                        {
                            this._isAuthenticated = true;
                            if ((int)attributes.Version == 1)
                                this._callAttrs.IsClientLocal = RpcCallClientLocality.Local;
                        }
                    }
                }
            }
            return this._callAttrs;
        }

        [DllImport("Rpcrt4.dll", EntryPoint = "RpcServerInqCallAttributesW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern RpcError RpcServerInqCallAttributes(IntPtr binding, [In, Out] ref RPC_CALL_ATTRIBUTES_V2 attributes);

        [DllImport("Rpcrt4.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern RpcError RpcImpersonateClient(IntPtr binding);

        [DllImport("Rpcrt4.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern RpcError RpcRevertToSelfEx(IntPtr binding);

        private class IgnoreOnDispose : IDisposable
        {
            void IDisposable.Dispose()
            {
            }
        }

        private class RpcImpersonationContext : IDisposable
        {
            private readonly RpcClientInfo _client;

            public RpcImpersonationContext(RpcClientInfo client)
            {
                this._client = client;
            }

            void IDisposable.Dispose()
            {
                this._client.RevertToSelf();
            }
        }
    }
}

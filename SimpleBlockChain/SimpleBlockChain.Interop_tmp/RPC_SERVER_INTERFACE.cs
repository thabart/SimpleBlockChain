using System;
using System.Runtime.InteropServices;

namespace SimpleBlockChain.Interop
{
    internal struct RPC_SERVER_INTERFACE
    {
        public static readonly Guid IID_SYNTAX = new Guid(2324192516U, (ushort)7403, (ushort)4553, (byte)159, (byte)232, (byte)8, (byte)0, (byte)43, (byte)16, (byte)72, (byte)96);
        public uint Length;
        public RPC_SYNTAX_IDENTIFIER InterfaceId;
        public RPC_SYNTAX_IDENTIFIER TransferSyntax;
        public IntPtr DispatchTable;
        public uint RpcProtseqEndpointCount;
        public IntPtr RpcProtseqEndpoint;
        public IntPtr DefaultManagerEpv;
        public IntPtr InterpreterInfo;
        public uint Flags;

        public RPC_SERVER_INTERFACE(RpcHandle handle, Ptr<MIDL_SERVER_INFO> pServer, Guid iid)
        {
            this.Length = (uint)Marshal.SizeOf(typeof(RPC_CLIENT_INTERFACE));
            this.InterfaceId = new RPC_SYNTAX_IDENTIFIER()
            {
                SyntaxGUID = iid,
                SyntaxVersion = RPC_VERSION.INTERFACE_VERSION
            };
            this.TransferSyntax = new RPC_SYNTAX_IDENTIFIER()
            {
                SyntaxGUID = RPC_SERVER_INTERFACE.IID_SYNTAX,
                SyntaxVersion = RPC_VERSION.SYNTAX_VERSION
            };
            this.DispatchTable = handle.Pin<RPC_DISPATCH_TABLE>(new RPC_DISPATCH_TABLE()
            {
                DispatchTableCount = 1U,
                DispatchTable = handle.Pin<RPC_DISPATCH_TABLE_Entry>(new RPC_DISPATCH_TABLE_Entry()
                {
                    DispatchMethod = RpcApi.ServerEntry.Handle,
                    Zero = IntPtr.Zero
                }),
                Reserved = IntPtr.Zero
            });
            this.RpcProtseqEndpointCount = 0U;
            this.RpcProtseqEndpoint = IntPtr.Zero;
            this.DefaultManagerEpv = IntPtr.Zero;
            this.InterpreterInfo = pServer.Handle;
            this.Flags = 67108864U;
        }
    }
}

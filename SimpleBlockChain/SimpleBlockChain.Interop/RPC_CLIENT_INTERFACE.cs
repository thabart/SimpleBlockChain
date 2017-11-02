using System;
using System.Runtime.InteropServices;

namespace SimpleBlockChain.Interop
{
    internal struct RPC_CLIENT_INTERFACE
    {
        public static readonly Guid IID_SYNTAX = new Guid(2324192516U, (ushort)7403, (ushort)4553, (byte)159, (byte)232, (byte)8, (byte)0, (byte)43, (byte)16, (byte)72, (byte)96);
        public uint Length;
        public RPC_SYNTAX_IDENTIFIER InterfaceId;
        public RPC_SYNTAX_IDENTIFIER TransferSyntax;
        public IntPtr DispatchTable;
        public uint RpcProtseqEndpointCount;
        public IntPtr RpcProtseqEndpoint;
        public IntPtr Reserved;
        public IntPtr InterpreterInfo;
        public uint Flags;

        public RPC_CLIENT_INTERFACE(Guid iid)
        {
            this.Length = (uint)Marshal.SizeOf(typeof(RPC_CLIENT_INTERFACE));
            this.InterfaceId = new RPC_SYNTAX_IDENTIFIER()
            {
                SyntaxGUID = iid,
                SyntaxVersion = RPC_VERSION.INTERFACE_VERSION
            };
            this.TransferSyntax = new RPC_SYNTAX_IDENTIFIER()
            {
                SyntaxGUID = RPC_CLIENT_INTERFACE.IID_SYNTAX,
                SyntaxVersion = RPC_VERSION.SYNTAX_VERSION
            };
            this.DispatchTable = IntPtr.Zero;
            this.RpcProtseqEndpointCount = 0U;
            this.RpcProtseqEndpoint = IntPtr.Zero;
            this.Reserved = IntPtr.Zero;
            this.InterpreterInfo = IntPtr.Zero;
            this.Flags = 0U;
        }
    }
}

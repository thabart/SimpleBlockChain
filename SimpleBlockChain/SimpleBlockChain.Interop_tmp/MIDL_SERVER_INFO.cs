using System;
using System.Runtime.InteropServices;

namespace SimpleBlockChain.Interop
{
    internal struct MIDL_SERVER_INFO
    {
        public IntPtr pStubDesc;
        public IntPtr DispatchTable;
        public IntPtr ProcString;
        public IntPtr FmtStringOffset;
        private IntPtr ThunkTable;
        private IntPtr pTransferSyntax;
        private IntPtr nCount;
        private IntPtr pSyntaxInfo;

        internal static Ptr<RPC_SERVER_INTERFACE> Create(RpcHandle handle, Guid iid, byte[] formatTypes, byte[] formatProc, RpcExecute fnExecute)
        {
            Ptr<MIDL_SERVER_INFO> ptr = handle.CreatePtr<MIDL_SERVER_INFO>(new MIDL_SERVER_INFO());
            return new MIDL_SERVER_INFO().Configure(handle, ptr, iid, formatTypes, formatProc, fnExecute);
        }

        private Ptr<RPC_SERVER_INTERFACE> Configure(RpcHandle handle, Ptr<MIDL_SERVER_INFO> me, Guid iid, byte[] formatTypes, byte[] formatProc, RpcExecute fnExecute)
        {
            Ptr<RPC_SERVER_INTERFACE> ptr = handle.CreatePtr<RPC_SERVER_INTERFACE>(new RPC_SERVER_INTERFACE(handle, me, iid));
            this.pStubDesc = handle.CreatePtr<MIDL_STUB_DESC>(new MIDL_STUB_DESC(handle, ptr.Handle, formatTypes, true)).Handle;
            IntPtr data = handle.PinFunction<RpcExecute>(fnExecute);
            this.DispatchTable = handle.Pin<IntPtr>(data);
            this.ProcString = handle.Pin<byte[]>(formatProc);
            this.FmtStringOffset = handle.Pin<int[]>(new int[1]);
            this.ThunkTable = IntPtr.Zero;
            this.pTransferSyntax = IntPtr.Zero;
            this.nCount = IntPtr.Zero;
            this.pSyntaxInfo = IntPtr.Zero;
            Marshal.StructureToPtr((object)this, me.Handle, false);
            return ptr;
        }
    }
}

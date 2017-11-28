using System;

namespace SimpleBlockChain.Interop
{
    internal struct MIDL_STUB_DESC
    {
        public IntPtr RpcInterfaceInformation;
        public IntPtr pfnAllocate;
        public IntPtr pfnFree;
        public IntPtr pAutoBindHandle;
        private IntPtr apfnNdrRundownRoutines;
        private IntPtr aGenericBindingRoutinePairs;
        private IntPtr apfnExprEval;
        private IntPtr aXmitQuintuple;
        public IntPtr pFormatTypes;
        public int fCheckBounds;
        public uint Version;
        private IntPtr pMallocFreeStruct;
        public int MIDLVersion;
        public IntPtr CommFaultOffsets;
        private IntPtr aUserMarshalQuadruple;
        private IntPtr NotifyRoutineTable;
        public IntPtr mFlags;
        private IntPtr CsRoutineTables;
        private IntPtr ProxyServerInfo;
        private IntPtr pExprInfo;

        public MIDL_STUB_DESC(RpcHandle handle, IntPtr interfaceInfo, byte[] formatTypes, bool serverSide)
        {
            this.RpcInterfaceInformation = interfaceInfo;
            this.pfnAllocate = RpcApi.AllocPtr.Handle;
            this.pfnFree = RpcApi.FreePtr.Handle;
            this.pAutoBindHandle = serverSide ? IntPtr.Zero : handle.Pin<IntPtr>(new IntPtr());
            this.apfnNdrRundownRoutines = new IntPtr();
            this.aGenericBindingRoutinePairs = new IntPtr();
            this.apfnExprEval = new IntPtr();
            this.aXmitQuintuple = new IntPtr();
            this.pFormatTypes = handle.Pin<byte[]>(formatTypes);
            this.fCheckBounds = 1;
            this.Version = 327682U;
            this.pMallocFreeStruct = new IntPtr();
            this.MIDLVersion = 117441012;
            IntPtr num;
            if (!serverSide)
                num = handle.Pin<COMM_FAULT_OFFSETS>(new COMM_FAULT_OFFSETS()
                {
                    CommOffset = (short)-1,
                    FaultOffset = (short)-1
                });
            else
                num = IntPtr.Zero;
            this.CommFaultOffsets = num;
            this.aUserMarshalQuadruple = new IntPtr();
            this.NotifyRoutineTable = new IntPtr();
            this.mFlags = new IntPtr(1);
            this.CsRoutineTables = new IntPtr();
            this.ProxyServerInfo = new IntPtr();
            this.pExprInfo = new IntPtr();
        }
    }
}

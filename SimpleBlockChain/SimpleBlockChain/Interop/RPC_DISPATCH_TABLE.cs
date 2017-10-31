using System;

namespace SimpleBlockChain.Interop
{
    internal struct RPC_DISPATCH_TABLE
    {
        public uint DispatchTableCount;
        public IntPtr DispatchTable;
        public IntPtr Reserved;
    }
}

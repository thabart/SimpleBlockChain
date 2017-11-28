using System;

namespace SimpleBlockChain.Interop
{
    internal struct RPC_DISPATCH_TABLE_Entry
    {
        public IntPtr DispatchMethod;
        public IntPtr Zero;
    }
}

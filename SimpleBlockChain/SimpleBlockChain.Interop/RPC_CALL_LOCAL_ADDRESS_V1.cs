using System;

namespace SimpleBlockChain.Interop
{
    internal struct RPC_CALL_LOCAL_ADDRESS_V1
    {
        public uint Version;
        public IntPtr Buffer;
        public int BufferSize;
        public RpcLocalAddressFormat AddressFormat;
    }
}

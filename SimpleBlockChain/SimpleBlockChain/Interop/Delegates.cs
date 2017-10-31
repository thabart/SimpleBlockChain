using System;

namespace SimpleBlockChain.Interop
{
    internal delegate uint RpcExecute(IntPtr clientHandle, uint szInput, IntPtr input, out uint szOutput, out IntPtr output);
    internal delegate IntPtr LocalAlloc(uint size);
    internal delegate void LocalFree(IntPtr ptr);
}

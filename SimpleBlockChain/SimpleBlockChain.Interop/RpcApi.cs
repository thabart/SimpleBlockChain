using System;
using System.Runtime.InteropServices;

namespace SimpleBlockChain.Interop
{
    internal static class RpcApi
    {
        internal static FunctionPtr<RpcApi.ServerEntryPoint> ServerEntry = new FunctionPtr<RpcApi.ServerEntryPoint>(new RpcApi.ServerEntryPoint(RpcApi.NdrServerCall2));
        internal static FunctionPtr<LocalAlloc> AllocPtr = new FunctionPtr<LocalAlloc>(new LocalAlloc(RpcApi.Alloc));
        internal static FunctionPtr<LocalFree> FreePtr = new FunctionPtr<LocalFree>(new LocalFree(RpcApi.Free));
        internal static readonly bool Is64BitProcess = IntPtr.Size == 8;
        private const uint LPTR = 64;
        internal static readonly byte[] TYPE_FORMAT;
        internal static readonly byte[] FUNC_FORMAT;
        internal static readonly Ptr<byte[]> FUNC_FORMAT_PTR;

        static RpcApi()
        {
            if (RpcApi.Is64BitProcess)
            {
                RpcApi.TYPE_FORMAT = new byte[39]
                {
          (byte) 0,
          (byte) 0,
          (byte) 27,
          (byte) 0,
          (byte) 1,
          (byte) 0,
          (byte) 40,
          (byte) 0,
          (byte) 8,
          (byte) 0,
          (byte) 1,
          (byte) 0,
          (byte) 1,
          (byte) 91,
          (byte) 17,
          (byte) 12,
          (byte) 8,
          (byte) 92,
          (byte) 17,
          (byte) 20,
          (byte) 2,
          (byte) 0,
          (byte) 18,
          (byte) 0,
          (byte) 2,
          (byte) 0,
          (byte) 27,
          (byte) 0,
          (byte) 1,
          (byte) 0,
          (byte) 40,
          (byte) 84,
          (byte) 24,
          (byte) 0,
          (byte) 1,
          (byte) 0,
          (byte) 1,
          (byte) 91,
          (byte) 0
                };
                RpcApi.FUNC_FORMAT = new byte[61]
                {
          (byte) 0,
          (byte) 104,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 48,
          (byte) 0,
          (byte) 50,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 8,
          (byte) 0,
          (byte) 36,
          (byte) 0,
          (byte) 71,
          (byte) 5,
          (byte) 10,
          (byte) 7,
          (byte) 1,
          (byte) 0,
          (byte) 1,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 72,
          (byte) 0,
          (byte) 8,
          (byte) 0,
          (byte) 8,
          (byte) 0,
          (byte) 11,
          (byte) 0,
          (byte) 16,
          (byte) 0,
          (byte) 2,
          (byte) 0,
          (byte) 80,
          (byte) 33,
          (byte) 24,
          (byte) 0,
          (byte) 8,
          (byte) 0,
          (byte) 19,
          (byte) 32,
          (byte) 32,
          (byte) 0,
          (byte) 18,
          (byte) 0,
          (byte) 112,
          (byte) 0,
          (byte) 40,
          (byte) 0,
          (byte) 16,
          (byte) 0,
          (byte) 0
                };
            }
            else
            {
                RpcApi.TYPE_FORMAT = new byte[39]
                {
          (byte) 0,
          (byte) 0,
          (byte) 27,
          (byte) 0,
          (byte) 1,
          (byte) 0,
          (byte) 40,
          (byte) 0,
          (byte) 4,
          (byte) 0,
          (byte) 1,
          (byte) 0,
          (byte) 1,
          (byte) 91,
          (byte) 17,
          (byte) 12,
          (byte) 8,
          (byte) 92,
          (byte) 17,
          (byte) 20,
          (byte) 2,
          (byte) 0,
          (byte) 18,
          (byte) 0,
          (byte) 2,
          (byte) 0,
          (byte) 27,
          (byte) 0,
          (byte) 1,
          (byte) 0,
          (byte) 40,
          (byte) 84,
          (byte) 12,
          (byte) 0,
          (byte) 1,
          (byte) 0,
          (byte) 1,
          (byte) 91,
          (byte) 0
                };
                RpcApi.FUNC_FORMAT = new byte[59]
                {
          (byte) 0,
          (byte) 104,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 24,
          (byte) 0,
          (byte) 50,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 8,
          (byte) 0,
          (byte) 36,
          (byte) 0,
          (byte) 71,
          (byte) 5,
          (byte) 8,
          (byte) 7,
          (byte) 1,
          (byte) 0,
          (byte) 1,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 72,
          (byte) 0,
          (byte) 4,
          (byte) 0,
          (byte) 8,
          (byte) 0,
          (byte) 11,
          (byte) 0,
          (byte) 8,
          (byte) 0,
          (byte) 2,
          (byte) 0,
          (byte) 80,
          (byte) 33,
          (byte) 12,
          (byte) 0,
          (byte) 8,
          (byte) 0,
          (byte) 19,
          (byte) 32,
          (byte) 16,
          (byte) 0,
          (byte) 18,
          (byte) 0,
          (byte) 112,
          (byte) 0,
          (byte) 20,
          (byte) 0,
          (byte) 16,
          (byte) 0,
          (byte) 0
                };
            }
            RpcApi.FUNC_FORMAT_PTR = new Ptr<byte[]>(RpcApi.FUNC_FORMAT);
        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LocalFree(IntPtr memHandle);

        internal static void Free(IntPtr ptr)
        {
            if (!(ptr != IntPtr.Zero))
                return;
            RpcApi.LocalFree(ptr);
        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LocalAlloc(uint flags, uint nBytes);

        internal static IntPtr Alloc(uint size)
        {
            return RpcApi.LocalAlloc(64U, size);
        }

        [DllImport("Rpcrt4.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern void NdrServerCall2(IntPtr ptr);

        internal delegate void ServerEntryPoint(IntPtr ptr);
    }
}

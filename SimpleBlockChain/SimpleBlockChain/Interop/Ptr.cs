using System;
using System.Runtime.InteropServices;

namespace SimpleBlockChain.Interop
{
    internal class Ptr<T> : IDisposable
    {
        private readonly GCHandle _handle;

        public T Data
        {
            get
            {
                return (T)this._handle.Target;
            }
        }

        public IntPtr Handle
        {
            get
            {
                return this._handle.AddrOfPinnedObject();
            }
        }

        public Ptr(T data)
        {
            this._handle = GCHandle.Alloc((object)data, GCHandleType.Pinned);
        }

        public void Dispose()
        {
            this._handle.Free();
        }
    }
}

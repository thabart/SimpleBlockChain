using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SimpleBlockChain.Interop
{
    [DebuggerDisplay("{Handle}")]
    internal abstract class RpcHandle : IDisposable
    {
        private readonly List<IDisposable> _pinnedAddresses = new List<IDisposable>();
        internal IntPtr Handle;

        ~RpcHandle()
        {
            this.Dispose(false);
        }

        internal IntPtr PinFunction<T>(T data) where T : class
        {
            FunctionPtr<T> functionPtr = new FunctionPtr<T>(data);
            this._pinnedAddresses.Add((IDisposable)functionPtr);
            return functionPtr.Handle;
        }

        internal IntPtr Pin<T>(T data)
        {
            return this.CreatePtr<T>(data).Handle;
        }

        internal bool GetPtr<T>(out T value)
        {
            foreach (object obj in this._pinnedAddresses)
            {
                if (obj is T)
                {
                    value = (T)obj;
                    return true;
                }
            }
            value = default(T);
            return false;
        }

        internal Ptr<T> CreatePtr<T>(T data)
        {
            Ptr<T> ptr = new Ptr<T>(data);
            this._pinnedAddresses.Add((IDisposable)ptr);
            return ptr;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            try
            {
                if (this.Handle != IntPtr.Zero)
                    this.DisposeHandle(ref this.Handle);
                for (int index = this._pinnedAddresses.Count - 1; index >= 0; --index)
                    this._pinnedAddresses[index].Dispose();
                this._pinnedAddresses.Clear();
            }
            finally
            {
                this.Handle = IntPtr.Zero;
            }
            if (!disposing)
                return;
            GC.SuppressFinalize((object)this);
        }

        protected abstract void DisposeHandle(ref IntPtr handle);
    }
}

using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace SimpleBlockChain.Interop
{
    internal class FunctionPtr<T> : IDisposable where T : class, ICloneable, ISerializable
    {
        private T _delegate;
        public IntPtr Handle;

        public FunctionPtr(T data)
        {
            this._delegate = data;
            this.Handle = Marshal.GetFunctionPointerForDelegate((Delegate)(object)data);
        }

        void IDisposable.Dispose()
        {
            this._delegate = default(T);
            this.Handle = IntPtr.Zero;
        }
    }
}

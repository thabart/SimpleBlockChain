using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SimpleBlockChain.Interop
{
    public class RpcException : Win32Exception
    {
        public RpcError RpcError
        {
            get
            {
                return (RpcError)this.NativeErrorCode;
            }
        }

        protected RpcException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        public RpcException()
          : base()
        {
        }

        public RpcException(string message)
          : base(message)
        {
        }

        public RpcException(RpcError errorCode)
          : base((int)errorCode)
        {
        }

        public static void Assert(bool condition)
        {
            if (!condition)
            {
                throw new RpcException();
            }
        }

        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new RpcException(message);
            }
        }

        [DebuggerNonUserCode]
        internal static void Assert(int rawError)
        {
            RpcException.Assert((RpcError)rawError);
        }

        [DebuggerNonUserCode]
        public static void Assert(RpcError errorCode)
        {
            if (errorCode != RpcError.RPC_S_OK)
            {
                RpcException rpcException = new RpcException(errorCode);
                throw rpcException;
            }
        }
    }
}

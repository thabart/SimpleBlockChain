using System;

namespace SimpleBlockChain.Core.Exceptions
{
    public class RpcException : Exception
    {
        public RpcException(string code) : base(code) { }
    }
}

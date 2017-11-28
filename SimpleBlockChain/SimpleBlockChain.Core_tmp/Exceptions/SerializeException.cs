using System;

namespace SimpleBlockChain.Core.Exceptions
{
    public class SerializeException : Exception
    {
        public SerializeException(string message) : base(message) { }
    }
}

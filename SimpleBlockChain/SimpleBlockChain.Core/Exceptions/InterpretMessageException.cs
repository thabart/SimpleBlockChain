using System;

namespace SimpleBlockChain.Core.Exceptions
{
    public class InterpretMessageException : Exception
    {
        public InterpretMessageException(string message) : base(message)
        {

        }
    }
}

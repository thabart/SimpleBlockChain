using System;

namespace SimpleBlockChain.Core.Exceptions
{
    public class ParseMessageException : Exception
    {
        public ParseMessageException(string message) : base(message)
        {

        }
    }
}

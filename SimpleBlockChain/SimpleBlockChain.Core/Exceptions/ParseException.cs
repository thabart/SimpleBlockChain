using System;

namespace SimpleBlockChain.Core.Exceptions
{
    public class ParseException : Exception
    {
        public ParseException(string code) : base(code)
        {
        }
    }
}

using System;

namespace SimpleBlockChain.Core.Exceptions
{
    public class TransactionException : Exception
    {
        public TransactionException(string code) : base(code) { }
    }
}

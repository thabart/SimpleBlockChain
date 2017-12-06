using System;

namespace SimpleBlockChain.Core.Exceptions
{
    public class WalletException : Exception
    {
        public WalletException(string code) : base(code) { }
    }
}

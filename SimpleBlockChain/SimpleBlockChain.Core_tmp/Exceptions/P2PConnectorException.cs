using System;

namespace SimpleBlockChain.Core.Exceptions
{
    public class P2PConnectorException : Exception
    {
        public P2PConnectorException(string code):  base(code) { }
    }
}

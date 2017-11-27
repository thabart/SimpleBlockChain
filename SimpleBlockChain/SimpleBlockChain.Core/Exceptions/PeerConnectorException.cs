using System;

namespace SimpleBlockChain.Core.Exceptions
{
    public class PeerConnectorException : Exception
    {
        public PeerConnectorException(string code) : base(code) { }
    }
}

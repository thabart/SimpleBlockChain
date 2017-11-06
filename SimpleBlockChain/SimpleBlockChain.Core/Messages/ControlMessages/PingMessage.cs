using System;

namespace SimpleBlockChain.Core.Messages.ControlMessages
{
    public class PingMessage : Message
    {
        public ulong Nonce { get; private set; }

        public PingMessage(ulong nonce, Networks network) : base(network)
        {
            Nonce = nonce;
        }

        protected override byte[] GetSerializedContent()
        {
            return BitConverter.GetBytes(Nonce);
        }

        protected override string GetCommandName()
        {
            return Constants.MessageNames.Ping;
        }
    }
}

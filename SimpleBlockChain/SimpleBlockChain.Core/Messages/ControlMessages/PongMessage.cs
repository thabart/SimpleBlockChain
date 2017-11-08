using System;

namespace SimpleBlockChain.Core.Messages.ControlMessages
{
    public class PongMessage : Message
    {
        public ulong Nonce { get; private set; }

        public PongMessage(ulong nonce, Networks network) : base(network)
        {
            Nonce = nonce;
        }

        protected override byte[] GetSerializedContent()
        {
            return BitConverter.GetBytes(Nonce);
        }

        public override string GetCommandName()
        {
            return Constants.MessageNames.Pong;
        }
    }
}

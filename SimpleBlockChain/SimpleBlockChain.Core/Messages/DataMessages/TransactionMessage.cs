using System;

namespace SimpleBlockChain.Core.Messages.DataMessages
{
    public class TransactionMessage : Message
    {
        public TransactionMessage(Networks network) : base(network)
        {
        }

        public override string GetCommandName()
        {
            return Constants.MessageNames.Transaction;
        }

        protected override byte[] GetSerializedContent()
        {
            return new byte[0];
        }
    }
}

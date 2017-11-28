namespace SimpleBlockChain.Core.Messages.DataMessages
{
    public class MemPoolMessage : Message
    {
        public MemPoolMessage(Networks network) : base(network)
        {
        }

        public override string GetCommandName()
        {
            return Constants.MessageNames.MemPool;
        }

        protected override byte[] GetSerializedContent()
        {
            return new byte[0];
        }
    }
}

namespace SimpleBlockChain.Core.Messages.ControlMessages
{
    public class VerackMessage : Message
    {
        public VerackMessage(Networks network) : base(network)
        {
        }

        public override string GetCommandName()
        {
            return Constants.MessageNames.Verack;
        }

        protected override byte[] GetSerializedContent()
        {
            return new byte[0];
        }
    }
}

namespace SimpleBlockChain.Core.Messages.ControlMessages
{
    public class GetAddressMessage : Message
    {
        public GetAddressMessage(Networks network) : base(network) { }

        public override string GetCommandName()
        {
            return Constants.MessageNames.GetAddr;
        }

        protected override byte[] GetSerializedContent()
        {
            return new byte[0];
        }
    }
}

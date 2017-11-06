namespace SimpleBlockChain.Core.Messages.ControlMessages
{
    public class PingMessage : Message
    {
        public ulong Nonce { get; set; }
    }
}

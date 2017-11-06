namespace SimpleBlockChain.Core.Messages
{
    public class MessageHeader
    {
        public char[] StartString { get; set; } // Length : 4
        public char[] CommandName { get; set; } // Length : 12
        public uint PayloadSize { get; set; } // Length : 4
        public char[] CheckSum { get; set; } // Length : 4
    }
}

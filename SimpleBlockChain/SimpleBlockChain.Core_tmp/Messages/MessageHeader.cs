namespace SimpleBlockChain.Core.Messages
{
    public class MessageHeader
    {
        public Networks Network { get; set; } // Length : 4

        public byte[] GetStartString()
        {
            byte[] startString = new byte[] { 0xf9, 0xbe, 0xb4, 0xd9 };
            if (Network == Networks.TestNet)
            {
                startString = new byte[] { 0x0b, 0x11, 0x09, 0x07 };
            }
            else if (Network == Networks.RegTest)
            {
                startString = new byte[] { 0xfa, 0xbf, 0xb5, 0xda };
            }

            return startString;
        }
    }
}

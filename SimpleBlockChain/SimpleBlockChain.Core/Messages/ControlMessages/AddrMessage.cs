using System;
using SimpleBlockChain.Core.Common;

namespace SimpleBlockChain.Core.Messages.ControlMessages
{
    public class AddrMessage : Message
    {
        public CompactSize CompactSize { get; private set; }

        public AddrMessage(CompactSize compactSize, Networks network) : base(network)
        {
            CompactSize = compactSize;
        }

        public override string GetCommandName()
        {
            throw new NotImplementedException();
        }

        protected override byte[] GetSerializedContent()
        {
            throw new NotImplementedException();
        }
    }
}

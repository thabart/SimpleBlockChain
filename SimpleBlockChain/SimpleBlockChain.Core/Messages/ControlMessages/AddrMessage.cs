using System;
using SimpleBlockChain.Core.Common;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Messages.ControlMessages
{
    public class AddrMessage : Message
    {
        public CompactSize CompactSize { get; private set; }
        public IEnumerable<IpAddress> IpAddresses { get; private set; }

        public AddrMessage(CompactSize compactSize, Networks network) : base(network)
        {
            CompactSize = compactSize;
            IpAddresses = new List<IpAddress>();
        }

        public override string GetCommandName()
        {
            return Constants.MessageNames.Addr;
        }

        protected override byte[] GetSerializedContent()
        {
            throw new NotImplementedException();
        }
    }
}

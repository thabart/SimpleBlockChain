using SimpleBlockChain.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Messages.ControlMessages
{
    public class AddrMessage : Message
    {
        public CompactSize CompactSize { get; private set; }
        public ICollection<IpAddress> IpAddresses { get; private set; }

        public AddrMessage(CompactSize compactSize, Networks network) : base(network)
        {
            CompactSize = compactSize;
            IpAddresses = new List<IpAddress>();
        }

        public AddrMessage(CompactSize compactSize, Networks network, List<IpAddress> ipAddresses) : base(network)
        {
            CompactSize = compactSize;
            IpAddresses = ipAddresses;
        }

        public override string GetCommandName()
        {
            return Constants.MessageNames.Addr;
        }

        protected override byte[] GetSerializedContent()
        {
            var result = new List<byte>();
            result.AddRange(CompactSize.Serialize());
            foreach (var ipAdr in IpAddresses)
            {
                result.AddRange(ipAdr.Serialize());
            }

            return result.ToArray();
        }

        public static AddrMessage Deserialize(byte[] payload, Networks network)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var kvp = CompactSize.Deserialize(payload);
            var compactSize = kvp.Key;
            var contentPayload = payload.Skip(kvp.Value);
            var nb = contentPayload.Count() / IpAddress.Size;
            var lstAddr = new List<IpAddress>();
            for (var i = 0; i < nb; i++)
            {
                var addrPayload = contentPayload.Skip(i * IpAddress.Size).Take(IpAddress.Size).ToArray();
                lstAddr.Add(IpAddress.Deserialize(addrPayload));
            }

            return new AddrMessage(compactSize, network, lstAddr);
        }
    }
}

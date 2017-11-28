using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Messages.ControlMessages
{
    public class VersionMessage : Message
    {
        public IpAddress TransmittingNode { get; private set; }
        public IpAddress ReceivingNode { get; private set; }
        public ulong Nonce { get; private set; }
        public string UserAgent { get; private set; }
        public int StartHeight { get; private set; }
        public bool Relay { get; private set; }

        public VersionMessage(IpAddress transmittingNode, IpAddress receivingNode, ulong nonce, string userAgent, int startHeight, bool relay, Networks network) : base(network)
        {
            TransmittingNode = transmittingNode;
            ReceivingNode = receivingNode;
            Nonce = nonce;
            UserAgent = userAgent;
        }

        public override string GetCommandName()
        {
            return Constants.MessageNames.Version;
        }

        public static VersionMessage Deserialize(byte[] payload, Networks network)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var transmittingService = (ServiceFlags)BitConverter.ToUInt64(payload.Skip(4).Take(8).ToArray(), 0);
            var receivingService = (ServiceFlags)BitConverter.ToUInt64(payload.Skip(20).Take(8).ToArray(), 0);
            var receivingIpv6Addr = payload.Skip(28).Take(16);
            var receivingPort = BitConverter.ToUInt16(payload.Skip(44).Take(2).ToArray(), 0);
            var transmittingIpv6Addr = payload.Skip(54).Take(16);
            var transmittingPort = BitConverter.ToUInt16(payload.Skip(70).Take(2).ToArray(), 0);
            var nonce = BitConverter.ToUInt64(payload.Skip(72).Take(8).ToArray(), 0);
            var compactSize = CompactSize.Deserialize(payload.Skip(80).ToArray());
            string userAgent = null;
            int startIndex = 80 + compactSize.Value;
            if (compactSize.Key.Size > 0)
            {
                userAgent = System.Text.Encoding.UTF8.GetString(payload.Skip(startIndex).Take(startIndex + (int)compactSize.Key.Size).ToArray());
                startIndex += (int)compactSize.Key.Size;
            }

            var startHeight = BitConverter.ToInt32(payload.Skip(startIndex).Take(4).ToArray(), 0);
            var relay = BitConverter.ToBoolean(payload.Skip(startIndex).Take(1).ToArray(), 0);
            return new VersionMessage(new IpAddress(DateTime.UtcNow, transmittingService, transmittingIpv6Addr.ToArray(), transmittingPort), 
                new IpAddress(DateTime.UtcNow, receivingService, receivingIpv6Addr.ToArray(), receivingPort),
                nonce, userAgent, startHeight, relay, network);
        }

        protected override byte[] GetSerializedContent()
        {
            var result = new List<byte>();
            var versionPayload = BitConverter.GetBytes(Constants.SupportedProtocolVersion);
            var transmittingServicePayload = BitConverter.GetBytes((UInt64)TransmittingNode.ServiceFlag);
            var timeStampPayload = BitConverter.GetBytes(DateTime.UtcNow.ToUnixTime());
            var receivingServiceFlagPayload = BitConverter.GetBytes((UInt64)ReceivingNode.ServiceFlag);
            var receivingIpV6Payload = ReceivingNode.Ipv6;
            var receivingPortPayload = BitConverter.GetBytes(ReceivingNode.Port);
            var transmittingIpV6Payload = TransmittingNode.Ipv6;
            var transmittingPortPayload = BitConverter.GetBytes(TransmittingNode.Port);
            var noncePayload = BitConverter.GetBytes(Nonce);
            var userAgentPayload = string.IsNullOrWhiteSpace(UserAgent) ? new byte[0] : System.Text.Encoding.UTF8.GetBytes(UserAgent);
            var compactSize = new CompactSize();
            compactSize.Size = (ulong)userAgentPayload.Count();
            var startHeightPayload = BitConverter.GetBytes(StartHeight);
            var relayPayload = BitConverter.GetBytes(Relay);


            result.AddRange(versionPayload);
            result.AddRange(transmittingServicePayload);
            result.AddRange(timeStampPayload);
            result.AddRange(receivingServiceFlagPayload);
            result.AddRange(receivingIpV6Payload);
            result.AddRange(receivingPortPayload);
            result.AddRange(transmittingServicePayload);
            result.AddRange(transmittingIpV6Payload);
            result.AddRange(transmittingPortPayload);
            result.AddRange(noncePayload);
            result.AddRange(compactSize.Serialize());
            result.AddRange(userAgentPayload);
            result.AddRange(startHeightPayload);
            result.AddRange(relayPayload);
            return result.ToArray();
        }
    }
}

using SimpleBlockChain.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Messages.DataMessages
{
    public class GetBlocksMessage : Message
    {
        public GetBlocksMessage(IEnumerable<IEnumerable<byte>> blockHashes, Networks network) : base(network)
        {
            BlockHashes = blockHashes;
        }

        public IEnumerable<IEnumerable<byte>> BlockHashes { get; private set; }

        public override string GetCommandName()
        {
            return Constants.MessageNames.GetBlocks;
        }

        public static GetBlocksMessage Deserialize(byte[] payload, Networks network)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var compactSize = CompactSize.Deserialize(payload.Skip(4).ToArray());
            var blockHashes = new List<IEnumerable<byte>>();
            if (compactSize.Key.Size > 0)
            {
                var startIndex = 4 + compactSize.Value;
                for(var i = 0; i < (int)compactSize.Key.Size; i++)
                {
                    blockHashes.Add(payload.Skip(startIndex).Take(32));
                    startIndex += 32;
                }
            }

            return new GetBlocksMessage(blockHashes, network);
        }

        protected override byte[] GetSerializedContent()
        {
            var result = new List<byte>();
            var versionPayload = BitConverter.GetBytes(Constants.SupportedProtocolVersion);
            var compactSize = new CompactSize();
            compactSize.Size = (ulong)BlockHashes.Count();

            result.AddRange(versionPayload);
            result.AddRange(compactSize.Serialize());
            foreach(var blockHash in BlockHashes)
            {
                result.AddRange(blockHash);
            }

            return result.ToArray();
        }
    }
}

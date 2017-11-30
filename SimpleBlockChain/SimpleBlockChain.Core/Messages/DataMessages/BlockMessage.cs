using SimpleBlockChain.Core.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Messages.DataMessages
{
    public class BlockMessage : Message
    {
        public BlockMessage(Block block, Networks network) : base(network)
        {
            Block = block;
        }

        public Block Block { get; private set; }

        public static BlockMessage Deserialize(IEnumerable<byte> payload, Networks network)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            return new BlockMessage(Block.Deserialize(payload), network);
        }

        public override string GetCommandName()
        {
            return Constants.MessageNames.Block;
        }

        protected override byte[] GetSerializedContent()
        {
            return Block.Serialize().ToArray();
        }
    }
}

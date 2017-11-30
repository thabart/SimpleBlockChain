using SimpleBlockChain.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Messages.DataMessages
{
    public class NotFoundMessage : Message
    {
        public NotFoundMessage(IList<Inventory> inventories, Networks network) : base(network)
        {
            Inventories = inventories;
        }

        public IList<Inventory> Inventories { get; private set; }

        public override string GetCommandName()
        {
            return Constants.MessageNames.NotFound;
        }

        public static NotFoundMessage Deserialize(byte[] payload, Networks network)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var compactSize = CompactSize.Deserialize(payload);
            var inventories = new List<Inventory>();
            if (compactSize.Key.Size > 0)
            {
                var content = payload.Skip(compactSize.Value);
                var size = (int)compactSize.Key.Size;
                for (var i = 0; i < size; i++)
                {
                    var inventoryPayload = content.Skip(i * Inventory.Size).Take(Inventory.Size);
                    inventories.Add(Inventory.Deserialize(inventoryPayload));
                }
            }

            return new NotFoundMessage(inventories, network);
        }

        protected override byte[] GetSerializedContent()
        {
            var size = Inventories.Count();
            var compactSize = new CompactSize();
            compactSize.Size = (ulong)size;
            var result = new List<byte>();
            result.AddRange(compactSize.Serialize());
            foreach (var inventory in Inventories)
            {
                result.AddRange(inventory.Serialize());
            }

            return result.ToArray();
        }
    }
}

using SimpleBlockChain.Core.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SimpleBlockChain.Core.Messages.DataMessages
{
    public class Inventory
    {
        public const int Size = 36;

        public Inventory(InventoryTypes type)
        {
            Type = type;
        }

        public Inventory(InventoryTypes type, IEnumerable<byte> hash)
        {
            Type = type;
        }

        public InventoryTypes Type { get; private set; }
        public IEnumerable<byte> Hash { get; private set; }

        public static Inventory Deserialize(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var type = (InventoryTypes)BitConverter.ToUInt32(payload.Take(4).ToArray(), 0);
            var hash = payload.Skip(4);
            return new Inventory(type)
            {
                Hash = hash
            };
        }

        public IEnumerable<byte> Serialize()
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes((UInt32)Type));
            result.AddRange(Hash);
            return result;
        }
    }

    public class InventoryMessage : Message
    {
        public InventoryMessage(IList<Inventory> inventories, Networks network) : base(network)
        {
            Inventories = inventories;
        }

        public IList<Inventory> Inventories { get; private set; }

        public override string GetCommandName()
        {
            return Constants.MessageNames.Inventory;
        }

        public static InventoryMessage Deserialize(byte[] payload, Networks network)
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
                for(var i = 0; i < size; i++)
                {
                    var inventoryPayload = content.Skip(i * Inventory.Size).Take(Inventory.Size);
                    inventories.Add(Inventory.Deserialize(inventoryPayload));
                }
            }

            return new InventoryMessage(inventories, network);
        }

        protected override byte[] GetSerializedContent()
        {
            var size = Inventories.Count();
            var compactSize = new CompactSize();
            compactSize.Size = (ulong)size;
            var result = new List<byte>();
            result.AddRange(compactSize.Serialize());
            foreach(var inventory in Inventories)
            {
                result.AddRange(inventory.Serialize());
            }

            return result.ToArray();
        }
    }
}

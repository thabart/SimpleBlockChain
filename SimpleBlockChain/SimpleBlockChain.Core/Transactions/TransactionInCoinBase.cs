using SimpleBlockChain.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Transactions
{
    public class TransactionInCoinbase : BaseTransactionIn
    {
        private static UInt32 DEFAULT_INDEX = 0xffffffff;

        public TransactionInCoinbase(uint height, byte[] coinBaseScript, uint sequence)
        {
            Height = height;
            CoinBaseScript = coinBaseScript;
            Sequence = sequence;
        }

        public uint Height { get; private set; }
        public byte[] CoinBaseScript { get; private set; }
        public uint Sequence { get; private set; }

        public override byte[] Serialize()
        {
            var result = new List<byte>();
            var hashPayload = new byte[32];
            Array.Clear(hashPayload, 0, 32);
            var compactSize = new CompactSize();
            compactSize.Size = (ulong)CoinBaseScript.Count();
            result.AddRange(hashPayload);
            result.AddRange(BitConverter.GetBytes(DEFAULT_INDEX));
            result.AddRange(compactSize.Serialize());
            result.AddRange(BitConverter.GetBytes(Height));
            result.AddRange(BitConverter.GetBytes(Sequence));
            return result.ToArray();
        }

        public static KeyValuePair<TransactionInCoinbase, int> Deserialize(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            int startIndex = 36;
            var compactSize = CompactSize.Deserialize(payload.Skip(startIndex).ToArray());
            startIndex += compactSize.Value;
            var height = BitConverter.ToUInt32(payload.Skip(startIndex).Take(4).ToArray(), 0);
            startIndex += 4;
            var coinBaseScript = payload.Skip(startIndex).Take((int)compactSize.Key.Size).ToArray();
            startIndex += (int)compactSize.Key.Size;
            var sequence = BitConverter.ToUInt32(payload.Skip(startIndex).Take(4).ToArray(), 4);
            startIndex += 4;
            return new KeyValuePair<TransactionInCoinbase, int>(new TransactionInCoinbase(height, coinBaseScript, sequence), startIndex);
        }
    }
}

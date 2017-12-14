using SimpleBlockChain.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Transactions
{
    public class TransactionInNoneCoinbase : BaseTransactionIn
    {
        public Outpoint Outpoint { get; private set; } // outpoint
        public IEnumerable<byte> SignatureScript { get; private set; } // signature script
        public UInt32 Sequence { get; private set; }

        public TransactionInNoneCoinbase(Outpoint outpoint, IEnumerable<byte> signatureScript, UInt32 sequence)
        {
            Outpoint = outpoint;
            SignatureScript = signatureScript;
            Sequence = sequence;
        }

        public override byte[] Serialize()
        {
            var result = new List<byte>();
            result.AddRange(Outpoint.Serialize());
            var compactSize = new CompactSize();
            compactSize.Size = 0;
            if (SignatureScript != null)
            {
                compactSize.Size = (ulong)SignatureScript.Count();
                result.AddRange(compactSize.Serialize());
                result.AddRange(SignatureScript);
            }
            else
            {
                result.AddRange(compactSize.Serialize());
            }

            result.AddRange(BitConverter.GetBytes(Sequence));
            return result.ToArray();
        }

        public static KeyValuePair<TransactionInNoneCoinbase, int> Deserialize(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var outpoint = Outpoint.Deserialize(payload.ToArray());
            int startIndex = Outpoint.SIZE;
            var compactSize = CompactSize.Deserialize(payload.Skip(startIndex).ToArray());
            startIndex += compactSize.Value;
            IEnumerable<byte> signatureScripts = new List<byte>();
            if (compactSize.Key.Size > 0)
            {
                signatureScripts = payload.Skip(startIndex).Take((int)compactSize.Key.Size);
                startIndex += (int)compactSize.Key.Size;
            }

            var sequence = BitConverter.ToUInt32(payload.Skip(startIndex).Take(4).ToArray(), 0);
            startIndex += 4;
            return new KeyValuePair<TransactionInNoneCoinbase, int>(new TransactionInNoneCoinbase(outpoint, signatureScripts, sequence), startIndex);
        }
    }
}

using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Transactions
{
    public class Transaction
    {
        private const UInt32 CURRENT_VERSION = 2;
        private const int MAX_STANDARD_VERSION = 2;
        public uint Version { get; set; } // version
        public uint LockTime { get; set; } // lock_time
        public List<TransactionIn> TransactionIn { get; private set; } // tx_in
        public List<TransactionOut> TransactionOut { get; private set; } // tx_out

        public Transaction()
        {
            TransactionOut = new List<TransactionOut>();
            TransactionIn = new List<TransactionIn>();
        }

        public IEnumerable<byte> Serialize()
        {
            // https://bitcoin.org/en/developer-reference#raw-transaction-format
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(CURRENT_VERSION));
            var inputCompactSize = new CompactSize();
            inputCompactSize.Size = (ulong)TransactionIn.Count();
            result.AddRange(inputCompactSize.Serialize());
            foreach (var input in TransactionIn)
            {
                result.AddRange(input.Serialize());
            }

            var outputCompactSize = new CompactSize();
            outputCompactSize.Size = (ulong)TransactionOut.Count();
            result.AddRange(outputCompactSize.Serialize());
            foreach(var output in TransactionOut)
            {
                result.AddRange(output.Serialize());
            }

            var dateTime = DateTime.UtcNow;
            result.AddRange(BitConverter.GetBytes(dateTime.ToUnixTime()));
            return result;
        }

        public static Transaction Deserialize(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            int currentStartIndex = 0;
            var result = new Transaction();
            result.Version = BitConverter.ToUInt32(payload.Take(4).ToArray(), 0);
            currentStartIndex = 4;
            var transactionInCompactSize = CompactSize.Deserialize(payload.Skip(currentStartIndex).ToArray());
            if (transactionInCompactSize.Key.Size > 0)
            {

            }

            currentStartIndex += transactionInCompactSize.Value;
            var transactionOutputCompactSize = CompactSize.Deserialize(payload.Skip(currentStartIndex).ToArray());
            currentStartIndex += transactionOutputCompactSize.Value;
            if (transactionOutputCompactSize.Key.Size > 0)
            {
                for (var i = 0; i < (int)transactionOutputCompactSize.Key.Size; i++)
                {
                    var kvp = Transactions.TransactionOut.Deserialize(payload.Skip(currentStartIndex).ToArray());
                    result.TransactionOut.Add(kvp.Key);
                    currentStartIndex += kvp.Value;
                }
            }

            result.LockTime = BitConverter.ToUInt32(payload.Skip(payload.Count() - 4).Take(4).ToArray(), 0);
            return result;
        }
    }
}

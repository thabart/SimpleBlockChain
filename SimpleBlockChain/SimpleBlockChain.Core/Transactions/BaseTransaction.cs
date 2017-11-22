using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SimpleBlockChain.Core.Transactions
{
    public enum TransactionTypes
    {
        Coinbase,
        NoneCoinbase
    }

    public abstract class BaseTransaction : IComparable
    {
        private const UInt32 CURRENT_VERSION = 2;
        private const int MAX_STANDARD_VERSION = 2;
        public uint Version { get; set; } // version
        public uint LockTime { get; set; } // lock_time
        public List<BaseTransactionIn> TransactionIn { get; protected set; } // tx_in
        public List<TransactionOut> TransactionOut { get; protected set; } // tx_out

        public BaseTransaction() : this(CURRENT_VERSION, DateTime.UtcNow.ToUnixTimeUInt32()) { }

        public BaseTransaction(uint version, uint lockTime)
        {
            Version = version;
            LockTime = lockTime;
            TransactionIn = new List<BaseTransactionIn>();
            TransactionOut = new List<TransactionOut>();
        }

        public static KeyValuePair<BaseTransaction, int> Deserialize(IEnumerable<byte> payload, TransactionTypes type)
        {
            BaseTransaction result = null;
            switch(type)
            {
                case TransactionTypes.Coinbase:
                    result = new CoinbaseTransaction();
                    break;
                case TransactionTypes.NoneCoinbase:
                    result = new NoneCoinbaseTransaction();
                    break;
            }


            int currentStartIndex = 0;
            result.Version = BitConverter.ToUInt32(payload.Take(4).ToArray(), 0);
            currentStartIndex = 4;
            var transactionInCompactSize = CompactSize.Deserialize(payload.Skip(currentStartIndex).ToArray());
            currentStartIndex += transactionInCompactSize.Value;
            if (transactionInCompactSize.Key.Size > 0)
            {
                var kvp = result.DeserializeInputs(payload.Skip(currentStartIndex), (int)transactionInCompactSize.Key.Size);
                result.TransactionIn = kvp.Key;
                currentStartIndex += kvp.Value;
            }

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

            result.LockTime = BitConverter.ToUInt32(payload.Skip(currentStartIndex).Take(4).ToArray(), 0);
            currentStartIndex += 4;
            return new KeyValuePair<BaseTransaction, int>(result, currentStartIndex);
        }

        public IEnumerable<byte> GetTxId()
        {
            var payload = Serialize();
            var mySHA256 = SHA256Managed.Create();
            return mySHA256.ComputeHash(mySHA256.ComputeHash(payload.ToArray()));
        }

        public IEnumerable<byte> Serialize()
        {
            // https://bitcoin.org/en/developer-reference#raw-transaction-format
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(Version));
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
            foreach (var output in TransactionOut)
            {
                result.AddRange(output.Serialize());
            }
            
            result.AddRange(BitConverter.GetBytes(LockTime));
            return result;
        }

        public abstract KeyValuePair<List<BaseTransactionIn>, int> DeserializeInputs(IEnumerable<byte> payload, int size);

        public void Check()
        {
            // https://bitcoin.org/en/developer-guide#block-chain-overview
            // TODO : Check the transaction is correct.
            // UNSPENT TRANSACTION
            // SPENT TRANSACTION.
        }

        public int CompareTo(object obj)
        {
            // < 0 : this < obj
            // == 0 : this == obj
            // > 0 : this > obj
            if (obj == null)
            {
                return -1;
            }

            var baseTrans = obj as BaseTransaction;
            if (baseTrans == null)
            {
                return -1;
            }

            return CompareTo(baseTrans);
        }

        public abstract int CompareTo(BaseTransaction obj);
    }
}

using SimpleBlockChain.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SimpleBlockChain.Core.Transactions
{
    public enum TransactionCategories
    {
        SmartContract = 0x00,
        Monetary = 0x01
    }

    public abstract class BaseTransaction : IComparable
    {
        private const UInt32 CURRENT_VERSION = 2;
        private const int MAX_STANDARD_VERSION = 2;
        public uint Version { get; set; } // version
        public uint LockTime { get; set; } // lock_time
        public TransactionCategories Category { get; set; } // category.

        public BaseTransaction() : this(CURRENT_VERSION, DateTime.UtcNow.ToUnixTimeUInt32(), TransactionCategories.Monetary) { }

        public BaseTransaction(uint version, uint lockTime, TransactionCategories category)
        {
            Version = version;
            LockTime = lockTime;
            Category = category;
        }

        public static KeyValuePair<BaseTransaction, int> Deserialize(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var type = (TransactionCategories)payload.ElementAt(4);
            switch(type)
            {
                case TransactionCategories.Monetary:
                    return BcBaseTransaction.Deserialize(payload);
                case TransactionCategories.SmartContract:
                    return SmartContractTransaction.Deserialize(payload);
            }

            return default(KeyValuePair<BaseTransaction, int>);
        }

        public IEnumerable<byte> GetTxId()
        {
            var payload = Serialize();
            var mySHA256 = SHA256.Create();
            return mySHA256.ComputeHash(mySHA256.ComputeHash(payload.ToArray()));
        }

        public abstract IEnumerable<byte> Serialize();

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

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var d = obj as BaseTransaction;
            if (d == null)
            {
                return false;
            }

            return GetTxId().SequenceEqual(d.GetTxId());
        }
    }
}

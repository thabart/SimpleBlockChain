using HashLib;
using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Encoding;
using SimpleBlockChain.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Transactions
{
    public class SmartContractTransaction  : BaseTransaction // https://github.com/ethereum/wiki/wiki/JSON-RPC#eth_sendtransaction
    {
        public IEnumerable<byte> From { get; set; }
        public IEnumerable<byte> To { get; set; }
        public IEnumerable<byte> Data { get; set; }
        public double Gas { get; set; }
        public double GasPrice { get; set; }
        public double Value { get; set; }
        public int Nonce { get; set; }


        public SmartContractTransaction() { }

        public SmartContractTransaction(uint version, uint lockTime) : base(version, lockTime, TransactionCategories.SmartContract) { }

        public override IEnumerable<byte> Serialize()
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(Version));
            result.Add((byte)TransactionCategories.SmartContract);
            var fromCompactSize = new CompactSize();
            var toCompactSize = new CompactSize();
            var dataCompactSize = new CompactSize();
            fromCompactSize.Size = (ulong)From.Count();
            toCompactSize.Size = To == null ? 0 : (ulong)To.Count();
            dataCompactSize.Size = Data == null ? 0 : (ulong)Data.Count();
            result.AddRange(fromCompactSize.Serialize()); // FROM.
            result.AddRange(From);
            result.AddRange(toCompactSize.Serialize()); // TO
            result.AddRange(To);
            result.AddRange(dataCompactSize.Serialize()); // DATA
            result.AddRange(Data);
            result.AddRange(BitConverter.GetBytes(Gas));
            result.AddRange(BitConverter.GetBytes(GasPrice));
            result.AddRange(BitConverter.GetBytes(Value));
            result.AddRange(BitConverter.GetBytes(Nonce));
            result.AddRange(BitConverter.GetBytes(LockTime));
            return null;
        }

        public static KeyValuePair<BaseTransaction, int> Deserialize(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
            SmartContractTransaction result = null;
            int currentStartIndex = 0;
            result.Version = BitConverter.ToUInt32(payload.Take(4).ToArray(), 0);
            result.Category = (TransactionCategories)payload.ElementAt(4);
            currentStartIndex = 5;
            var fromCompactSize = CompactSize.Deserialize(payload.Skip(currentStartIndex).ToArray()); // FROM
            currentStartIndex += fromCompactSize.Value;
            if (fromCompactSize.Key.Size > 0)
            {
                result.From = payload.Skip(currentStartIndex).Take((int)fromCompactSize.Key.Size);
                currentStartIndex += (int)fromCompactSize.Key.Size;
            }

            var toCompactSize = CompactSize.Deserialize(payload.Skip(currentStartIndex).ToArray()); // TO
            currentStartIndex += toCompactSize.Value;
            if (toCompactSize.Key.Size > 0)
            {
                result.To = payload.Skip(currentStartIndex).Take((int)toCompactSize.Key.Size);
                currentStartIndex += (int)toCompactSize.Key.Size;
            }

            var dataCompactSize = CompactSize.Deserialize(payload.Skip(currentStartIndex).ToArray()); // DATA
            currentStartIndex += dataCompactSize.Value;
            if (dataCompactSize.Key.Size > 0)
            {
                result.To = payload.Skip(currentStartIndex).Take((int)dataCompactSize.Key.Size);
                currentStartIndex += (int)dataCompactSize.Key.Size;
            }

            result.Gas = BitConverter.ToDouble(payload.Skip(currentStartIndex).Take(8).ToArray(), 0);
            currentStartIndex += 8;
            result.GasPrice = BitConverter.ToDouble(payload.Skip(currentStartIndex).Take(8).ToArray(), 0);
            currentStartIndex += 8;
            result.Value = BitConverter.ToDouble(payload.Skip(currentStartIndex).Take(8).ToArray(), 0);
            currentStartIndex += 8;
            result.Nonce = BitConverter.ToInt32(payload.Skip(currentStartIndex).Take(4).ToArray(), 0);
            currentStartIndex += 4;
            result.LockTime = BitConverter.ToUInt32(payload.Skip(currentStartIndex).Take(4).ToArray(), 0);
            currentStartIndex += 4;
            return new KeyValuePair<BaseTransaction, int>(result, currentStartIndex);
        }

        public IEnumerable<byte> GetSmartContractAddress()
        {
            List<string> result = new List<string>
            {
                From.ToHexString(),
                Nonce.ToString()
            };
            var encoded = RlpEncoding.Encode(result);
            var hash = HashFactory.Crypto.SHA3.CreateKeccak256();
            return hash.ComputeBytes(encoded).GetBytes().Take(20);
        }

        public override int CompareToImpl(BaseTransaction obj)
        {
            return -1;
        }
    }
}

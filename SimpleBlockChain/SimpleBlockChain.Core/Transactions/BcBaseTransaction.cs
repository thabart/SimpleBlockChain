using Newtonsoft.Json.Linq;
using SimpleBlockChain.Core.Aggregates;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Transactions
{
    public enum TransactionTypes
    {
        Coinbase,
        NoneCoinbase
    }

    public abstract class BcBaseTransaction : BaseTransaction
    {
        public List<BaseTransactionIn> TransactionIn { get; protected set; } // tx_in
        public List<TransactionOut> TransactionOut { get; protected set; } // tx_out

        public BcBaseTransaction() { }

        public BcBaseTransaction(uint version, uint lockTime) : base(version, lockTime)
        {
            TransactionIn = new List<BaseTransactionIn>();
            TransactionOut = new List<TransactionOut>();
        }

        public static KeyValuePair<BcBaseTransaction, int> Deserialize(IEnumerable<byte> payload, TransactionTypes type)
        {
            BcBaseTransaction result = null;
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
                    var kvp = result.DeserializeOutput(payload.Skip(currentStartIndex).ToArray());
                    result.TransactionOut.Add(kvp.Key);
                    currentStartIndex += kvp.Value;
                }
            }

            result.LockTime = BitConverter.ToUInt32(payload.Skip(currentStartIndex).Take(4).ToArray(), 0);
            currentStartIndex += 4;
            return new KeyValuePair<BcBaseTransaction, int>(result, currentStartIndex);
        }

        public override IEnumerable<byte> Serialize()
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
        public abstract KeyValuePair<TransactionOut, int> DeserializeOutput(IEnumerable<byte> payload);
        
        public TransactionOut GetTransactionOut(WalletAggregateAddress walletAddr)
        {
            if (walletAddr == null)
            {
                throw new ArgumentNullException(nameof(walletAddr));
            }

            if (walletAddr.Key == null)
            {
                throw new ArgumentNullException(nameof(walletAddr.Key));
            }

            if (TransactionOut == null || !TransactionOut.Any())
            {
                return null;
            }

            var interpreter = new ScriptInterpreter();
            var scriptBuilder = new ScriptBuilder();
            var secondScript = scriptBuilder.New()
                .AddToStack(walletAddr.Key.GetSignature())
                .AddToStack(walletAddr.Key.GetPublicKey())
                .Build();
            foreach(var transactionOut in TransactionOut)
            {
                if (interpreter.Check(transactionOut.Script, secondScript))
                {
                    return transactionOut;
                }
            }

            return null;
        }
        
        public TransactionOut GetTransactionOut(string encodedBcAddr)
        {
            if (string.IsNullOrWhiteSpace(encodedBcAddr))
            {
                throw new ArgumentNullException(nameof(encodedBcAddr));
            }

            var bcAddr = BlockChainAddress.Deserialize(encodedBcAddr);
            var publicKeyHash = bcAddr.PublicKeyHash;
            return GetTransactionOut(publicKeyHash);
        }

        public TransactionOut GetTransactionOut(IEnumerable<byte> publicKeyHash)
        {
            if (publicKeyHash == null)
            {
                throw new ArgumentNullException(nameof(publicKeyHash));
            }


            foreach (var transactionOut in TransactionOut)
            {
                var script = transactionOut.Script;
                if (script.ContainsPublicKeyHash(publicKeyHash))
                {
                    return transactionOut;
                }
            }

            return null;

        }

        public bool CanSpend(string adrHash)
        {
            return GetTransactionOut(adrHash) != null;
        }

        public abstract int CompareTo(BcBaseTransaction obj);

        public JObject SerializeJson()
        {
            var result = new JObject();
            var content = new JObject();
            var serialized = Serialize();
            content.Add("size", serialized.Count());
            /*
            content.Add("fee", 0.1);
            content.Add("modifiedfee", 0.1);
            content.Add("time", null);
            content.Add("height", null);
            content.Add("startingpriority", null);
            content.Add("currentpriority", null);
            content.Add("currentpriority", null);
            */
            result.Add(GetTxId().ToHexString(), content);
            return result;
        }

        public override int GetHashCode()
        {
            return Version.GetHashCode() * LockTime.GetHashCode();
        }
    }
}

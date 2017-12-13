using Newtonsoft.Json.Linq;
using SimpleBlockChain.Core.Aggregates;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Scripts;
using SimpleBlockChain.Core.Stores;
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

        public long GetFee()
        {
            var outputValue = TransactionOut.Sum(t => t.Value); // TRANSACTION FEE + REWARD.
            var inputValue = TransactionIn.Sum(t => t.GetValue());
            var leftValue = inputValue - outputValue;
            var result = ((double)(Serialize().Count() / (double)1000) * Constants.DEFAULT_MIN_TX_REWARD) + leftValue;
            return (long)result;
        }

        public IEnumerable<byte> GetTxId()
        {
            var payload = Serialize();
            var mySHA256 = SHA256.Create();
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

        public TransactionOut GetTransactionIn(string encodedBcAddr)
        {
            if (string.IsNullOrWhiteSpace(encodedBcAddr))
            {
                throw new ArgumentNullException(nameof(encodedBcAddr));
            }

            var bcAddr = BlockChainAddress.Deserialize(encodedBcAddr);
            var publicKeyHash = bcAddr.PublicKeyHash;
            var blockChain = BlockChainStore.Instance().GetBlockChain();
            foreach (var txIn in TransactionIn)
            {
                var nCbtxIn = txIn as TransactionInNoneCoinbase;
                if (nCbtxIn == null || nCbtxIn.Outpoint == null)
                {
                    continue;
                }

                var previousTx = blockChain.GetTransaction(nCbtxIn.Outpoint.Hash);
                if (previousTx == null || previousTx.TransactionOut == null)
                {
                    continue;
                }

                var previousTxOut = previousTx.TransactionOut.ElementAtOrDefault((int)nCbtxIn.Outpoint.Index);
                if (previousTxOut == null || previousTxOut.Script == null || !previousTxOut.Script.ContainsPublicKeyHash(publicKeyHash))
                {
                    continue;
                }

                return previousTxOut;
            }

            return null;
        }

        public long CalculateBalance(string encodedBcAddr)
        {
            if (string.IsNullOrWhiteSpace(encodedBcAddr))
            {
                throw new ArgumentNullException(nameof(encodedBcAddr));
            }

            var txIn = GetTransactionIn(encodedBcAddr);
            var txOut = GetTransactionOut(encodedBcAddr);
            if (txIn != null)
            {
                if (txOut == null)
                {
                    return 0;
                }

                return -(txIn.Value - txOut.Value);
            }            

            if (txOut == null)
            {
                return 0;
            }

            return txOut.Value;
        }

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

            var interpreter = new Interpreter();
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

        public void Check()
        {            
            var isCoinBaseTransaction = this is CoinbaseTransaction; // https://bitcoin.org/en/developer-guide#block-chain-overview
            if (!isCoinBaseTransaction && (TransactionIn == null || !TransactionIn.Any()))
            {
                throw new ValidationException(ErrorCodes.NoTransactionIn);
            }

            var blockChain = BlockChainStore.Instance().GetBlockChain();
            if (!isCoinBaseTransaction)
            {
                long totalOutput = 0;
                foreach (var txIn in TransactionIn)
                {
                    var noneCoinBaseTxIn = txIn as TransactionInNoneCoinbase; // Check TRANSACTION EXISTS.
                    var previousTxId = noneCoinBaseTxIn.Outpoint.Hash;
                    var previousIndex = noneCoinBaseTxIn.Outpoint.Index;
                    var previousTransaction = blockChain.GetUnspentTransaction(previousTxId, previousIndex);
                    if (previousTransaction == null)
                    {
                        throw new ValidationException(ErrorCodes.ReferencedTransactionNotValid);
                    }

                    var previousTxOut = previousTransaction.TransactionOut.ElementAt((int)previousIndex); // Check SCRIPT.
                    var sigScript = Script.Deserialize(noneCoinBaseTxIn.SignatureScript);
                    var pkScript = previousTxOut.Script;
                    var interpreter = new Interpreter();
                    if (!interpreter.Check(sigScript, pkScript))
                    {
                        throw new ValidationException(ErrorCodes.TransactionSignatureNotCorrect);
                    }

                    totalOutput += previousTxOut.Value;
                }

                var sumOutput = TransactionOut.Sum(t => t.Value);
                if (sumOutput > totalOutput)
                {
                    throw new ValidationException(ErrorCodes.TransactionOutputExceedInput);
                }
            }
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

        public abstract int CompareTo(BaseTransaction obj);

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
    }
}

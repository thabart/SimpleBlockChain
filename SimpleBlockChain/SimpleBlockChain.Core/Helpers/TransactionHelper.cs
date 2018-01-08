using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Helpers
{
    public interface ITransactionHelper
    {
        long GetMinFee();
        long GetReward(BaseTransaction transaction);
        long CalculateBalance(BcBaseTransaction transaction, IEnumerable<BlockChainAddress> bcAddrs, Networks network);
        long GetFee(BaseTransaction transaction, Networks network);
        long GetFee(BcBaseTransaction transaction, Networks network);
        long GetFee(SmartContractTransaction transaction, Networks network);
        TransactionOut GetTransactionIn(BcBaseTransaction transaction, Networks network);
        TransactionOut GetTransactionIn(BcBaseTransaction transaction, IEnumerable<BlockChainAddress> bcAddrs, Networks network);
    }

    internal class TransactionHelper : ITransactionHelper
    {
        private const double DEFAULT_TX_SIZE = 500;
        private readonly IBlockChainStore _blockChainStore;

        public TransactionHelper(IBlockChainStore blockChainStore)
        {
            _blockChainStore = blockChainStore;
        }

        public long GetMinFee()
        {
            return (long)Math.Ceiling(((DEFAULT_TX_SIZE / (double)1000) * Constants.DEFAULT_MIN_TX_FEE));
        }

        public long GetReward(BaseTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            return (long)Math.Ceiling(((transaction.Serialize().Count() / (double)1000) * Constants.DEFAULT_MIN_TX_FEE));
        }

        public long CalculateBalance(BcBaseTransaction transaction, IEnumerable<BlockChainAddress> bcAddrs, Networks network)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            if (bcAddrs == null)
            {
                throw new ArgumentNullException(nameof(bcAddrs));
            }
            
            var txIn = GetTransactionIn(transaction, bcAddrs, network) as TransactionOut;
            var publicKeyHashes = bcAddrs.Select(bcAddr => bcAddr.PublicKeyHash);
            TransactionOut txOut = null;
            foreach (var transactionOut in transaction.TransactionOut)
            {
                var script = transactionOut.Script;
                if (publicKeyHashes.Any(publicKeyHash => script.ContainsPublicKeyHash(publicKeyHash)))
                {
                    txOut = transactionOut as TransactionOut;
                }
            }
            
            var noneCoinBaseTransaction = transaction as NoneCoinbaseTransaction;
            if (noneCoinBaseTransaction != null)
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

        public long GetFee(BaseTransaction transaction, Networks network)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var tx = transaction as BcBaseTransaction;
            if (tx != null)
            {
                return GetFee(tx, network);
            }

            return GetFee(transaction as SmartContractTransaction, network);
        }

        public long GetFee(SmartContractTransaction transaction, Networks network)
        {
            return 1;
        }

        public long GetFee(BcBaseTransaction transaction, Networks network)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var outputValue = transaction.TransactionOut.Where(t => t is TransactionOut).Sum(t => (t as TransactionOut).Value); // TRANSACTION FEE + REWARD.
            long inputValue = 0;
            var txIn = GetTransactionIn(transaction, network) as TransactionOut;
            if (txIn != null)
            {
                inputValue = txIn.Value;
            }

            var leftValue = inputValue - outputValue;
            var reward = GetReward(transaction);
            var result = reward + leftValue;
            return result;
        }

        public TransactionOut GetTransactionIn(BcBaseTransaction transaction, Networks network)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var blockChain = _blockChainStore.GetBlockChain();
            foreach (var txIn in transaction.TransactionIn)
            {
                var nCbtxIn = txIn as TransactionInNoneCoinbase;
                if (nCbtxIn == null || nCbtxIn.Outpoint == null)
                {
                    continue;
                }

                var previousTx = blockChain.GetTransaction(nCbtxIn.Outpoint.Hash);
                TransactionOut previousTxOut = null;
                if (previousTx == null || previousTx.TransactionOut == null)
                {
                    previousTxOut = MemoryPool.Instance().GetUnspentTransaction(nCbtxIn.Outpoint.Hash, nCbtxIn.Outpoint.Index);
                    if (previousTxOut == null)
                    {
                        continue;
                    }
                }
                else
                {
                    previousTxOut = previousTx.TransactionOut.ElementAtOrDefault((int)nCbtxIn.Outpoint.Index);
                    if (previousTxOut == null || previousTxOut.Script == null)
                    {
                        continue;
                    }
                }


                return previousTxOut;
            }

            return null;
        }

        public TransactionOut GetTransactionIn(BcBaseTransaction transaction, IEnumerable<BlockChainAddress> bcAddrs, Networks network)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            if (bcAddrs == null)
            {
                throw new ArgumentNullException(nameof(bcAddrs));
            }

            var publicKeyHashes = bcAddrs.Select(bcAddr => bcAddr.PublicKeyHash);
            var blockChain = _blockChainStore.GetBlockChain();
            var memPool = MemoryPool.Instance();
            foreach (var txIn in transaction.TransactionIn)
            {
                var nCbtxIn = txIn as TransactionInNoneCoinbase;
                if (nCbtxIn == null || nCbtxIn.Outpoint == null)
                {
                    continue;
                }

                var previousTx = blockChain.GetTransaction(nCbtxIn.Outpoint.Hash);
                TransactionOut previousTxOut = null;
                if (previousTx == null || previousTx.TransactionOut == null)
                {
                    previousTxOut = memPool.GetUnspentTransaction(nCbtxIn.Outpoint.Hash, nCbtxIn.Outpoint.Index);
                    if (previousTxOut == null)
                    {
                        continue;
                    }
                }
                else
                {
                    previousTxOut = previousTx.TransactionOut.ElementAtOrDefault((int)nCbtxIn.Outpoint.Index);
                }
                
                if (previousTxOut == null || previousTxOut.Script == null || publicKeyHashes.All(publicKeyHash => !previousTxOut.Script.ContainsPublicKeyHash(publicKeyHash)))
                {
                    continue;
                }

                return previousTxOut;
            }

            return null;
        }
    }
}

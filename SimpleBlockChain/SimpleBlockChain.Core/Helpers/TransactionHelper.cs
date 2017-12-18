using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Linq;

namespace SimpleBlockChain.Core.Helpers
{
    public interface ITransactionHelper
    {
        long CalculateBalance(BaseTransaction transaction, string encodedBcAddr, Networks network);
        long GetFee(BaseTransaction transaction, Networks network);
        TransactionOut GetTransactionIn(BaseTransaction transaction, string encodedBcAddr, Networks network);
        long GetMinFee();
        long GetReward(BaseTransaction transaction);
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

        public long CalculateBalance(BaseTransaction transaction, string encodedBcAddr, Networks network)
        {
            if (string.IsNullOrWhiteSpace(encodedBcAddr))
            {
                throw new ArgumentNullException(nameof(encodedBcAddr));
            }

            if (!transaction.CanSpend(encodedBcAddr))
            {
                return 0;
            }

            var bcAdr = BlockChainAddress.Deserialize(encodedBcAddr);
            var noneCoinBaseTransaction = transaction as NoneCoinbaseTransaction;
            if (noneCoinBaseTransaction != null)
            {
                var txOut = transaction.GetTransactionOut(encodedBcAddr);
                if (txOut == null)
                {
                    return 0;
                }

                return txOut.Value;
            }

            var coinBaseTransaction = transaction as CoinbaseTransaction;
            return coinBaseTransaction.TransactionOut.First().Value;
        }

        public long GetFee(BaseTransaction transaction, Networks network)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var outputValue = transaction.TransactionOut.Sum(t => t.Value); // TRANSACTION FEE + REWARD.
            long inputValue = 0;
            var txIn = GetTransactionIn(transaction, network);
            if (txIn != null)
            {
                inputValue = txIn.Value;
            }

            var leftValue = inputValue - outputValue;
            var reward = GetReward(transaction);
            var result = reward + leftValue;
            return result;
        }

        public TransactionOut GetTransactionIn(BaseTransaction transaction, Networks network)
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
                if (previousTx == null || previousTx.TransactionOut == null)
                {
                    continue;
                }

                var previousTxOut = previousTx.TransactionOut.ElementAtOrDefault((int)nCbtxIn.Outpoint.Index);
                if (previousTxOut == null || previousTxOut.Script == null)
                {
                    continue;
                }

                return previousTxOut;
            }

            return null;
        }

        public TransactionOut GetTransactionIn(BaseTransaction transaction, string encodedBcAddr, Networks network)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            if (string.IsNullOrWhiteSpace(encodedBcAddr))
            {
                throw new ArgumentNullException(nameof(encodedBcAddr));
            }

            var bcAddr = BlockChainAddress.Deserialize(encodedBcAddr);
            var publicKeyHash = bcAddr.PublicKeyHash;
            var blockChain = _blockChainStore.GetBlockChain();
            foreach (var txIn in transaction.TransactionIn)
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
    }
}

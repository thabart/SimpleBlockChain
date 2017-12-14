using SimpleBlockChain.Core.Factories;
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
    }

    internal class TransactionHelper : ITransactionHelper
    {
        private readonly IBlockChainFactory _blockChainFactory;

        public TransactionHelper(IBlockChainFactory blockChainFactory)
        {
            _blockChainFactory = blockChainFactory;
        }

        public long CalculateBalance(BaseTransaction transaction, string encodedBcAddr, Networks network)
        {
            if (string.IsNullOrWhiteSpace(encodedBcAddr))
            {
                throw new ArgumentNullException(nameof(encodedBcAddr));
            }

            var txIn = GetTransactionIn(transaction, encodedBcAddr, network);
            var txOut = transaction.GetTransactionOut(encodedBcAddr);
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
            var result = ((double)(transaction.Serialize().Count() / (double)1000) * Constants.DEFAULT_MIN_TX_REWARD) + leftValue;
            return (long)result;
        }

        public TransactionOut GetTransactionIn(BaseTransaction transaction, Networks network)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var blockChain = _blockChainFactory.Build(network);
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
            var blockChain = _blockChainFactory.Build(network);
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

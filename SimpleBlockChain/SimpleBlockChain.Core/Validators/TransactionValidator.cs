using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Scripts;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Linq;

namespace SimpleBlockChain.Core.Validators
{
    public interface ITransactionValidator
    {
        void Check(BaseTransaction transaction);
    }

    internal class TransactionValidator : ITransactionValidator
    {
        private readonly IBlockChainStore _blockChainStore;
        private readonly IScriptInterpreter _scriptInterpreter;

        public TransactionValidator(IBlockChainStore blockChainStore, IScriptInterpreter scriptInterpreter)
        {
            _blockChainStore = blockChainStore;
            _scriptInterpreter = scriptInterpreter;
        }

        public void Check(BaseTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var blockChain = _blockChainStore.GetBlockChain();
            var isCoinBaseTransaction = transaction is CoinbaseTransaction; // https://bitcoin.org/en/developer-guide#block-chain-overview
            if (!isCoinBaseTransaction && (transaction.TransactionIn == null || !transaction.TransactionIn.Any()))
            {
                throw new ValidationException(ErrorCodes.NoTransactionIn);
            }
            
            if (!isCoinBaseTransaction)
            {
                long totalOutput = 0;
                foreach (var txIn in transaction.TransactionIn)
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
                    if (!_scriptInterpreter.Check(sigScript, pkScript))
                    {
                        throw new ValidationException(ErrorCodes.TransactionSignatureNotCorrect);
                    }

                    totalOutput += previousTxOut.Value;
                }

                var sumOutput = transaction.TransactionOut.Sum(t => t.Value);
                if (sumOutput > totalOutput)
                {
                    throw new ValidationException(ErrorCodes.TransactionOutputExceedInput);
                }
            }
        }
    }
}

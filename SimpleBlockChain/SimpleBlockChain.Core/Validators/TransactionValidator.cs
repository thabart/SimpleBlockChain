using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Compiler;
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
        void Check(BcBaseTransaction transaction);
        void Check(SmartContractTransaction transaction);
    }

    internal class TransactionValidator : ITransactionValidator
    {
        private readonly IBlockChainStore _blockChainStore;
        private readonly ISmartContractStore _smartContractStore;
        private readonly IScriptInterpreter _scriptInterpreter;

        public TransactionValidator(IBlockChainStore blockChainStore, ISmartContractStore smartContractStore, IScriptInterpreter scriptInterpreter)
        {
            _blockChainStore = blockChainStore;
            _smartContractStore = smartContractStore;
            _scriptInterpreter = scriptInterpreter;
        }

        public void Check(BaseTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var bcTx = transaction as BcBaseTransaction;
            if (bcTx != null)
            {
                Check(bcTx);
                return;
            }

            Check(transaction as SmartContractTransaction);
        }

        public void Check(BcBaseTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var blockChain = _blockChainStore.GetBlockChain();
            var memoryPool = MemoryPool.Instance();
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
                    var noneCoinBaseTxIn = txIn as TransactionInNoneCoinbase; // Check TRANSACTION EXISTS IN BLOCK CHAIN & MEMORY POOL.
                    var previousTxId = noneCoinBaseTxIn.Outpoint.Hash;
                    var previousIndex = noneCoinBaseTxIn.Outpoint.Index;
                    /*
                    if (memoryPool.ContainsTransactions(previousTxId, previousIndex))
                    {
                        throw new ValidationException(ErrorCodes.AlreadySpentInMemoryPool);
                    }
                    */

                    var previousTxOut = blockChain.GetUnspentTransaction(previousTxId, previousIndex);
                    if (previousTxOut == null)
                    {
                        var r = memoryPool.GetUnspentTransaction(previousTxId, previousIndex) as TransactionOut;
                        if (r == null)
                        {
                            throw new ValidationException(ErrorCodes.ReferencedTransactionNotValid);
                        }

                        previousTxOut = new UTXO
                        {
                            Index = (int)previousIndex,
                            Script = r.Script,
                            TxId = transaction.GetTxId(),
                            Value = r.Value
                        };
                    }
                    
                    var sigScript = Script.Deserialize(noneCoinBaseTxIn.SignatureScript);  // Check SCRIPT.
                    var pkScript = previousTxOut.Script;
                    if (!_scriptInterpreter.Check(sigScript, pkScript))
                    {
                        throw new ValidationException(ErrorCodes.TransactionSignatureNotCorrect);
                    }

                    totalOutput += previousTxOut.Value;
                }

                var sumOutput = transaction.TransactionOut.Where(t => t is TransactionOut).Sum(t => ((TransactionOut)t).Value);
                if (sumOutput > totalOutput)
                {
                    throw new ValidationException(ErrorCodes.TransactionOutputExceedInput);
                }
            }
        }

        public void Check(SmartContractTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var blockChain = _blockChainStore.GetBlockChain();
            var smartContracts = _smartContractStore.GetSmartContracts();
            var vm = new SolidityVm();
            var defaultCallValue = new DataWord(new byte[] { 0x00 });
            if (transaction.To == null)
            {
                if (transaction.From == null)
                {
                    throw new ArgumentNullException(nameof(transaction.From));
                }

                if (transaction.From.Count() != 20)
                {
                    throw new ValidationException(ErrorCodes.FromInvalidLength);
                }

                if (transaction.Data == null)
                {
                    throw new ArgumentNullException(nameof(transaction.Data));
                }

                var program = new SolidityProgram(transaction.Data.ToList(), new SolidityProgramInvoke(new DataWord(transaction.From.ToArray()), defaultCallValue)); // TRY TO GET THE CONTRACT.
                try
                {
                    while (!program.IsStopped())
                    {
                        program.Step();
                    }

                    var hReturn = program.GetResult().GetHReturn();
                    if (hReturn == null || !hReturn.Any())
                    {
                        throw new ValidationException(ErrorCodes.SmartContractNotValid);
                    }
                }
                catch(Exception)
                {
                    throw new ValidationException(ErrorCodes.SmartContractNotValid);
                }
            }
            else
            {
                if (transaction.To.Count() != 20)
                {
                    throw new ValidationException(ErrorCodes.ToInvalidLength);
                }

                var smartContract = smartContracts.GetSmartContract(transaction.To);
                if (smartContract == null)
                {
                    throw new ValidationException(ErrorCodes.SmartContractDoesntExist);
                }
            }
        }
    }
}

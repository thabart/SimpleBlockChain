using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Builders
{
    public class NoneCoinbaseTransactionBuilder : TransactionBuilder
    {
        public NoneCoinbaseTransactionBuilder() : base(new NoneCoinbaseTransaction()) { }

        public NoneCoinbaseTransactionBuilder Spend(BaseTransaction transaction, UInt32 index, IEnumerable<byte> signatureScript, uint sequence = 0xffffffff)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            if (index < 0)
            {
                // TODO : THROW
            }

            if (transaction.TransactionOut.Count() > index)
            {
                // TODO : THROW
            }

            if (signatureScript == null)
            {
                throw new ArgumentNullException(nameof(signatureScript));
            }

            var txId = transaction.GetTxId();
            var outpoint = new Outpoint(txId, index);
            var transactionIn = new TransactionInNoneCoinbase(outpoint, signatureScript, sequence);
            Transaction.TransactionIn.Add(transactionIn);
            return this;
        }
    }
}

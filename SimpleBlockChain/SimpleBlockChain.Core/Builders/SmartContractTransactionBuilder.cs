using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Builders
{
    public class SmartContractTransactionBuilder : TransactionBuilder
    {
        public SmartContractTransactionBuilder() : base(new SmartContractTransaction()) { }

        public TransactionBuilder CreateSmartContract(IEnumerable<byte> code, Script script, string author, string name, uint height, byte[] nonce, uint sequence = 0xffffffff)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (script == null)
            {
                throw new ArgumentNullException(nameof(script));
            }

            if (string.IsNullOrWhiteSpace(author))
            {
                throw new ArgumentNullException(nameof(author));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var transactionOut = new TransactionOutSmartContract(script, code, author, name);
            Transaction.TransactionOut.Add(transactionOut);
            var transactionInCoinbase = new TransactionInCoinbase(height, nonce, sequence);
            Transaction.TransactionIn.Add(transactionInCoinbase);
            return this;
        }
    }
}

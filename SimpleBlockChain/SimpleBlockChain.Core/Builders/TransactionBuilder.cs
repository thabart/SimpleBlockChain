using SimpleBlockChain.Core.Transactions;
using System;

namespace SimpleBlockChain.Core.Builders
{
    public class TransactionBuilder
    {
        protected BaseTransaction Transaction;

        public TransactionBuilder NewNoneCoinbaseTransaction()
        {
            return this;
        }

        public TransactionBuilder NewCoinbaseTransaction()
        {
            return this;
        }
        
        public TransactionBuilder AddOutput(long value, Script script)
        {
            var transactionOutput = new TransactionOut(value, script);
            Transaction.TransactionOut.Add(transactionOutput);
            return this;
        }

        public TransactionBuilder SetTime(UInt32 value)
        {
            Transaction.LockTime = value;
            return this;
        }

        public TransactionBuilder SetBlockNumber(UInt32 value)
        {
            Transaction.LockTime = value;
            return this;
        }

        public BaseTransaction Build()
        {
            return Transaction;
        }
    }
}

using SimpleBlockChain.Core.Transactions;
using System;

namespace SimpleBlockChain.Core.Builders
{
    public class TransactionBuilder
    {
        protected BaseTransaction Transaction;

        public TransactionBuilder()
        {
        }

        public TransactionBuilder(BaseTransaction transaction)
        {
            Transaction = transaction;
        }

        public NoneCoinbaseTransactionBuilder NewNoneCoinbaseTransaction()
        {
            return new NoneCoinbaseTransactionBuilder();
        }

        public CoinbaseTransactionBuilder NewCoinbaseTransaction()
        {
            return new CoinbaseTransactionBuilder();
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

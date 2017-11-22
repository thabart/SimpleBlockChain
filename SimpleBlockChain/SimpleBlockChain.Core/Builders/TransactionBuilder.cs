﻿using SimpleBlockChain.Core.Transactions;
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

        public NoneCoinbaseTransactionBuilder NewNoneCoinbaseTransaction(uint version, uint lockTime)
        {
            return new NoneCoinbaseTransactionBuilder(version, lockTime);
        }

        public CoinbaseTransactionBuilder NewCoinbaseTransaction()
        {
            return new CoinbaseTransactionBuilder();
        }

        public CoinbaseTransactionBuilder NewCoinbaseTransaction(uint version, uint lockTime)
        {
            return new CoinbaseTransactionBuilder(version, lockTime);
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

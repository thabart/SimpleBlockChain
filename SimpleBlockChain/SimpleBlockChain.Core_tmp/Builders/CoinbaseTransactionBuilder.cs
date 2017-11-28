using SimpleBlockChain.Core.Transactions;

namespace SimpleBlockChain.Core.Builders
{
    public class CoinbaseTransactionBuilder : TransactionBuilder
    {
        public CoinbaseTransactionBuilder() : base(new CoinbaseTransaction()) { }
        public CoinbaseTransactionBuilder(uint version, uint lockTime) : base(new CoinbaseTransaction(version, lockTime)) { }

        public CoinbaseTransactionBuilder SetInput(uint height, byte[] nonce, uint sequence = 0xffffffff)
        {
            var transactionInCoinbase = new TransactionInCoinbase(height, nonce, sequence);
            Transaction.TransactionIn.Add(transactionInCoinbase);
            return this;
        }
    }
}

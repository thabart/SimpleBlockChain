using SimpleBlockChain.Core.Transactions;

namespace SimpleBlockChain.Core.Builders
{
    public class CoinbaseTransactionBuilder : TransactionBuilder
    {
        public CoinbaseTransactionBuilder SetInput(uint height, byte[] none, uint sequence = 0xffffffff)
        {
            var transactionInCoinbase = new TransactionInCoinbase(height, none, sequence);
            Transaction.TransactionIn.Add(transactionInCoinbase);
            return this;
        }
    }
}

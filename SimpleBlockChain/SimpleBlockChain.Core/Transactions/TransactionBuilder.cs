namespace SimpleBlockChain.Core.Transactions
{
    public class TransactionBuilder
    {
        private Transaction _transaction;

        public TransactionBuilder()
        {
            _transaction = new Transaction();
        }

        public void New()
        {
            _transaction = new Transaction();
        }

        public TransactionBuilder AddOutput(long value, Script script)
        {
            var transactionOutput = new TransactionOut(value, script);
            _transaction.TransactionOut.Add(transactionOutput);
            return this;
        }
    }
}

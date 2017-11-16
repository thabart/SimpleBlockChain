namespace SimpleBlockChain.Core.Transactions
{
    public class TransactionBuilder
    {
        private Transaction _transaction;

        public TransactionBuilder()
        {
            _transaction = new Transaction();
        }

        public TransactionBuilder New()
        {
            _transaction = new Transaction();
            return this;
        }

        public TransactionBuilder AddOutput(long value, Script script)
        {
            var transactionOutput = new TransactionOut(value, script);
            _transaction.TransactionOut.Add(transactionOutput);
            return this;
        }

        public TransactionBuilder AddInput(TransactionIn transactionIn)
        {
            _transaction.TransactionIn.Add(transactionIn);
            return this;
        }

        public Transaction Build()
        {
            return _transaction;
        }
    }
}

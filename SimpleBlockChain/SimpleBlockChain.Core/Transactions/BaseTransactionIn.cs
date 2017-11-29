namespace SimpleBlockChain.Core.Transactions
{
    public abstract class BaseTransactionIn
    {
        public abstract byte[] Serialize();
        public abstract long GetValue();
    }
}

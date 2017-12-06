namespace SimpleBlockChain.Core.Exceptions
{
    public class DataWalletException : WalletException
    {
        public DataWalletException(string code) : base(code)
        {
        }
    }
}

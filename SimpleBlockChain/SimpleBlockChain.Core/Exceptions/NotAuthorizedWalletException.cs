namespace SimpleBlockChain.Core.Exceptions
{
    public class NotAuthorizedWalletException : WalletException
    {
        public NotAuthorizedWalletException(string code) : base(code) { }
    }
}

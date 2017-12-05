namespace SimpleBlockChain.Data.Sqlite.Models
{
    internal class WalletAddress
    {
        public string Hash { get; set; }
        public string SerializedKey { get; set; }
        public string WalletName { get; set; }
        public virtual Wallet Wallet { get; set; }
    }
}

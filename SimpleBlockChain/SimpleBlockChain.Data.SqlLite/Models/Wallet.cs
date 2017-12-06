namespace SimpleBlockChain.Data.Sqlite.Models
{
    public class Wallet
    {
        public Wallet(string name, string serializedContent)
        {
            Name = name;
            SerializedContent = serializedContent;
        }

        public string Name { get; set; }
        public string SerializedContent { get; set; }
        // public virtual ICollection<WalletAddress> Addresses { get; set; }
    }
}

using System.Collections.Generic;

namespace SimpleBlockChain.Data.Sqlite.Models
{
    public class Wallet
    {
        public string Name { get; set; }
        public virtual ICollection<WalletAddress> Addresses { get; set; }
    }
}

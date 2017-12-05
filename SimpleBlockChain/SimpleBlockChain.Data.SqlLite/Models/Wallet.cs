using System.Collections.Generic;

namespace SimpleBlockChain.Data.Sqlite.Models
{
    internal class Wallet
    {
        public string Name { get; set; }
        public virtual ICollection<WalletAddress> Addresses { get; set; }
    }
}

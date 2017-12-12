using System.Collections.Generic;

namespace SimpleBlockChain.Core.Aggregates
{
    public class WalletAggregate
    {
        public string Name { get; set; }
        public Networks Network { get; set; }
        public ICollection<WalletAggregateAddress> Addresses { get; set; }
    }
}

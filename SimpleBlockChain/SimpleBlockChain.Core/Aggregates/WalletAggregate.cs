using System.Collections.Generic;

namespace SimpleBlockChain.Core.Aggregates
{
    public class WalletAggregate
    {
        public string Name { get; set; }
        public IEnumerable<WalletAggregateAddress> Addresses { get; set; }
    }
}

using SimpleBlockChain.Core.Crypto;

namespace SimpleBlockChain.Core.Aggregates
{
    public class WalletAggregateAddress
    {
        public string Hash { get; set; }
        public Key Key { get; set; }
    }
}

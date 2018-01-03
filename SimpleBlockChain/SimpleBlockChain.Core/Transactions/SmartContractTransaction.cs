using System.Collections.Generic;

namespace SimpleBlockChain.Core.Transactions
{
    public class SmartContractTransaction
    {
        public IEnumerable<byte> From { get; set; }
        public IEnumerable<byte> To { get; set; }
        public IEnumerable<byte> Data { get; set; }
    }
}

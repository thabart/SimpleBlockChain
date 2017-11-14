using SimpleBlockChain.Core.Common;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Transactions
{
    public class Transaction
    {
        private const int CURRENT_VERSION = 2;
        private const int MAX_STANDARD_VERSION = 2;
        public uint Version { get; set; } // version
        public uint LockTime { get; set; } // lock_time
        public List<TransactionIn> TransactionIn { get; private set; } // tx_in
        public CompactSize TransactionInCount { get; private set; } // tx_in_count
        public List<TransactionOut> TransactionOut { get; private set; } // tx_out
        public CompactSize TransactionOutCount { get; private set; } // tx_out_count

        public Transaction()
        {
            TransactionOut = new List<TransactionOut>();
            TransactionIn = new List<TransactionIn>();
        }
    }
}

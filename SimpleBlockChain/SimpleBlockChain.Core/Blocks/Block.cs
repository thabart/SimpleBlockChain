using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Blocks
{
    public class Block
    {
        private static int CURRENT_VERSION = 4;

        public Block()
        {
            Transactions = new List<BaseTransaction>();
        }

        public Block PreviousBlock { get; set; }

        public static Block GetGenesisBlock()
        {
            return null;
        }

        public IEnumerable<BaseTransaction> Transactions { get; set; }

        public byte[] Serialize()
        {
            var result = new List<byte>();
            var rawTransactions = new List<byte>();
            var compactSize = new CompactSize
            {
                Size = (ulong)Transactions.Count()
            };
            foreach(var transaction in Transactions)
            {
                rawTransactions.AddRange(transaction.Serialize());
            }

            result.AddRange(SerializeHeader());
            result.AddRange(compactSize.Serialize());
            result.AddRange(rawTransactions);
            return result.ToArray();
        }

        public byte[] SerializeHeader()
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(CURRENT_VERSION));

            return result.ToArray();
        }
    }
}

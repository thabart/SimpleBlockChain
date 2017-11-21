using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

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

        public IList<BaseTransaction> Transactions { get; set; }

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
            var merkleTree = GetMerkleRoot();
            return result.ToArray();
        }

        public byte[] SerializeHeader()
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(CURRENT_VERSION));

            return result.ToArray();
        }

        private IEnumerable<byte> GetMerkleRoot()
        {
            var orderedTransactions = Transactions.OrderBy(p => p);
            if (orderedTransactions.Count() == 1)
            {
                var transaction = orderedTransactions.First() as CoinbaseTransaction;
                if (transaction != null)
                {
                    return transaction.GetTxId();
                }
            }


            return CalculateMerkleTreeHash(orderedTransactions.Select(t => t.GetTxId()));
        }

        private static IEnumerable<byte> CalculateMerkleTreeHash(IEnumerable<IEnumerable<byte>> lstTxIds)
        {
            var mySHA256 = SHA256Managed.Create();
            if (lstTxIds.Count() == 2)
            {
                var record = new List<byte>();
                record.AddRange(lstTxIds.First());
                record.AddRange(lstTxIds.Last());
                return mySHA256.ComputeHash(record.ToArray());
            }

            if (lstTxIds.Count() == 1)
            {
                var record = new List<byte>();
                record.AddRange(lstTxIds.First());
                record.AddRange(lstTxIds.First());
                return mySHA256.ComputeHash(record.ToArray());
            }

            var result = new List<IEnumerable<byte>>();
            var nbIterations = Math.Round((double)(lstTxIds.Count() / 2));
            var remain = lstTxIds.Count() - nbIterations;
            int startIndex = 0;
            for (var i = 0; i < nbIterations; i++)
            {
                var firstTransaction = lstTxIds.Skip(startIndex).First();
                var secondTransaction = lstTxIds.Skip(startIndex + 1).First();
                startIndex += 2;
                var record = new List<byte>();
                record.AddRange(firstTransaction);
                record.AddRange(secondTransaction);
                result.Add(mySHA256.ComputeHash(record.ToArray()));
            }

            if (remain > 0)
            {
                var record = new List<byte>();
                record.AddRange(lstTxIds.Last());
                record.AddRange(lstTxIds.Last());
                result.Add(mySHA256.ComputeHash(record.ToArray()));
            }

            return CalculateMerkleTreeHash(result);
        }
    }
}

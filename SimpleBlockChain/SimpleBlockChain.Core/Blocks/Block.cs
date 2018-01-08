using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SimpleBlockChain.Core.Blocks
{
    public class Block
    {        
        public Block(IEnumerable<byte> previousHashHeader, uint nBits, uint nonce, int version = BlockHeader.CURRENT_VERSION)
        {
            if (previousHashHeader == null)
            {
                previousHashHeader = Enumerable.Repeat((byte)0x00, 32);
            }

            Transactions = new List<BcBaseTransaction>();
            BlockHeader = new BlockHeader
            {
                NBits = nBits,
                Nonce = nonce,
                PreviousBlockHeader = previousHashHeader,
                Version = version
            };
        }

        public Block(BlockHeader blockHeader, IList<BcBaseTransaction> transactions)
        {
            if (blockHeader == null)
            {
                throw new ArgumentNullException(nameof(blockHeader));
            }
            var previousHashHeader = blockHeader.PreviousBlockHeader;
            if (previousHashHeader == null)
            {
                previousHashHeader = Enumerable.Repeat((byte)0x00, 32);
            }

            BlockHeader = blockHeader;
            Transactions = transactions;
        }

        public BlockHeader BlockHeader { get; private set; }

        public void UpdateMerkleRoot()
        {
            BlockHeader.MerkleRoot = GetMerkleRoot();
        }

        public long GetTotalFees()
        {
            if (Transactions == null || !Transactions.Any())
            {
                return 0;
            }

            long result = 0;
            foreach(var tx in Transactions)
            {
                var cbTx = tx as NoneCoinbaseTransaction;
                if (cbTx == null)
                {
                    continue;
                }

                result += ((TransactionOut)cbTx.TransactionOut.First()).Value;
            }

            return result;
        }

        // Read the target nbits : https://bitcoin.org/en/developer-reference#target-nbits

        public static Block BuildGenesisBlock()
        {
            var scriptBuilder = new ScriptBuilder();
            var script = scriptBuilder
                .New()
                .AddOperation(OpCodes.OP_DUP)
                .AddOperation(OpCodes.OP_HASH160)
                .AddToStack(Constants.DEFAULT_GENESIS_PUBLIC_KEY_HASH.ToByteArray())
                .AddOperation(OpCodes.OP_EQUALVERIFY)
                .AddOperation(OpCodes.OP_CHECKSIG)
                .Build();
            var result = new Block(null, Constants.DEFAULT_NBITS, Constants.DEFAULT_GENESIS_NONCE, 1);
            result.BlockHeader.Time = (new DateTime(2017, 12, 14, 12, 0, 0)).ToUnixTimeUInt32();
            var transactionBuilder = new TransactionBuilder();
            const string txt = "The Times 03/Jan/2009 Chancellor on brink of second bailout for banks";
            var coinBase = System.Text.ASCIIEncoding.ASCII.GetBytes(txt).ToArray();
            var transaction = transactionBuilder
                .NewCoinbaseTransaction(Constants.DEFAULT_GENESIS_TRANSACTION_VERSION, Constants.DEFAULT_GENESIS_TRANSACTION_LOCKTIME)
                .SetInput(0x4D, coinBase, Constants.DEFAULT_GENESIS_SEQUENCE)
                .AddOutput(50, script)
                .Build();
            result.Transactions.Add(transaction);
            return result;
        }

        public IList<BcBaseTransaction> Transactions { get; set; }

        public static Block Deserialize(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var header = DeserializeBlockHeader(payload);
            int currentIndex = 80;
            var transactionLst = new List<BcBaseTransaction>();
            var kvp = CompactSize.Deserialize(payload.Skip(80).ToArray());
            currentIndex += kvp.Value;
            if (kvp.Key.Size > 0)
            {
                for(var i = 0; i < (int)kvp.Key.Size; i++)
                {
                    var type = i == 0 ? TransactionTypes.Coinbase : TransactionTypes.NoneCoinbase;
                    var skvp = BcBaseTransaction.Deserialize(payload.Skip(currentIndex), type);
                    transactionLst.Add(skvp.Key);
                    currentIndex += skvp.Value;
                }
            }

            var result = new Block(header.PreviousBlockHeader, header.NBits, header.Nonce);
            result.BlockHeader = header;
            result.Transactions = transactionLst;
            return result;
        }

        public static BlockHeader DeserializeBlockHeader(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (payload.Count() < BlockHeader.SIZE)
            {
                // TODO : EXCEPTION
                return null;
            }

            var version = BitConverter.ToInt32(payload.Take(4).ToArray(), 0);
            var previousBlockHashHeader = payload.Skip(4).Take(32);
            var merkleRootHash = payload.Skip(36).Take(32);
            var time = BitConverter.ToUInt32(payload.Skip(68).Take(4).ToArray(), 0);
            var nBits = BitConverter.ToUInt32(payload.Skip(72).Take(4).ToArray(), 0);
            var nonce = BitConverter.ToUInt32(payload.Skip(76).Take(4).ToArray(), 0);
            return new BlockHeader
            {
                MerkleRoot = merkleRootHash,
                NBits = nBits,
                Nonce = nonce,
                PreviousBlockHeader = previousBlockHashHeader,
                Time = time,
                Version = version
            };
        }

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

        public byte[] GetHashHeader()
        {
            var payload = SerializeHeader();
            var mySHA256 = SHA256.Create();
            return mySHA256.ComputeHash(mySHA256.ComputeHash(payload));
        }

        public byte[] SerializeHeader()
        {
            // https://bitcoin.org/en/developer-reference#block-headers
            var result = new List<byte>();
            var merkleTree = GetMerkleRoot();
            var time = BlockHeader.Time;
            result.AddRange(BitConverter.GetBytes(BlockHeader.Version));
            result.AddRange(BlockHeader.PreviousBlockHeader);
            result.AddRange(merkleTree);
            result.AddRange(BitConverter.GetBytes(time));
            result.AddRange(BitConverter.GetBytes(BlockHeader.NBits));
            result.AddRange(BitConverter.GetBytes(BlockHeader.Nonce));
            return result.ToArray();
        }

        public IEnumerable<byte> GetMerkleRoot()
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
            var mySHA256 = SHA256.Create();
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

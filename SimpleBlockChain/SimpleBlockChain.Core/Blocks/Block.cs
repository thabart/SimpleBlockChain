using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Exceptions;
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
        private const int CURRENT_VERSION = 4;

        private IEnumerable<byte> _previousHashHeader;
        private uint _nbits;
        private uint _nonce;
        private int _version;

        private DateTime _blockHeaderHashingStartTime { get; set; }
        
        public Block(IEnumerable<byte> previousHashHeader, uint nBits, uint nonce, int version = CURRENT_VERSION)
        {
            if (previousHashHeader == null)
            {
                previousHashHeader = Enumerable.Repeat((byte)0x00, 32);
            }

            Transactions = new List<BaseTransaction>();
            _previousHashHeader = previousHashHeader;
            _nbits = nBits;
            _nonce = nonce;
            _version = version;
        }

        public void SetBlockHeaderHashingStartTime(DateTime dateTime)
        {
            _blockHeaderHashingStartTime = dateTime;
        }

        public BlockHeader BlockHeader { get; private set; }

        public void Check()
        {
            var merkleRoot = BlockHeader.MerkleRoot;
            var calculatedMerkleRoot = GetMerkleRoot();
            if (merkleRoot.SequenceEqual(calculatedMerkleRoot))
            {
                throw new ValidationException(ErrorCodes.InvalidMerkleRoot);
            }

            foreach(var transaction in Transactions)
            {
                transaction.Check();
            }
        }

        // Read the target nbits : https://bitcoin.org/en/developer-reference#target-nbits

        public static Block BuildGenesisBlock()
        {
            var scriptBuilder = new ScriptBuilder();
            var script = scriptBuilder
                .New()
                .AddToStack(Constants.DEFAULT_GENESIS_PUBLIC_KEY.ToByteArray())
                .AddOperation(OpCodes.OP_CHECKSIG)
                .Build();
            var result = new Block(null, Constants.DEFAULT_NBITS, Constants.DEFAULT_GENESIS_NONCE, 1);
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

        public IList<BaseTransaction> Transactions { get; set; }

        public static Block Deserialize(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var header = DeserializeBlockHeader(payload);
            int currentIndex = 80;
            var transactionLst = new List<BaseTransaction>();
            var kvp = CompactSize.Deserialize(payload.Skip(80).ToArray());
            currentIndex += kvp.Value;
            if (kvp.Key.Size > 0)
            {
                for(var i = 0; i < (int)kvp.Key.Size; i++)
                {
                    var type = i == 0 ? TransactionTypes.Coinbase : TransactionTypes.NoneCoinbase;
                    var skvp = BaseTransaction.Deserialize(payload.Skip(currentIndex), type);
                    transactionLst.Add(skvp.Key);
                    currentIndex += skvp.Value;
                }
            }

            var result = new Block(header.PreviousBlockHeader, header.NBits, header.Nonce);
            result.BlockHeader = header;
            result.Transactions = transactionLst;
            return result;
        }

        private static BlockHeader DeserializeBlockHeader(IEnumerable<byte> payload)
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

            var version = BitConverter.ToUInt32(payload.Take(4).ToArray(), 0);
            var previousBlockHashHeader = payload.Skip(4).Take(32);
            var merkleRootHash = payload.Skip(36).Take(32);
            var time = BitConverter.ToUInt32(payload.Skip(68).Take(4).ToArray(), 0).ToDateTime();
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

        public byte[] SerializeHeader()
        {
            // https://bitcoin.org/en/developer-reference#block-headers
            var result = new List<byte>();
            var merkleTree = GetMerkleRoot();
            var time = BitConverter.GetBytes((UInt32)_blockHeaderHashingStartTime.ToUnixTime());
            result.AddRange(BitConverter.GetBytes(_version));
            result.AddRange(_previousHashHeader);
            result.AddRange(merkleTree);
            result.AddRange(time);
            result.AddRange(BitConverter.GetBytes(_nbits));
            result.AddRange(BitConverter.GetBytes(_nonce));
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

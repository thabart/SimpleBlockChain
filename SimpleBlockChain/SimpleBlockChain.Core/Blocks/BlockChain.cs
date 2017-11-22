using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SimpleBlockChain.Core.Blocks
{
    public class BlockChain
    {
        public BlockChain()
        {
            Blocks = new List<Block>
            {
                Block.BuildGenesisBlock()
            };
        }

        public IList<Block> Blocks { get; private set; }

        public BlockChain AddBlock(Block block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            var lastBlock = Blocks.Last();
            var lastHeaderBlock = lastBlock.SerializeHeader();
            var mySHA256 = SHA256Managed.Create();
            var newPreviousBlockHashHeader = mySHA256.ComputeHash(mySHA256.ComputeHash(lastHeaderBlock));
            if (!block.BlockHeader.PreviousBlockHeader.SequenceEqual(newPreviousBlockHashHeader))
            {
                throw new ValidationException(ErrorCodes.PreviousHashBlockDoesntMatch);
            }

            block.Check();
            Blocks.Add(block);
            return this;
        }

        public bool ContainsUnspentTransaction(IEnumerable<byte> txId, uint index)
        {
            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }
            // TODO : IMPLEMENT.

            return true;
        }

        public BaseTransaction GetUnspentTransaction(IEnumerable<byte> txId, uint index)
        {
            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }

            // TODO : IMPLEMENT.

            return null;
        }

        public bool ContainsTransaction(IEnumerable<byte> txId)
        {
            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }

            return false;
        }
        public BaseTransaction GetTransaction(IEnumerable<byte> txId)
        {
            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }

            return null;
        }
    }
}

using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Stores;
using System;
using System.Linq;

namespace SimpleBlockChain.Core.Validators
{
    public interface IBlockValidator
    {
        void Check(Block block);
    }

    internal class BlockValidator : IBlockValidator
    {
        private readonly IBlockChainStore _blockChainStore;
        private readonly ITransactionValidator _transactionValidator;

        public BlockValidator(IBlockChainStore blockChainStore, ITransactionValidator transactionValidator)
        {
            _blockChainStore = blockChainStore;
            _transactionValidator = transactionValidator;
        }

        public void Check(Block block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            var merkleRoot = block.BlockHeader.MerkleRoot; // Check MERKLE-ROOT.
            var calculatedMerkleRoot = block.GetMerkleRoot();
            if (!merkleRoot.SequenceEqual(calculatedMerkleRoot))
            {
                throw new ValidationException(ErrorCodes.InvalidMerkleRoot);
            }

            var blockChain = _blockChainStore.GetBlockChain(); // Check PREVIOUS BLOCK.
            var currentBlock = blockChain.GetCurrentBlock();
            if (!currentBlock.GetHashHeader().SequenceEqual(block.BlockHeader.PreviousBlockHeader))
            {
                throw new ValidationException(ErrorCodes.InvalidPreviousHashHeader);
            }

            var hash = currentBlock.GetHashHeader();
            var currentNBits = Constants.DEFAULT_NBITS; // TODO : CALCULATE THE DEFAULT NBITS : https://bitcoin.org/en/developer-guide#proof-of-work
            var target = TargetHelper.GetTarget(currentNBits);
            if (!TargetHelper.IsValid(hash, target))
            {
                throw new ValidationException(ErrorCodes.NotEnoughDifficult);
            }

            foreach (var transaction in block.Transactions) // Check ALL TRANSACTIONS.
            {
                _transactionValidator.Check(transaction);
            }
        }
    }
}

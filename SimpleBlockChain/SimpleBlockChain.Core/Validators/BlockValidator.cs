using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Factories;
using SimpleBlockChain.Core.Helpers;
using System;
using System.Linq;

namespace SimpleBlockChain.Core.Validators
{
    public interface IBlockValidator
    {
        void Check(Block block, Networks network);
    }

    internal class BlockValidator : IBlockValidator
    {
        private readonly IBlockChainFactory _blockChainFactory;
        private readonly ITransactionValidator _transactionValidator;

        public BlockValidator(IBlockChainFactory blockChainFactory, ITransactionValidator transactionValidator)
        {
            _blockChainFactory = blockChainFactory;
            _transactionValidator = transactionValidator;
        }

        public void Check(Block block, Networks network)
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

            var blockChain = _blockChainFactory.Build(network); // Check PREVIOUS BLOCK.
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
                _transactionValidator.Check(transaction, network);
            }
        }
    }
}

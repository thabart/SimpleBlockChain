using NUnit.Framework;
using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Core.Transactions;

namespace SimpleBlockChain.UnitTests.Blocks
{
    [TestFixture]
    public class BlockChainFixture
    {
        [Test]
        public void WhenGetGenesisBlock()
        {
            var blockChain = new BlockChain();
            var block = blockChain.GetCurrentBlock();
            Assert.NotNull(block);
        }

        [Test]
        public void WhenAddBlock()
        {
            // Genesis block <=> block1
            // var blockChain = new BlockChain();
            // var block = blockChain.GetCurrentBlock();
            var w = GetKey();
            var adr = new BlockChainAddress(ScriptTypes.P2PKH, Networks.MainNet, w); // Get genesis address.

        }

        private Key GetKey()
        {
            var keyRepository = new KeyRepository();
            keyRepository.Load("titi");
            return null;
        }
    }
}

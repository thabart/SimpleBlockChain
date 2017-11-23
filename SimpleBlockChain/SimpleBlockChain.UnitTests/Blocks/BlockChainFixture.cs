using NUnit.Framework;
using SimpleBlockChain.Core.Blocks;

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
    }
}

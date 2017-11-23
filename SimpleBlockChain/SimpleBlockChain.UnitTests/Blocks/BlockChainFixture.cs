using NUnit.Framework;
using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Core.Transactions;

namespace SimpleBlockChain.UnitTests.Blocks
{
    [TestFixture]
    public class BlockChainFixture
    {
        private const ScriptTypes _scriptTypes = ScriptTypes.P2PKH;
        private const Networks _network = Networks.MainNet;

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
            var blockChain = new BlockChain(); // Get the genesis block.
            var genesisBlock = blockChain.GetCurrentBlock();

            var w = GetKey();
            var adr = new BlockChainAddress(_scriptTypes, _network, w); // Get GENESIS block chain address.
            var destinationBlockChainAddress = GenerateBlockChainAddress(); // Destination block chain address.
            var minerBlockChainAddress = GenerateBlockChainAddress(); // Miner block chain address.

            var scriptBuilder = new ScriptBuilder();
            var destinationScript = Script.CreateP2PKHScript(destinationBlockChainAddress.PublicKeyHash);
            var minerScript = Script.CreateP2PKHScript(minerBlockChainAddress.PublicKeyHash);

            var transactionBuilder = new TransactionBuilder();
            var coinBaseTransaction = transactionBuilder
                .NewCoinbaseTransaction()
                .SetBlockNumber(1)
                .AddOutput(1, minerScript)
                .Build(); // Add transaction coin-base (only for the MINER).
            var noneCoinBaseTransaction = transactionBuilder
                .NewNoneCoinbaseTransaction()
                .Spend(null, 0, destinationScript.Serialize())
                // .AddOutput()
                .Build();
        }

        private static BlockChainAddress GenerateBlockChainAddress()
        {
            var key = Key.Genererate();
            var adr = new BlockChainAddress(_scriptTypes, _network, key);
            return adr;
        }

        private Key GetKey()
        {
            var keyRepository = new KeyRepository();
            keyRepository.Load("titi");
            return null;
        }
    }
}

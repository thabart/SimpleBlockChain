using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Transactions;
using System.Linq;
using System.Numerics;
using System.Text;
using Xunit;

namespace SimpleBlockChain.UnitTests.Blocks
{
    public class BlockChainFixture
    {
        private const ScriptTypes _scriptTypes = ScriptTypes.P2PKH;
        private const Networks _network = Networks.MainNet;

        [Fact]
        public void WhenGetGenesisBlock()
        {
            var blockChain = new BlockChain();
            var block = blockChain.GetCurrentBlock();
            Assert.NotNull(block);
        }

        [Fact]
        public void WhenAddBlock()
        {
            var blockChain = BlockChainStore.Instance().GetBlockChain(); // Get the genesis block.
            var genesisBlock = blockChain.GetCurrentBlock();
            var firstTransaction = genesisBlock.Transactions.First();
            var firstTransactionOut = firstTransaction.TransactionOut.First();

            var genesisKey = GetKey();
            var genesisAdr = new BlockChainAddress(_scriptTypes, _network, genesisKey); // Create block chain address.
            var destinationBlockChainAddress = GenerateBlockChainAddress();
            var minerBlockChainAddress = GenerateBlockChainAddress();

            var payload = Encoding.UTF8.GetBytes(Constants.DEFAULT_SIGNATURE_CONTENT); // Create the script
            var signature = genesisKey.Sign(payload);
            var scriptBuilder = new ScriptBuilder();
            var genesisScript = scriptBuilder
                .New()
                .AddToStack(signature)
                .AddToStack(genesisKey.GetPublicKey())
                .Build();
            var destinationScript = Script.CreateP2PKHScript(destinationBlockChainAddress.PublicKeyHash);
            var minerScript = Script.CreateP2PKHScript(minerBlockChainAddress.PublicKeyHash);

            var transactionBuilder = new TransactionBuilder();
            var coinBaseTransaction = transactionBuilder // Add COIN-BASE TRANSACTION.
                .NewCoinbaseTransaction()
                .SetBlockNumber(1)
                .AddOutput(1, minerScript)
                .Build();
            var noneCoinBaseTransaction = transactionBuilder // ADD GENESIS (10 BTC) => DESTINATION TRANSACTION.
                .NewNoneCoinbaseTransaction()
                .Spend(firstTransaction, 0, genesisScript.Serialize())
                .AddOutput(10, destinationScript)
                .Build();

            var nonce = NonceHelper.GetNonceUInt32(); // CREATE A BLOCK.
            var block = new Block(genesisBlock.GetHashHeader(), Constants.DEFAULT_NBITS, nonce);
            block.Transactions.Add(coinBaseTransaction);
            block.Transactions.Add(noneCoinBaseTransaction);
            var a = noneCoinBaseTransaction.Serialize().ToArray();
            var b = BaseTransaction.Deserialize(a, TransactionTypes.NoneCoinbase);
            block.UpdateMerkleRoot();
            block.Check();

            blockChain.AddBlock(block);
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
            keyRepository.Load("mili");
            return keyRepository.Keys.First();
        }
    }
}

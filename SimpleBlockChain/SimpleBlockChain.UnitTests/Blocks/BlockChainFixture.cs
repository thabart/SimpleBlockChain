using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Factories;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Transactions;
using SimpleBlockChain.UnitTests.Stores;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SimpleBlockChain.UnitTests.Blocks
{
    [TestClass]
    public class BlockChainFixture
    {
        private Mock<IAssemblyHelper> _assemblyHelperMock;
        private const ScriptTypes _scriptTypes = ScriptTypes.P2PKH;
        private const Networks _network = Networks.MainNet;

        [TestMethod]
        public void WhenGetGenesisBlock()
        {
            RemoveBlockChain();
            BuildServiceProvider();
            var genesisBlock = Block.BuildGenesisBlock();
            var serviceProvider = BuildServiceProvider();
            var blockChainFactory = serviceProvider.GetService<IBlockChainFactory>();
            var blockChain =  blockChainFactory.Build(_network);
            var block = blockChain.GetBlock(0);
            var genesisPayload = genesisBlock.Serialize();
            var blockPayload = block.Serialize();

            Assert.IsNotNull(block);
            Assert.IsNotNull(genesisPayload);
            Assert.IsNotNull(blockPayload);
            Assert.IsTrue(genesisPayload.SequenceEqual(blockPayload));
        }

        [TestMethod]
        public void WhenAddBlockAfterGenesis()
        {
            RemoveBlockChain();
            var serviceProvider = BuildServiceProvider();
            var blockChainFactory = serviceProvider.GetService<IBlockChainFactory>();
            var blockChain = blockChainFactory.Build(_network); // Get the genesis block.
            var genesisBlock = blockChain.GetCurrentBlock();
            var firstTransaction = genesisBlock.Transactions.First();
            var firstTransactionOut = firstTransaction.TransactionOut.First();

            var genesisKey = KeyStore.GetGenesisKey();
            var genesisAdr = new BlockChainAddress(_scriptTypes, _network, genesisKey); // Create block chain address.
            var destinationBlockChainAddress = GenerateBlockChainAddress();
            var minerBlockChainAddress = GenerateBlockChainAddress();

            var signature = genesisKey.GetSignature(); // Create the script.
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
            blockChain.AddBlock(block);
            var secondBlock = blockChain.GetBlock(1);

            Assert.IsNotNull(secondBlock);
            Assert.IsTrue(secondBlock.GetHashHeader().SequenceEqual(block.GetHashHeader()));
            Assert.IsTrue(blockChain.GetCurrentBlockHeight() == 1);
        }

        [TestMethod]
        public void WhenGetUnspentGenesisTransaction()
        {
            RemoveBlockChain();
            var serviceProvider = BuildServiceProvider();
            var blockChainFactory = serviceProvider.GetService<IBlockChainFactory>();
            var blockChain = blockChainFactory.Build(_network);
            var unspentTransactions = blockChain.GetUnspentTransactions();

            Assert.IsNotNull(unspentTransactions);
            Assert.IsTrue(unspentTransactions.Count() == 1);
        }

        [TestMethod]
        public void WhenGetUnspentTransactionsTwoBlocks()
        {
            RemoveBlockChain();
            var serviceProvider = BuildServiceProvider();
            var blockChainFactory = serviceProvider.GetService<IBlockChainFactory>();
            var blockChain = blockChainFactory.Build(_network);
            var genesisBlock = blockChain.GetCurrentBlock();
            var firstTransaction = genesisBlock.Transactions.First();
            var firstTransactionOut = firstTransaction.TransactionOut.First();

            var genesisKey = KeyStore.GetGenesisKey();
            var genesisAdr = new BlockChainAddress(_scriptTypes, _network, genesisKey); // Create block chain address.
            var destinationBlockChainAddress = GenerateBlockChainAddress();
            var minerBlockChainAddress = GenerateBlockChainAddress();

            var signature = genesisKey.GetSignature(); // Create the script.
            var scriptBuilder = new ScriptBuilder();
            var genesisScript = scriptBuilder
                .New()
                .AddToStack(signature)
                .AddToStack(genesisKey.GetPublicKey())
                .Build();
            var destinationScript = Script.CreateP2PKHScript(destinationBlockChainAddress.PublicKeyHash);
            var minerScript = Script.CreateP2PKHScript(minerBlockChainAddress.PublicKeyHash);
            var genesisScriptDest = Script.CreateP2PKHScript(genesisKey.GetPublicKeyHashed());

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
            var otherCoinBaseTransaction = transactionBuilder
                .NewNoneCoinbaseTransaction()
                .Spend(firstTransaction, 0, genesisScript.Serialize())
                .AddOutput(39, genesisScriptDest)
                .Build();

            var nonce = NonceHelper.GetNonceUInt32(); // CREATE A BLOCK.
            var block = new Block(genesisBlock.GetHashHeader(), Constants.DEFAULT_NBITS, nonce);
            block.Transactions.Add(coinBaseTransaction);
            block.Transactions.Add(noneCoinBaseTransaction);
            block.Transactions.Add(otherCoinBaseTransaction);
            var a = noneCoinBaseTransaction.Serialize().ToArray();
            var b = BaseTransaction.Deserialize(a, TransactionTypes.NoneCoinbase);
            block.UpdateMerkleRoot();
            blockChain.AddBlock(block);
            var unspentTransactions = blockChain.GetUnspentTransactions();

            Assert.IsNotNull(unspentTransactions);
            Assert.IsTrue(unspentTransactions.Count() == 3);
            Assert.IsTrue(unspentTransactions.Sum(t => t.Value) == 50);
        }

        [TestMethod]
        public void WhenGetUnspentTransactionsThreeBlocks()
        {
            RemoveBlockChain();
            var serviceProvider = BuildServiceProvider();
            var blockChainFactory = serviceProvider.GetService<IBlockChainFactory>();
            var blockChain = blockChainFactory.Build(_network);
            var genesisBlock = blockChain.GetCurrentBlock();
            var firstTransaction = genesisBlock.Transactions.First();
            var firstTransactionOut = firstTransaction.TransactionOut.First();

            var genesisKey = KeyStore.GetGenesisKey();
            var genesisAdr = new BlockChainAddress(_scriptTypes, _network, genesisKey); // Create block chain address.
            var destinationBlockChainAddress = GenerateBlockChainAddress();
            var minerBlockChainAddress = GenerateBlockChainAddress();

            var signature = genesisKey.GetSignature(); // Create the script.
            var scriptBuilder = new ScriptBuilder();
            var genesisScript = scriptBuilder
                .New()
                .AddToStack(signature)
                .AddToStack(genesisKey.GetPublicKey())
                .Build();
            var destinationScript = Script.CreateP2PKHScript(destinationBlockChainAddress.PublicKeyHash);
            var minerScript = Script.CreateP2PKHScript(minerBlockChainAddress.PublicKeyHash);
            var genesisScriptDest = Script.CreateP2PKHScript(genesisKey.GetPublicKeyHashed());

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
            var otherCoinBaseTransaction = transactionBuilder
                .NewNoneCoinbaseTransaction()
                .Spend(firstTransaction, 0, genesisScript.Serialize())
                .AddOutput(39, genesisScriptDest)
                .Build();

            var nonce = NonceHelper.GetNonceUInt32(); // CREATE A BLOCK.
            var firstBlock = new Block(genesisBlock.GetHashHeader(), Constants.DEFAULT_NBITS, nonce);
            firstBlock.Transactions.Add(coinBaseTransaction);
            firstBlock.Transactions.Add(noneCoinBaseTransaction);
            firstBlock.Transactions.Add(otherCoinBaseTransaction);
            firstBlock.UpdateMerkleRoot();
            blockChain.AddBlock(firstBlock);
            var unspentTransactions = blockChain.GetUnspentTransactions();

            Assert.IsNotNull(unspentTransactions);
            Assert.IsTrue(unspentTransactions.Count() == 3);

            // ADD THIRD BLOCK.
            coinBaseTransaction = transactionBuilder // Add COIN-BASE TRANSACTION.
                .NewCoinbaseTransaction()
                .SetBlockNumber(2)
                .AddOutput(1, minerScript)
                .Build();
            noneCoinBaseTransaction = transactionBuilder // ADD GENESIS (2 BTC) => DESTINATION TRANSACTION.
                 .NewNoneCoinbaseTransaction()
                 .Spend(otherCoinBaseTransaction, 0, genesisScript.Serialize())
                 .AddOutput(2, destinationScript)
                 .Build();
            otherCoinBaseTransaction = transactionBuilder
                 .NewNoneCoinbaseTransaction()
                 .Spend(otherCoinBaseTransaction, 0, genesisScript.Serialize())
                 .AddOutput(36, genesisScriptDest)
                 .Build();
            nonce = NonceHelper.GetNonceUInt32(); // CREATE A BLOCK.
            var secondBlock = new Block(genesisBlock.GetHashHeader(), Constants.DEFAULT_NBITS, nonce);
            secondBlock.Transactions.Add(coinBaseTransaction);
            secondBlock.Transactions.Add(noneCoinBaseTransaction);
            secondBlock.Transactions.Add(otherCoinBaseTransaction);
            secondBlock.UpdateMerkleRoot();
            blockChain.AddBlock(secondBlock);

            unspentTransactions = blockChain.GetUnspentTransactions();
            Assert.IsTrue(unspentTransactions.Count() == 5);
            Assert.IsTrue(unspentTransactions.Sum(t => t.Value) == 50);
        }

        private static BlockChainAddress GenerateBlockChainAddress()
        {
            var key = Key.Genererate();
            var adr = new BlockChainAddress(_scriptTypes, _network, key);
            return adr;
        }

        private IServiceProvider BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            _assemblyHelperMock = new Mock<IAssemblyHelper>();
            var assm = Assembly.LoadFrom("SimpleBlockChain.Core.dll");
            _assemblyHelperMock.Setup(a => a.GetEntryAssembly()).Returns(assm);
            serviceCollection.AddTransient<IBlockChainFactory, BlockChainFactory>();
            serviceCollection.AddSingleton<IAssemblyHelper>(_assemblyHelperMock.Object);
            return serviceCollection.BuildServiceProvider();
        }

        private void RemoveBlockChain()
        {
            var assm = Assembly.LoadFrom("SimpleBlockChain.Core.dll");
            var path = Path.Combine(Path.GetDirectoryName(assm.Location), string.Format("db_{0}.dat", BlockChain.GetDirectoryName(_network)));
            if (!Directory.Exists(path))
            {
                return;
            }

            Directory.Delete(path, true);
        }
    }
}

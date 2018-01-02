using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Factories;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Transactions;
using SimpleBlockChain.UnitTests.Stores;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SimpleBlockChain.UnitTests.Helpers
{
    [TestClass]
    public class TransactionHelperFixture
    {
        private const Networks _network = Networks.MainNet;
        private const ScriptTypes _scriptTypes = ScriptTypes.P2PKH;
        private Mock<IAssemblyHelper> _assemblyHelperMock;

        [TestMethod]
        public void WhenCalculateReward()
        {
            var serviceProvider = BuildServiceProvider();
            var transactionHelper = serviceProvider.GetService<ITransactionHelper>();
            Assert.IsTrue(transactionHelper.GetMinFee() == 1);
        }
        
        [TestMethod]
        public void WhenCalculateBalance0PTCSLeft()
        {
            RemoveBlockChain();
            var serviceProvider = BuildServiceProvider();
            var transactionHelper = serviceProvider.GetService<ITransactionHelper>();
            var blockChainFactory = serviceProvider.GetService<IBlockChainFactory>();
            var blockChainStore = serviceProvider.GetService<IBlockChainStore>();
            blockChainStore.Switch(_network);
            var blockChain = blockChainStore.GetBlockChain(); // Get the genesis block.
            var genesisBlock = blockChain.GetCurrentBlock();
            var firstTransaction = genesisBlock.Transactions.First();
            var firstTransactionOut = firstTransaction.TransactionOut.First();
            var genesisKey = KeyStore.GetGenesisKey();
            var genesisAdr = new BlockChainAddress(_scriptTypes, _network, genesisKey); // Create block chain address.
            var destinationBlockChainAddress = GenerateBlockChainAddress();

            var signature = genesisKey.GetSignature(); // Create the script.
            var scriptBuilder = new ScriptBuilder();
            var genesisScript = scriptBuilder
                .New()
                .AddToStack(signature)
                .AddToStack(genesisKey.GetPublicKey())
                .Build();
            var destinationScript = Script.CreateP2PKHScript(destinationBlockChainAddress.PublicKeyHash);
            var destGenesisScript = Script.CreateP2PKHScript(genesisKey.GetPublicKeyHashed());

            var transactionBuilder = new TransactionBuilder();
            var noneCoinBaseTransaction = transactionBuilder // ADD GENESIS (10 BTC) => DESTINATION TRANSACTION.
                .NewNoneCoinbaseTransaction()
                .Spend(firstTransaction, 0, genesisScript.Serialize())
                .AddOutput(2, destinationScript)
                .Build();

            var balance = transactionHelper.CalculateBalance(noneCoinBaseTransaction, new[] { genesisAdr }, _network);
            var receiverBalance = transactionHelper.CalculateBalance(noneCoinBaseTransaction, new[] { destinationBlockChainAddress }, _network);

            Assert.IsTrue(balance == 0);
            Assert.IsTrue(receiverBalance == 2);
        }

        [TestMethod]
        public void WhenCalculateBalance48PTCSLeft()
        {
            RemoveBlockChain();
            var serviceProvider = BuildServiceProvider();
            var transactionHelper = serviceProvider.GetService<ITransactionHelper>();
            var blockChainFactory = serviceProvider.GetService<IBlockChainFactory>();
            var blockChainStore = serviceProvider.GetService<IBlockChainStore>();
            blockChainStore.Switch(_network);
            var blockChain = blockChainStore.GetBlockChain(); // Get the genesis block.
            var genesisBlock = blockChain.GetCurrentBlock();
            var firstTransaction = genesisBlock.Transactions.First();
            var firstTransactionOut = firstTransaction.TransactionOut.First();
            var genesisKey = KeyStore.GetGenesisKey();
            var genesisAdr = new BlockChainAddress(_scriptTypes, _network, genesisKey); // Create block chain address.
            var destinationBlockChainAddress = GenerateBlockChainAddress();

            var signature = genesisKey.GetSignature(); // Create the script.
            var scriptBuilder = new ScriptBuilder();
            var genesisScript = scriptBuilder
                .New()
                .AddToStack(signature)
                .AddToStack(genesisKey.GetPublicKey())
                .Build();
            var destinationScript = Script.CreateP2PKHScript(destinationBlockChainAddress.PublicKeyHash);
            var destGenesisScript = Script.CreateP2PKHScript(genesisKey.GetPublicKeyHashed());

            var transactionBuilder = new TransactionBuilder();
            var noneCoinBaseTransaction = transactionBuilder // ADD GENESIS (10 BTC) => DESTINATION TRANSACTION.
                .NewNoneCoinbaseTransaction()
                .Spend(firstTransaction, 0, genesisScript.Serialize())
                .AddOutput(2, destinationScript)
                .AddOutput(48, destGenesisScript)
                .Build();

            var genesisBalance= transactionHelper.CalculateBalance(noneCoinBaseTransaction, new[] { genesisAdr }, _network);
            var receiverBalance = transactionHelper.CalculateBalance(noneCoinBaseTransaction, new[] { destinationBlockChainAddress }, _network);

            Assert.IsTrue(genesisBalance == 48);
            Assert.IsTrue(receiverBalance == 2);
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
            serviceCollection.AddSingleton<IAssemblyHelper>(_assemblyHelperMock.Object);
            serviceCollection.AddTransient<ITransactionHelper, TransactionHelper>();
            serviceCollection.AddSingleton<IBlockChainStore, BlockChainStore>();
            serviceCollection.AddTransient<IBlockChainFactory, BlockChainFactory>();
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

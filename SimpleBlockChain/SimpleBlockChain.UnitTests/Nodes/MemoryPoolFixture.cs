using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Transactions;
using System.Collections.Generic;

namespace SimpleBlockChain.UnitTests.Nodes
{
    [TestClass]
    public class MemoryPoolFixture
    {
        [TestMethod]
        public void WhenCountDescendants()
        {
            var scriptBuilder = new ScriptBuilder();
            var firstClient = BuildBlockChainAddress();
            var secondClient = BuildBlockChainAddress();
            var thirdClient = BuildBlockChainAddress();
            var fourthClient = BuildBlockChainAddress();
            var fifthClient = BuildBlockChainAddress();
            var sixClient = BuildBlockChainAddress();
            var sevenClient = BuildBlockChainAddress();
            var firstClientScript = scriptBuilder
                .New()
                .AddToStack(firstClient.Value.GetSignature())
                .AddToStack(firstClient.Value.GetPublicKey())
                .Build();
            var fifthClientScript = scriptBuilder
                .New()
                .AddToStack(fifthClient.Value.GetSignature())
                .AddToStack(fifthClient.Value.GetPublicKey())
                .Build();
            var builder = new TransactionBuilder();
            var firstTransaction = builder.NewCoinbaseTransaction()
                .AddOutput(20, Script.CreateP2PKHScript(firstClient.Key.PublicKeyHash))
                .AddOutput(10, Script.CreateP2PKHScript(secondClient.Key.PublicKeyHash))
                .Build();
            var secondTransaction = builder.NewNoneCoinbaseTransaction()
                .Spend(firstTransaction, 0, firstClientScript.Serialize())
                .AddOutput(20, Script.CreateP2PKHScript(thirdClient.Key.PublicKeyHash))
                .Build();
            var thirdTransaction = builder.NewNoneCoinbaseTransaction()
                .Spend(firstTransaction, 1, firstClientScript.Serialize())
                .AddOutput(5, Script.CreateP2PKHScript(fourthClient.Key.PublicKeyHash))
                .AddOutput(5, Script.CreateP2PKHScript(fifthClient.Key.PublicKeyHash))
                .Build();
            var fourthTransaction = builder.NewNoneCoinbaseTransaction()
                .Spend(thirdTransaction, 1, fifthClientScript.Serialize())
                .AddOutput(2, Script.CreateP2PKHScript(sixClient.Key.PublicKeyHash))
                .AddOutput(2, Script.CreateP2PKHScript(sevenClient.Key.PublicKeyHash))
                .Build();


            var memPool = MemoryPool.Instance();
            memPool.AddTransaction(firstTransaction, 1);
            memPool.AddTransaction(secondTransaction, 1);
            memPool.AddTransaction(thirdTransaction, 1);
            memPool.AddTransaction(fourthTransaction, 1);

            var firstUnspentTx = memPool.GetUnspentMemoryRecord(firstTransaction.GetTxId());
            var secondUnspentTx = memPool.GetUnspentMemoryRecord(secondTransaction.GetTxId());
            var thirdUnspentTx = memPool.GetUnspentMemoryRecord(thirdTransaction.GetTxId());
            var fourthUnspentTx = memPool.GetUnspentMemoryRecord(fourthTransaction.GetTxId());
            var firstNbDescendants = memPool.CountDescendants(firstUnspentTx);
            var secondNbDescendants = memPool.CountDescendants(secondUnspentTx);
            var thirdNbDescendants = memPool.CountDescendants(thirdUnspentTx);
            var fourthNbDescendants = memPool.CountDescendants(fourthUnspentTx);

            Assert.IsTrue(firstNbDescendants == 4);
            Assert.IsTrue(secondNbDescendants == 1);
            Assert.IsTrue(thirdNbDescendants == 2);
            Assert.IsTrue(fourthNbDescendants == 1);
        }

        [TestMethod]
        public void WhenCountAncestors()
        {
            var scriptBuilder = new ScriptBuilder();
            var firstClient = BuildBlockChainAddress();
            var secondClient = BuildBlockChainAddress();
            var thirdClient = BuildBlockChainAddress();
            var fourthClient = BuildBlockChainAddress();
            var fifthClient = BuildBlockChainAddress();
            var sixClient = BuildBlockChainAddress();
            var sevenClient = BuildBlockChainAddress();
            var firstClientScript = scriptBuilder
                .New()
                .AddToStack(firstClient.Value.GetSignature())
                .AddToStack(firstClient.Value.GetPublicKey())
                .Build();
            var fifthClientScript = scriptBuilder
                .New()
                .AddToStack(fifthClient.Value.GetSignature())
                .AddToStack(fifthClient.Value.GetPublicKey())
                .Build();
            var builder = new TransactionBuilder();
            var firstTransaction = builder.NewCoinbaseTransaction()
                .AddOutput(20, Script.CreateP2PKHScript(firstClient.Key.PublicKeyHash))
                .AddOutput(10, Script.CreateP2PKHScript(secondClient.Key.PublicKeyHash))
                .Build();
            var secondTransaction = builder.NewNoneCoinbaseTransaction()
                .Spend(firstTransaction, 0, firstClientScript.Serialize())
                .AddOutput(20, Script.CreateP2PKHScript(thirdClient.Key.PublicKeyHash))
                .Build();
            var thirdTransaction = builder.NewNoneCoinbaseTransaction()
                .Spend(firstTransaction, 1, firstClientScript.Serialize())
                .AddOutput(5, Script.CreateP2PKHScript(fourthClient.Key.PublicKeyHash))
                .AddOutput(5, Script.CreateP2PKHScript(fifthClient.Key.PublicKeyHash))
                .Build();
            var fourthTransaction = builder.NewNoneCoinbaseTransaction()
                .Spend(thirdTransaction, 1, fifthClientScript.Serialize())
                .AddOutput(2, Script.CreateP2PKHScript(sixClient.Key.PublicKeyHash))
                .AddOutput(2, Script.CreateP2PKHScript(sevenClient.Key.PublicKeyHash))
                .Build();


            var memPool = MemoryPool.Instance();
            memPool.AddTransaction(firstTransaction, 1);
            memPool.AddTransaction(secondTransaction, 1);
            memPool.AddTransaction(thirdTransaction, 1);
            memPool.AddTransaction(fourthTransaction, 1);

            var firstUnspentTx = memPool.GetUnspentMemoryRecord(firstTransaction.GetTxId());
            var secondUnspentTx = memPool.GetUnspentMemoryRecord(secondTransaction.GetTxId());
            var thirdUnspentTx = memPool.GetUnspentMemoryRecord(thirdTransaction.GetTxId());
            var fourthUnspentTx = memPool.GetUnspentMemoryRecord(fourthTransaction.GetTxId());
            var firstNbDescendants = memPool.CountAncestors(firstUnspentTx);
            var secondNbDescendants = memPool.CountAncestors(secondUnspentTx);
            var thirdNbDescendants = memPool.CountAncestors(thirdUnspentTx);
            var fourthNbDescendants = memPool.CountAncestors(fourthUnspentTx);

            Assert.IsTrue(firstNbDescendants == 1);
            Assert.IsTrue(secondNbDescendants == 2);
            Assert.IsTrue(thirdNbDescendants == 2);
            Assert.IsTrue(fourthNbDescendants == 3);
        }

        private static KeyValuePair<BlockChainAddress, Key> BuildBlockChainAddress()
        {
            var network = Networks.MainNet;
            var key = Key.Genererate();
            var blockChainAddress = new BlockChainAddress(ScriptTypes.P2PKH, network, key);
            var hash = blockChainAddress.GetSerializedHash();
            var deserializedBA = BlockChainAddress.Deserialize(hash);
            return new KeyValuePair<BlockChainAddress, Key>(deserializedBA, key);
        }
    }
}

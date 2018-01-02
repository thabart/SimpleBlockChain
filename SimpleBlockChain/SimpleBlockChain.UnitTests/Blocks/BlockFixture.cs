using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Math;
using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Linq;

namespace SimpleBlockChain.UnitTests.Blocks
{
    [TestClass]
    public class BlockFixture
    {
        private static uint NBits = 0x1e0ffff0;

        [TestMethod]
        public void WhenSerializeGenesisBlock()
        {
            var genesisBlock = Block.BuildGenesisBlock();
            var firstHeader = genesisBlock.SerializeHeader();
            var firstHashHeader = genesisBlock.GetHashHeader();
            var payload = genesisBlock.Serialize();

            var deserializedGenesisBlock = Block.Deserialize(payload);
            var secondHeader = deserializedGenesisBlock.SerializeHeader();
            var secondHashHeader = deserializedGenesisBlock.GetHashHeader();

            Assert.IsTrue(firstHeader.Count() == 80);
            Assert.IsTrue(secondHeader.Count() == 80);
            Assert.IsTrue(firstHashHeader.SequenceEqual(secondHashHeader));
            Assert.IsTrue(firstHeader.SequenceEqual(secondHeader));
        }

        [TestMethod]
        public void WhenSerializeBlockWithOneCoinbaseTransaction()
        {
            var ba = BuildBlockChainAddress();
            var builder = new TransactionBuilder();
            var nonce = BitConverter.GetBytes(NonceHelper.GetNonceUInt64());
            var transaction = builder.NewCoinbaseTransaction()
                .SetInput(4, nonce)
                .AddOutput(20, Script.CreateP2PKHScript(ba.PublicKeyHash))
                .Build();

            var block = new Block(null, NBits, NonceHelper.GetNonceUInt32());
            block.Transactions.Add(transaction);
            var serializedBlock = block.Serialize();

            var des = Block.Deserialize(serializedBlock);
            string s = "";
        }

        [TestMethod]
        public void WhenSerializeBlockWithOneCoinbaseTransactionAndTwoNoneCoinbaseTransaction()
        {
            var ba = BuildBlockChainAddress();
            var builder = new TransactionBuilder();
            var nonce = BitConverter.GetBytes(NonceHelper.GetNonceUInt64());
            var transaction = builder.NewCoinbaseTransaction()
                .SetInput(4, nonce)
                .AddOutput(20, Script.CreateP2PKHScript(ba.PublicKeyHash))
                .Build();
            var secondTransaction = builder.NewNoneCoinbaseTransaction()
                .AddOutput(10, Script.CreateP2PKHScript(ba.PublicKeyHash))
                .Build();
            var thirdTransaction = builder.NewNoneCoinbaseTransaction()
                .AddOutput(11, Script.CreateP2PKHScript(ba.PublicKeyHash))
                .Build();

            var block = new Block(null, NBits, NonceHelper.GetNonceUInt32());
            block.Transactions.Add(thirdTransaction);
            block.Transactions.Add(secondTransaction);
            block.Transactions.Add(transaction);

            block.Serialize();
        }

        [TestMethod]
        public void WhenBuildGenesisBlock()
        {
            var block = Block.BuildGenesisBlock();
            var payload = block.Serialize();
            var deserialized = Block.Deserialize(payload);
            Assert.IsTrue(true);
        }

        private static BlockChainAddress BuildBlockChainAddress()
        {
            var network = Networks.MainNet;
            var key = Key.Deserialize(new BigInteger("66661394595692466950200829442443674598224300882267065208709422638481412972116609477112206002430829808784107536250360432119209033266013484787698545014625057"), new BigInteger("43102461949956883352376427470284148089747996528740865531180015053863743793176")); //Key.Genererate();

            var k2 = Key.Genererate();
            var publicKey = new BigInteger(k2.GetPublicKey().ToArray());
            var privateKey = k2.GetPrivateKey();
            var keyHash = new BigInteger(k2.GetPublicKeyHashed().ToArray());
            var blockChainAddress2 = new BlockChainAddress(ScriptTypes.P2PKH, network, k2);
            var hh = blockChainAddress2.GetSerializedHash();

            var h = new BigInteger(key.GetPublicKeyHashed());
            var blockChainAddress = new BlockChainAddress(ScriptTypes.P2PKH, network, key);
            var s = blockChainAddress.GetJson().ToString();
            var hash = blockChainAddress.GetSerializedHash();
            var deserializedBA = BlockChainAddress.Deserialize(hash);
            return deserializedBA;
        }
    }
}

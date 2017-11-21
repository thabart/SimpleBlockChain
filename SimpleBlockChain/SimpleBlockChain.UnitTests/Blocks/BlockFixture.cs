using NUnit.Framework;
using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Transactions;
using System;

namespace SimpleBlockChain.UnitTests.Blocks
{
    [TestFixture]
    public class BlockFixture
    {
        private static uint NBits = 0x1e0ffff0;

        [Test]
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

        [Test]
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

        private static BlockChainAddress BuildBlockChainAddress()
        {
            var network = Networks.MainNet;
            var key = new Key();
            var blockChainAddress = new BlockChainAddress(ScriptTypes.P2PKH, network, key);
            var hash = blockChainAddress.GetSerializedHash();
            var deserializedBA = BlockChainAddress.Deserialize(hash);
            return deserializedBA;
        }
    }
}

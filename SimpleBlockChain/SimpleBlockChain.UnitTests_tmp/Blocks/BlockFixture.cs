using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Transactions;
using System;
using Xunit;

namespace SimpleBlockChain.UnitTests.Blocks
{
    public class BlockFixture
    {
        private static uint NBits = 0x1e0ffff0;

        [Fact]
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

        [Fact]
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

        [Fact]
        public void WhenBuildGenesisBlock()
        {
            var block = Block.BuildGenesisBlock();
            var payload = block.Serialize();
            var deserialized = Block.Deserialize(payload);
            Assert.True(true);
        }

        private static BlockChainAddress BuildBlockChainAddress()
        {
            var network = Networks.MainNet;
            var key = Key.Genererate();
            var blockChainAddress = new BlockChainAddress(ScriptTypes.P2PKH, network, key);
            var hash = blockChainAddress.GetSerializedHash();
            var deserializedBA = BlockChainAddress.Deserialize(hash);
            return deserializedBA;
        }
    }
}

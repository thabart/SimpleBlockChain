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
    public class BlockChainFixture
    {
        private static uint NBits = 0x1e0ffff0;

        [Test]
        public void WhenAddBlock()
        {
            var firstBlock = BuildBlock(1, 10);
            var secondBlock = BuildBlock(1, 12);
        }

        private static Block BuildBlock(long basecoinOutput, long noneBasecoinOutput)
        {
            var ba = BuildBlockChainAddress();
            var builder = new TransactionBuilder();
            var nonce = BitConverter.GetBytes(NonceHelper.GetNonceUInt64());
            var coinBaseTransaction = builder.NewCoinbaseTransaction()
                .SetInput(4, nonce)
                .AddOutput(basecoinOutput, Script.CreateP2PKHScript(ba.PublicKeyHash))
                .Build();
            var noneCoinBaseTransaction = builder.NewNoneCoinbaseTransaction()
                .AddOutput(noneBasecoinOutput, Script.CreateP2PKHScript(ba.PublicKeyHash))
                .Build();

            var block = new Block(null, NBits, NonceHelper.GetNonceUInt32());
            block.Transactions.Add(coinBaseTransaction);
            block.Transactions.Add(noneCoinBaseTransaction);
            return block;
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

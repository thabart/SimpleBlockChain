using NUnit.Framework;
using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Transactions;
using System;

namespace SimpleBlockChain.UnitTests.Builders
{
    [TestFixture]
    public class TransactionBuilderFixture
    {
        [Test]
        public void WhenBuildNoneCoinbaseTransaction()
        {
            var ba = BuildBlockChainAddress();
            var builder = new TransactionBuilder();
           var transaction = builder.NewNoneCoinbaseTransaction()
                .AddOutput(20, Script.CreateP2PKHScript(ba.PublicKeyHash))
                .Build();
            var serializedTransaction = transaction.Serialize();
            var deserializedTransaction = BaseTransaction.Deserialize(serializedTransaction, TransactionTypes.NoneCoinbase);
        }

        [Test]
        public void WhenBuildCoinbaseTransaction()
        {
            var ba = BuildBlockChainAddress();
            var builder = new TransactionBuilder();
            var nonce = BitConverter.GetBytes(NonceHelper.GetNonceUInt64());
            var transaction = builder.NewCoinbaseTransaction()
                .SetInput(4, nonce)
                .AddOutput(20, Script.CreateP2PKHScript(ba.PublicKeyHash))
                .Build();
            var serializedTransaction = transaction.Serialize();
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

using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Transactions;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleBlockChain.Core.Compiler;

namespace SimpleBlockChain.UnitTests.Builders
{
    [TestClass]
    public class TransactionBuilderFixture
    {
        [TestMethod]
        public void WhenBuildNoneCoinbaseTransaction()
        {
            var ba = BuildBlockChainAddress();
            var builder = new TransactionBuilder();
           var transaction = builder.NewNoneCoinbaseTransaction()
                .AddOutput(20, Script.CreateP2PKHScript(ba.PublicKeyHash))
                .Build();
            var serializedTransaction = transaction.Serialize();
            var deserializedTransaction = BcBaseTransaction.Deserialize(serializedTransaction);
        }

        [TestMethod]
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

        /*
        [TestMethod]
        public void WhenBuildSmartContractTransaction()
        {
            var code = @"
                using System;
                public class SimpleTest
                {
                    private string y {get; set;}
                    private string z;
                    public string Test2(string parameter)
                    {
                        return parameter;
                    }
                    private string Test()
                    {
                        return ""1"";
                    }
                }";
            var compiler = new DotnetCompiler();
            var smartContract = compiler.Compile(code);
            var ba = BuildBlockChainAddress();
            var builder = new TransactionBuilder();
            var script = Script.CreateP2PKHScript(ba.PublicKeyHash);
            var nonce = BitConverter.GetBytes(NonceHelper.GetNonceUInt64());
            var transaction = builder
                .NewSmartContractTransaction().CreateSmartContract(smartContract, script, "thabart", "test", 0, nonce)
                .Build();
            var serializedTransaction = transaction.Serialize();
            var deserializedTx = BaseTransaction.Deserialize(serializedTransaction, TransactionTypes.SmartContract);
            string s = "";
        }
        */

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

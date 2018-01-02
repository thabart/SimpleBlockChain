using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Compiler;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Scripts;
using SimpleBlockChain.Core.Transactions;
using SimpleBlockChain.UnitTests.Stores;
using System.Text;

namespace SimpleBlockChain.UnitTests.Builders
{
    [TestClass]
    public class ScriptBuilderFixture
    {
        [TestMethod]
        public void Check16EqualTo16()
        {
            var scriptBuilder = new ScriptBuilder();
            var inputScript = scriptBuilder.New()
                .AddOperation(OpCodes.OP_1)
                .Build();
            var outputScript = scriptBuilder.New()
                .AddOperation(OpCodes.OP_15)
                .AddOperation(OpCodes.OP_ADD)
                .AddOperation(OpCodes.OP_16)
                .AddOperation(OpCodes.OP_EQUAL)
                .AddOperation(OpCodes.OP_VERIFY)
                .Build();

            var serializedInputScript = inputScript.Serialize();
            var serializedOutputScript = outputScript.Serialize();

            var deserializedInputScript = Script.Deserialize(serializedInputScript);
            var deserializedOutputScript = Script.Deserialize(serializedOutputScript);

            var interpreter = new ScriptInterpreter();
            bool isCorrect = interpreter.Check(deserializedInputScript, deserializedOutputScript);

            Assert.IsTrue(isCorrect);
        }

        [TestMethod]
        public void CheckSignature()
        {
            var key = Key.Genererate();
            var publicKeyHash = key.GetPublicKeyHashed();
            var scriptBuilder = new ScriptBuilder();
            var payload = Encoding.UTF8.GetBytes("sss");
            var signature = key.Sign(payload);
            var inputScript = scriptBuilder.New()
                .AddToStack(signature)
                .AddToStack(key.GetPublicKey())
                .Build();
            var outputScript = scriptBuilder.New()
                .AddOperation(OpCodes.OP_DUP)
                .AddOperation(OpCodes.OP_HASH160)
                .AddToStack(publicKeyHash)
                .AddOperation(OpCodes.OP_EQUALVERIFY)
                .AddOperation(OpCodes.OP_CHECKSIG)
                .Build();

            var serializedInputScript = inputScript.Serialize();
            var serializedOutputScript = outputScript.Serialize();

            var deserializedInputScript = Script.Deserialize(serializedInputScript);
            var deserializedOutputScript = Script.Deserialize(serializedOutputScript);

            var interpreter = new ScriptInterpreter();
            bool isCorrect = interpreter.Check(deserializedInputScript, deserializedOutputScript);

            Assert.IsTrue(isCorrect);
        }

        [TestMethod]
        public void CheckClientSignature()
        {
            var clientKey = KeyStore.GetClientKey();
            var clientWallet = BlockChainAddress.Deserialize("12K5LnVWKCu9QGyB39uGAgVSAfBs33PKS96HSL93");
            var scriptBuilder = new ScriptBuilder();
            var inputScript = scriptBuilder.New()
                .AddToStack(clientKey.GetSignature())
                .AddToStack(clientKey.GetPublicKey())
                .Build();
            var outputScript = scriptBuilder.New()
                .AddOperation(OpCodes.OP_DUP)
                .AddOperation(OpCodes.OP_HASH160)
                .AddToStack(clientWallet.PublicKeyHash)
                .AddOperation(OpCodes.OP_EQUALVERIFY)
                .AddOperation(OpCodes.OP_CHECKSIG)
                .Build();

            var serializedInputScript = inputScript.Serialize();
            var serializedOutputScript = outputScript.Serialize();

            var deserializedInputScript = Script.Deserialize(serializedInputScript);
            var deserializedOutputScript = Script.Deserialize(serializedOutputScript);

            var interpreter = new ScriptInterpreter();
            bool isCorrect = interpreter.Check(deserializedInputScript, deserializedOutputScript);

            Assert.IsTrue(isCorrect);
        }
    }
}

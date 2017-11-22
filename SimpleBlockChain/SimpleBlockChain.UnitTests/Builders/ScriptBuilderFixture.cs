using NUnit.Framework;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Scripts;
using SimpleBlockChain.Core.Transactions;
using System.Text;

namespace SimpleBlockChain.UnitTests.Builders
{
    [TestFixture]
    public class ScriptBuilderFixture
    {
        [Test]
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

            var interpreter = new Interpreter();
            bool isCorrect = interpreter.Check(deserializedInputScript, deserializedOutputScript);

            Assert.True(isCorrect);
        }

        [Test]
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

            var interpreter = new Interpreter();
            bool isCorrect = interpreter.Check(deserializedInputScript, deserializedOutputScript);

            Assert.True(isCorrect);
        }
    }
}

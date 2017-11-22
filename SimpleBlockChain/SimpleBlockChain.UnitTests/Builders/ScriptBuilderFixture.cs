using NUnit.Framework;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Scripts;
using SimpleBlockChain.Core.Transactions;

namespace SimpleBlockChain.UnitTests.Builders
{
    [TestFixture]
    public class ScriptBuilderFixture
    {
        [Test]
        public void WhenCreateScript()
        {
            var scriptBuilder = new ScriptBuilder();
            var inputScript = scriptBuilder.New()
                .AddOperation(OpCodes.OP_1)
                .Build();
            var outputScript = scriptBuilder.New()
                .AddOperation(OpCodes.OP_15)
                .AddOperation(OpCodes.OP_ADD)
                .AddOperation(OpCodes.OP_16)
                .AddOperation(OpCodes.OP_EQUAL).
                Build();

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

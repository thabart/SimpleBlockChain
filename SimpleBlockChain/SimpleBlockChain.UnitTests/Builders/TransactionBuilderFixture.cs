using NUnit.Framework;
using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Transactions;

namespace SimpleBlockChain.UnitTests.Builders
{
    [TestFixture]
    public class TransactionBuilderFixture
    {
        public void WhenBuilderNoneCoinbaseTransaction()
        {
            var network = Networks.MainNet;
            var key = new Key();
            var blockChainAddress = new BlockChainAddress(ScriptTypes.P2PKH, network, key);
            var builder = new TransactionBuilder();

           var transaction = builder.NewCoinbaseTransaction()
                .NewNoneCoinbaseTransaction()
                .AddOutput(20, Script.CreateP2PKHScript(blockChainAddress.PublicKeyHash))
                .Build();

        }
    }
}

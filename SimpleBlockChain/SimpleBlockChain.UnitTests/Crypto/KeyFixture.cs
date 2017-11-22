using NUnit.Framework;
using SimpleBlockChain.Core.Crypto;
using System.Text;

namespace SimpleBlockChain.UnitTests.Crypto
{
    [TestFixture]
    public class KeyFixture
    {
        [Test]
        public void WhenCheckSignatureThenTrueIsReturned()
        {
            var key = Key.Genererate();
            var payload = Encoding.UTF8.GetBytes("sss");
            var signature = key.Sign(payload);
            Assert.True(key.CheckSignature(payload, signature));

            var k = key.GetPublicKey();
            var nk = Key.Deserialize(k);
            var b =  nk.CheckSignature(payload, signature);
            Assert.True(b);
        }
    }
}

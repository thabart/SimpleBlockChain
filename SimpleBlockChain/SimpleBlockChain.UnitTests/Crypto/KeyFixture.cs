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
            var key = new Key();
            var payload = Encoding.UTF8.GetBytes("sss");
            var signature = key.Sign(payload);
            Assert.True(key.CheckSignature(payload, signature));
        }
    }
}

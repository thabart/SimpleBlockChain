﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleBlockChain.Core.Crypto;
using System.Linq;
using System.Numerics;
using System.Text;

namespace SimpleBlockChain.UnitTests.Crypto
{
    [TestClass]
    public class KeyFixture
    {
        [TestMethod]
        public void WhenCheckSignatureThenTrueIsReturned()
        {
            var key = Key.Genererate();
            var pk = new BigInteger(key.GetPublicKey().ToArray());
            var payload = Encoding.UTF8.GetBytes("sss");
            var signature = key.Sign(payload);
            Assert.IsTrue(key.CheckSignature(payload, signature));

            var k = key.GetPublicKey();
            var nk = Key.Deserialize(k);
            var b =  nk.CheckSignature(payload, signature);
            Assert.IsTrue(b);
        }
    }
}

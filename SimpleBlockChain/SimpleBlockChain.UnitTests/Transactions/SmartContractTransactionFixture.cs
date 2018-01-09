using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Transactions;
using System.Linq;

namespace SimpleBlockChain.UnitTests.Transactions
{
    [TestClass]
    public class SmartContractTransactionFixture
    {
        [TestMethod]
        public void WhenGetSmartContractAddr()
        {
            var adr = "6ac7ea33f8831ea9dcc53393aaa88b25a785dbf0";
            var transaction = new SmartContractTransaction();
            transaction.From = adr.FromHexString();
            transaction.Nonce = 1;

            var res = transaction.GetSmartContractAddress();

            Assert.IsTrue(res.Count() == 20);
        }
    }
}

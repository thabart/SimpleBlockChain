using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Stores;
using System;

namespace SimpleBlockChain.UnitTests.Helpers
{
    [TestClass]
    public class TransactionHelperFixture
    {
        private Mock<IBlockChainStore> _blockChainStoreMock;

        [TestMethod]
        public void WhenCalculateReward()
        {
            var serviceProvider = BuildServiceProvider();
            var transactionHelper = serviceProvider.GetService<ITransactionHelper>();
            Assert.IsTrue(transactionHelper.GetMinFee() == 1);
        }

        private IServiceProvider BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            _blockChainStoreMock = new Mock<IBlockChainStore>();
            serviceCollection.AddSingleton<IBlockChainStore>(_blockChainStoreMock.Object);
            serviceCollection.AddTransient<ITransactionHelper, TransactionHelper>();
            return serviceCollection.BuildServiceProvider();
        }
    }
}

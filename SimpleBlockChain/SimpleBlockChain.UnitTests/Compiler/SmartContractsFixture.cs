using HashLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Compiler;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Factories;
using SimpleBlockChain.Core.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SimpleBlockChain.UnitTests.Compiler
{
    [TestClass]
    public class SmartContractsFixture
    {
        private Mock<IAssemblyHelper> _assemblyHelperMock;
        private const Networks _network = Networks.MainNet;

        [TestMethod]
        public void WhenExecutePrecompiledContractThenHelloWorldIsDisplayed()
        {
            const string operationSignature = "get()";
            var hash = HashFactory.Crypto.SHA3.CreateKeccak256();
            var msgData = hash.ComputeBytes(System.Text.Encoding.ASCII.GetBytes(operationSignature)).GetBytes().Take(4);
            var pgInvoke = new SolidityProgramInvoke(msgData, new DataWord(), new DataWord());
            RemoveSmartContracts();
            var serviceProvider = BuildServiceProvider();
            var smartContractFactory = serviceProvider.GetService<ISmartContractFactory>();
            var smartContracts = smartContractFactory.Build(_network);

            var contract = smartContracts.GetSmartContract("0000000000000000000000000000000000000001".FromHexString());
            
            var program = new SolidityProgram(contract.Code.ToList(), pgInvoke);
            var vm = new SolidityVm();
            while (!program.IsStopped())
            {
                vm.Step(program);
            }

            var hResult = program.GetResult().GetHReturn();
            var res = hResult.ToHexString();
            Assert.IsTrue(res == "0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000b68656c6c6f20776f726c64000000000000000000000000000000000000000000");
        }

        private IServiceProvider BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            _assemblyHelperMock = new Mock<IAssemblyHelper>();
            var assm = Assembly.LoadFrom("SimpleBlockChain.Core.dll");
            _assemblyHelperMock.Setup(a => a.GetEntryAssembly()).Returns(assm);
            serviceCollection.AddTransient<ISmartContractFactory, SmartContractFactory>();
            serviceCollection.AddSingleton<IAssemblyHelper>(_assemblyHelperMock.Object);
            return serviceCollection.BuildServiceProvider();
        }

        private void RemoveSmartContracts()
        {
            var assm = Assembly.LoadFrom("SimpleBlockChain.Core.dll");
            var path = Path.Combine(Path.GetDirectoryName(assm.Location), string.Format("db_{0}.dat", SmartContracts.GetDirectoryName(_network)));
            if (!Directory.Exists(path))
            {
                return;
            }

            Directory.Delete(path, true);
        }
    }
}

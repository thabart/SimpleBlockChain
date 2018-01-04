using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleBlockChain.Core.Compiler;
using SimpleBlockChain.Core.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.UnitTests.Compiler
{
    [TestClass]
    public class SolidityVmFixture
    {
        [TestMethod]
        public void WhenCallDataLoad1()
        {
            string code = "600035";
            var payload = code.FromHexString().ToList();
            IEnumerable<byte> msgData = ("00000000000000000000000000000000000000000000000000000000000000A1" +
                "00000000000000000000000000000000000000000000000000000000000000B1").FromHexString().ToList();
            var pgInvoke = new SolidityProgramInvoke(msgData);
            var program = new SolidityProgram(payload, pgInvoke);
            var vm = new SolidityVm();

            vm.Step(program);
            vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "00000000000000000000000000000000000000000000000000000000000000A1");
        }

        [TestMethod]
        public void WhenCallDataLoad2()
        {
            string code = "600235";
            var payload = code.FromHexString().ToList();
            IEnumerable<byte> msgData = ("00000000000000000000000000000000000000000000000000000000000000A1" +
                "00000000000000000000000000000000000000000000000000000000000000B1").FromHexString().ToList();
            var pgInvoke = new SolidityProgramInvoke(msgData);
            var program = new SolidityProgram(payload, pgInvoke);
            var vm = new SolidityVm();

            vm.Step(program);
            vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000A10000");
        }

        [TestMethod]
        public void WhenCallDataLoad3()
        {
            string code = "602035";
            var payload = code.FromHexString().ToList();
            IEnumerable<byte> msgData = ("00000000000000000000000000000000000000000000000000000000000000A1" +
                "00000000000000000000000000000000000000000000000000000000000000B1").FromHexString().ToList();
            var pgInvoke = new SolidityProgramInvoke(msgData);
            var program = new SolidityProgram(payload, pgInvoke);
            var vm = new SolidityVm();

            vm.Step(program);
            vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "00000000000000000000000000000000000000000000000000000000000000B1");
        }

        [TestMethod]
        public void WhenCallDataLoad4()
        {
            string code = "602335";
            var payload = code.FromHexString().ToList();
            IEnumerable<byte> msgData = ("00000000000000000000000000000000000000000000000000000000000000A1" +
                "00000000000000000000000000000000000000000000000000000000000000B1").FromHexString().ToList();
            var pgInvoke = new SolidityProgramInvoke(msgData);
            var program = new SolidityProgram(payload, pgInvoke);
            var vm = new SolidityVm();

            vm.Step(program);
            vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "00000000000000000000000000000000000000000000000000000000B1000000");
        }

        [TestMethod]
        public void WhenCallDataLoad5()
        {
            string code = "603F35";
            var payload = code.FromHexString().ToList();
            IEnumerable<byte> msgData = ("00000000000000000000000000000000000000000000000000000000000000A1" +
                "00000000000000000000000000000000000000000000000000000000000000B1").FromHexString().ToList();
            var pgInvoke = new SolidityProgramInvoke(msgData);
            var program = new SolidityProgram(payload, pgInvoke);
            var vm = new SolidityVm();

            vm.Step(program);
            vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "B100000000000000000000000000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void WhenCallDataCopy1()
        {
            string code = "60206000600037";
            var payload = code.FromHexString().ToList();
            IEnumerable<byte> msgData = ("00000000000000000000000000000000000000000000000000000000000000A1" +
                "00000000000000000000000000000000000000000000000000000000000000B1").FromHexString().ToList();
            var pgInvoke = new SolidityProgramInvoke(msgData);
            var program = new SolidityProgram(payload, pgInvoke);
            var vm = new SolidityVm();

            vm.Step(program);
            vm.Step(program);
            vm.Step(program);
            vm.Step(program);

            var pop = program.GetMemory().First().ToHexString().ToUpper();
            Assert.IsTrue(pop == "00000000000000000000000000000000000000000000000000000000000000A1");
        }
    }
}

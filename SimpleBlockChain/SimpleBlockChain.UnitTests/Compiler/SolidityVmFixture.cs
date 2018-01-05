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
        private SolidityProgramInvoke _pgInvoke;
        private SolidityVm _vm;
        private static string _adr = "77045E71A7A2C50903D88E564CD72FAB11E82051";

        [TestInitialize]
        public void Init()
        {
            var address = new DataWord(_adr.FromHexString().ToArray());
            IEnumerable<byte> msgData = ("00000000000000000000000000000000000000000000000000000000000000A1" +
                "00000000000000000000000000000000000000000000000000000000000000B1").FromHexString().ToList();
            _pgInvoke = new SolidityProgramInvoke(msgData, address);
            _vm = new SolidityVm();
        }

        [TestMethod]
        public void WhenCallDataLoad1()
        {
            string code = "600035";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "00000000000000000000000000000000000000000000000000000000000000A1");
        }

        [TestMethod]
        public void WhenCallDataLoad2()
        {
            string code = "600235";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000A10000");
        }

        [TestMethod]
        public void WhenCallDataLoad3()
        {
            string code = "602035";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "00000000000000000000000000000000000000000000000000000000000000B1");
        }

        [TestMethod]
        public void WhenCallDataLoad4()
        {
            string code = "602335";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "00000000000000000000000000000000000000000000000000000000B1000000");
        }

        [TestMethod]
        public void WhenCallDataLoad5()
        {
            string code = "603F35";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "B100000000000000000000000000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void WhenCallDataCopy1()
        {
            string code = "60206000600037";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.GetMemory().First().ToHexString().ToUpper();
            Assert.IsTrue(pop == "00000000000000000000000000000000000000000000000000000000000000A1");
        }

        [TestMethod]
        public void WhenCallDataCopy2()
        {
            string code = "60406000600037";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.GetMemory().First().ToHexString().ToUpper();
            Assert.IsTrue(pop == "00000000000000000000000000000000000000000000000000000000000000A100000000000000000000000000000000000000000000000000000000000000B1");
        }
        
        [TestMethod]
        public void WhenCallDataCopy3()
        {
            string code = "60406004600037";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.GetMemory().First().ToHexString().ToUpper();
            Assert.IsTrue(pop == "000000000000000000000000000000000000000000000000000000A100000000000000000000000000000000000000000000000000000000000000B100000000");
        }

        [TestMethod]
        public void WhenAddress1()
        {
            string code = "30";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "00000000000000000000000077045E71A7A2C50903D88E564CD72FAB11E82051");
        }

        [TestMethod]
        public void WhenPush1()
        {
            string code = "60A0";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "00000000000000000000000000000000000000000000000000000000000000A0");
        }

        [TestMethod]
        public void WhenPush2()
        {
            string code = "61A0B0";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "000000000000000000000000000000000000000000000000000000000000A0B0");
        }

        [TestMethod]
        public void WhenPush3()
        {
            string code = "62A0B0C0";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000A0B0C0");
        }

        [TestMethod]
        public void WhenPush4()
        {
            string code = "63A0B0C0D0";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "00000000000000000000000000000000000000000000000000000000A0B0C0D0");
        }

        [TestMethod]
        public void WhenPush5()
        {
            string code = "64A0B0C0D0E0";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "000000000000000000000000000000000000000000000000000000A0B0C0D0E0");
        }

        [TestMethod]
        public void WhenPush6()
        {
            string code = "65A0B0C0D0E0F0";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000A0B0C0D0E0F0");
        }

        [TestMethod]
        public void WhenPush7()
        {
            string code = "66A0B0C0D0E0F0A1";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "00000000000000000000000000000000000000000000000000A0B0C0D0E0F0A1");
        }

        [TestMethod]
        public void WhenPush8()
        {
            string code = "67A0B0C0D0E0F0A1B1";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "000000000000000000000000000000000000000000000000A0B0C0D0E0F0A1B1");
        }

        [TestMethod]
        public void WhenPush9()
        {
            string code = "68A0B0C0D0E0F0A1B1C1";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000A0B0C0D0E0F0A1B1C1");
        }

        [TestMethod]
        public void WhenPush10()
        {
            string code = "69A0B0C0D0E0F0A1B1C1D1";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "00000000000000000000000000000000000000000000A0B0C0D0E0F0A1B1C1D1");
        }

        [TestMethod]
        public void WhenPush11()
        {
            string code = "6AA0B0C0D0E0F0A1B1C1D1E1";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "000000000000000000000000000000000000000000A0B0C0D0E0F0A1B1C1D1E1");
        }

        [TestMethod]
        public void WhenPush12()
        {
            string code = "6BA0B0C0D0E0F0A1B1C1D1E1F1";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000A0B0C0D0E0F0A1B1C1D1E1F1");
        }

        [TestMethod]
        public void WhenAnd1()
        {
            string code = "600A600A16";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "000000000000000000000000000000000000000000000000000000000000000A");
        }

        [TestMethod]
        public void WhenAnd2()
        {
            string code = "60C0600A16";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void WhenOr1()
        {
            string code = "60F0600F17";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "00000000000000000000000000000000000000000000000000000000000000FF");
        }

        [TestMethod]
        public void WhenOr2()
        {
            string code = "60C3603C17";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "00000000000000000000000000000000000000000000000000000000000000FF");
        }

        [TestMethod]
        public void WhenXOR1()
        {
            string code = "60FF60FF18";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void WhenXOR2()
        {
            string code = "600F60F018";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "00000000000000000000000000000000000000000000000000000000000000FF");
        }

        [TestMethod]
        public void WhenByte1()
        {
            string code = "65AABBCCDDEEFF601E1A";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "00000000000000000000000000000000000000000000000000000000000000EE");
        }

        [TestMethod]
        public void WhenByte2()
        {
            string code = "65AABBCCDDEEFF60201A";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void WhenByte3()
        {
            string code = "65AABBCCDDEE3A601F1A";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "000000000000000000000000000000000000000000000000000000000000003A");
        }

        [TestMethod]
        public void WhenIsZero1()
        {

        }
    }
}

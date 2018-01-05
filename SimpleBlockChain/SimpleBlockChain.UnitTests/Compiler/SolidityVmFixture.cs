using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleBlockChain.Core.Compiler;
using SimpleBlockChain.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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
            var callValue = new DataWord("0DE0B6B3A7640000".FromHexString().ToArray());
            IEnumerable<byte> msgData = ("00000000000000000000000000000000000000000000000000000000000000A1" +
                "00000000000000000000000000000000000000000000000000000000000000B1").FromHexString().ToList();
            _pgInvoke = new SolidityProgramInvoke(msgData, address, callValue);
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

            var pop = program.GetMemory().ToHexString().ToUpper();
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

            var pop = program.GetMemory().ToHexString().ToUpper();
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

            var pop = program.GetMemory().ToHexString().ToUpper();
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

            var pop = program.GetMemory().ToHexString().ToUpper();
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
            string code = "600015";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000001");
        }

        [TestMethod]
        public void WhenIsZero2()
        {
            string code = "602A15";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void WhenEq1()
        {
            string code = "602A602A14";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000001");
        }

        [TestMethod]
        public void WhenEq2()
        {
            string code = "622A3B4C622A3B4C14";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000001");
        }

        [TestMethod]
        public void WhenEq3()
        {
            string code = "622A3B5C622A3B4C14";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void WhenGt1()
        {
            string code = "6001600211";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000001");
        }

        [TestMethod]
        public void WhenGt2()
        {
            string code = "6001610F0011";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000001");
        }

        [TestMethod]
        public void WhenGt3()
        {
            string code = "6301020304610F0011";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void WhenSgt1()
        {
            string code = "6001600213";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000001");
        }

        [TestMethod]
        public void WhenSgt2()
        {
            string code = "7F000000000000000000000000000000000000000000000000000000000000001E" + // 30
                "7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF56" + // -170
                "13";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void WhenSgt3()
        {
            string code = "7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF56" + // -170
                "7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF57" + // -169
                "13";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000001");
        }

        [TestMethod]
        public void WhenLt1()
        {
            string code = "6001600210";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void WhenLt2()
        {
            string code = "6001610F0010";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void WhenLt3()
        {
            string code = "6301020304610F0010";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000001");
        }

        [TestMethod]
        public void WhenSlt1()
        {
            string code = "6001600212";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void WhenSlt2()
        {
            string code = "7F000000000000000000000000000000000000000000000000000000000000001E"+
                "7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF56" +
                "12";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000001");
        }

        [TestMethod]
        public void WhenExecuteContract()
        {
            string code = "606060405261010c806100126000396000f360606040526000357c010000000000000000000000000000000000000000000000000000000090048063f3593cd01461003957610037565b005b61004660048050506100b4565b60405180806020018281038252838181518152602001915080519060200190808383829060006004602084601f0104600302600f01f150905090810190601f1680156100a65780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b6020604051908101604052806000815260200150604060405190810160405280600b81526020017f68656c6c6f20776f726c640000000000000000000000000000000000000000008152602001509050610109565b9056";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program); // PUSH1 0x60
            _vm.Step(program); // PUSH1 0x40 
            _vm.Step(program); // MSTORE 
            _vm.Step(program); // PUSH2 0x010c 
            _vm.Step(program); // DUP1
            _vm.Step(program); // PUSH2 0x0012 
            _vm.Step(program); // PUSH1 0x00  

            string s = "";
        }
    }
}

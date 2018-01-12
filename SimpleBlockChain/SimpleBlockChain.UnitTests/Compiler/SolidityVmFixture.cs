using HashLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Compiler;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Factories;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Stores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SimpleBlockChain.UnitTests.Compiler
{
    [TestClass]
    public class SolidityVmFixture
    {
        private SolidityProgramInvoke _pgInvoke;
        private SolidityVm _vm;
        private static string _adr = "77045E71A7A2C50903D88E564CD72FAB11E82051";
        private Mock<IAssemblyHelper> _assemblyHelperMock;
        private const Networks _network = Networks.MainNet;

        [TestInitialize]
        public void Init()
        {
            var serviceProvider = BuildServiceProvider();
            var smartContractStore = serviceProvider.GetService<ISmartContractStore>();
            var address = new DataWord(_adr.FromHexString().ToArray());
            var callValue = new DataWord("0DE0B6B3A7640000".FromHexString().ToArray());
            IEnumerable<byte> msgData = ("00000000000000000000000000000000000000000000000000000000000000A1" +
                "00000000000000000000000000000000000000000000000000000000000000B1").FromHexString().ToList();
            var scAddress = "0000000000000000000000000000000000000001".FromHexString();
            _pgInvoke = new SolidityProgramInvoke(msgData, scAddress, address, callValue, smartContractStore.GetSmartContracts());
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
        public void WhenNot1()
        {
            string code = "600119";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE");
        }

        [TestMethod]
        public void WhenNot2()
        {
            string code = "61A00319";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF5FFC");
        }

        [TestMethod]
        public void WhenNot3()
        {
            string code = "600019";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
        }

        [TestMethod]
        public void WhenPop1()
        {
            string code = "61000060016200000250";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000001");
        }

        [TestMethod]
        public void WhenPop2()
        {
            string code = "6100006001620000025050";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void WhenDups()
        {
            for(int i = 1; i <= 5; i++)
            {
                WhenDup(i);
            }
        }

        private void WhenDup(int n)
        {
            var code = "";
            var operation = (byte)((byte)SolidityOpCodes.DUP1 + n - 1);
            for (int i = 0; i < n; i++)
            {
                code += "60" + (12 + i);
            }

            var payload = code.FromHexString().ToList();
            payload.Add(operation);
            var program = new SolidityProgram(payload, _pgInvoke);
            var expected = "0000000000000000000000000000000000000000000000000000000000000012";
            var expectedLength = n + 1;
            for (int i = 0; i < expectedLength; i++) {
                _vm.Step(program);
            }
            
            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == expected);
        }

        [TestMethod]
        public void WhenSwaps()
        {
            for(var i = 1; i <= 10; i++)
            {
                WhenSwap(i);
            }
        }

        private void WhenSwap(int n)
        {
            var code = "";
            var operation = (byte)((byte)SolidityOpCodes.SWAP1 + n - 1);
            var top = (new DataWord(new byte[] { (byte)(0x10 + n) })).GetData().ToHexString().ToUpper();
            for (int i = n; i > -1; --i)
            {
                code += "60" + (new byte[] { (byte)(0x10 + i) }).ToHexString();
            }

            var payload = code.FromHexString().ToList();
            payload.Add(operation);
            var program = new SolidityProgram(payload, _pgInvoke);
            for (int i = 0; i < n + 2; ++i)
            {
                _vm.Step(program);
            }

            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(pop == top);
        }

        [TestMethod]
        public void WhenMSTORE1()
        {
            string code = "611234600052";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var memory = program.GetMemory().ToHexString();            
            Assert.IsTrue(memory == "0000000000000000000000000000000000000000000000000000000000001234");
        }

        [TestMethod]
        public void WhenMSTORE2()
        {
            string code = "611234600052615566602052";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var memory = program.GetMemory().ToHexString();
            Assert.IsTrue(memory == "0000000000000000000000000000000000000000000000000000000000001234" +
                "0000000000000000000000000000000000000000000000000000000000005566");
        }
        
        [TestMethod]
        public void WhenMSTORE3()
        {
            string code = "611234600052615566602052618888600052";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var memory = program.GetMemory().ToHexString();
            Assert.IsTrue(memory == "0000000000000000000000000000000000000000000000000000000000008888" +
                "0000000000000000000000000000000000000000000000000000000000005566");
        }

        [TestMethod]
        public void WhenMSTORE4()
        {
            string code = "61123460A052";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var memory = program.GetMemory().ToHexString();
            Assert.IsTrue(memory == "0000000000000000000000000000000000000000000000000000000000000000" +
                "0000000000000000000000000000000000000000000000000000000000000000" +
                "0000000000000000000000000000000000000000000000000000000000000000" +
                "0000000000000000000000000000000000000000000000000000000000000000" +
                "0000000000000000000000000000000000000000000000000000000000000000" +
                "0000000000000000000000000000000000000000000000000000000000001234");
        }

        [TestMethod]
        public void WhenMLOAD1()
        {
            string code = "600051";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);

            var memory = program.GetMemory().ToHexString();
            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(memory == "0000000000000000000000000000000000000000000000000000000000000000");
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void WhenMLOAD2()
        {
            string code = "602251";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);

            var memory = program.GetMemory().ToHexString();
            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(memory == "0000000000000000000000000000000000000000000000000000000000000000" +
                "0000000000000000000000000000000000000000000000000000000000000000" +
                "0000000000000000000000000000000000000000000000000000000000000000");
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void WhenMLOAD3()
        {
            string code = "602051";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);

            var memory = program.GetMemory().ToHexString();
            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(memory == "0000000000000000000000000000000000000000000000000000000000000000" +
                "0000000000000000000000000000000000000000000000000000000000000000");
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void WhenMLOAD4()
        {
            string code = "611234602052602051";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var memory = program.GetMemory().ToHexString();
            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(memory == "0000000000000000000000000000000000000000000000000000000000000000" +
                "0000000000000000000000000000000000000000000000000000000000001234");
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000001234");
        }

        [TestMethod]
        public void WhenMLOAD5()
        {
            string code = "611234602052601F51";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload, _pgInvoke);

            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);
            _vm.Step(program);

            var memory = program.GetMemory().ToHexString();
            var pop = program.StackPop().GetData().ToHexString().ToUpper();
            Assert.IsTrue(memory == "0000000000000000000000000000000000000000000000000000000000000000" +
                "0000000000000000000000000000000000000000000000000000000000001234");
            Assert.IsTrue(pop == "0000000000000000000000000000000000000000000000000000000000000012");
        }

        #region Complex contracts

        [TestMethod]
        public void WhenExecuteComplexContract1()
        {
            // Analyse the code : https://github.com/CoinCulture/evm-tools/blob/master/analysis/guide.md
            // HEX => OPCODES : https://etherscan.io/opcode-tool
            // COMMAND : --bin-runtime : get the code as it would be in the contract after having been deployed.
            var serviceProvider = BuildServiceProvider();
            var smartContractStore = serviceProvider.GetService<ISmartContractStore>();
            var address = new DataWord(_adr.FromHexString().ToArray());
            var callValue = new DataWord("00".FromHexString().ToArray());
            IEnumerable<byte> msgData = ("6d4ce63c").FromHexString().ToList();
            var scAddress = "0000000000000000000000000000000000000001".FromHexString();
            var pgInvoke = new SolidityProgramInvoke(msgData, scAddress, address, callValue, smartContractStore.GetSmartContracts());
            var vm = new SolidityVm();           
            var code = "60606040526000357c0100000000000000000000000000000000000000000000000000000000900480636d4ce63c146037576035565b005b604260048050506058565b6040518082815260200191505060405180910390f35b6000600390506062565b9056";
            var payload = code.FromHexString().ToList();
            var secondProg = new SolidityProgram(payload, pgInvoke);
            while(!secondProg.IsStopped())
            {
                vm.Step(secondProg);
            }

            var res = secondProg.GetResult().GetHReturn().ToHexString();
            Assert.IsTrue(res == "0000000000000000000000000000000000000000000000000000000000000003"); // RETURN THE NUMBER 3.
        }

        [TestMethod]
        public void WhenExecuteComplexContract2()
        {
            var serviceProvider = BuildServiceProvider();
            var smartContractStore = serviceProvider.GetService<ISmartContractStore>();
            var address = new DataWord(_adr.FromHexString().ToArray());
            var callValue = new DataWord("00".FromHexString().ToArray());
            IEnumerable<byte> msgData = ("6d4ce63c").FromHexString().ToList();
            var scAddress = "0000000000000000000000000000000000000001".FromHexString();
            var pgInvoke = new SolidityProgramInvoke(msgData, scAddress, address, callValue, smartContractStore.GetSmartContracts());
            var vm = new SolidityVm();
            var code = "60606040526000357c0100000000000000000000000000000000000000000000000000000000900480636d4ce63c1461003957610037565b005b61004660048050506100b4565b60405180806020018281038252838181518152602001915080519060200190808383829060006004602084601f0104600302600f01f150905090810190601f1680156100a65780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b6020604051908101604052806000815260200150604060405190810160405280600b81526020017f68656c6c6f20776f726c640000000000000000000000000000000000000000008152602001509050610109565b9056";
            var payload = code.FromHexString().ToList();
            var secondProg = new SolidityProgram(payload, pgInvoke);
            while (!secondProg.IsStopped())
            {
                vm.Step(secondProg);
            }

            var hResult = secondProg.GetResult().GetHReturn();
            var res = hResult.ToHexString();
            Assert.IsTrue(res == "0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000b68656c6c6f20776f726c64000000000000000000000000000000000000000000");
            var output = Encoding.ASCII.GetString(hResult).Replace("\0", "").Replace("\v", "");
        }

        [TestMethod]
        public void WhenExecuteComplexContract3()
        {
            RemoveSmartContracts(); // REMOVE THE SMART CONTRACTS.

            var serviceProvider = BuildServiceProvider(); // GET THE DEPENDENCIES.

            var smartContractStore = serviceProvider.GetService<ISmartContractStore>();
            smartContractStore.Switch(Core.Networks.MainNet);

            var operationSignature = "set(uint256)"; // CREATE SET PARAMETER.
            var hash = HashFactory.Crypto.SHA3.CreateKeccak256();
            var operationPayload = hash.ComputeBytes(Encoding.ASCII.GetBytes(operationSignature)).GetBytes().Take(4);
            var operationPayloadStr = operationPayload.ToHexString();
            var parameterPayload = (new DataWord(69)).GetData();
            var setMsgData = new List<byte>();
            setMsgData.AddRange(operationPayload);
            setMsgData.AddRange(parameterPayload);
            var str = setMsgData.ToHexString();

            operationSignature = "get()"; // CREATE GET PARAMETER.
            var getMsgData = hash.ComputeBytes(Encoding.ASCII.GetBytes(operationSignature)).GetBytes().Take(4);

            str = getMsgData.ToHexString();
            var address = new DataWord(_adr.FromHexString().ToArray());
            var callValue = new DataWord("00".FromHexString().ToArray());
            var scAddress = "0000000000000000000000000000000000000001".FromHexString();
            var setPgInvoke = new SolidityProgramInvoke(setMsgData, scAddress, address, callValue, smartContractStore.GetSmartContracts());
            var getPgInvoke = new SolidityProgramInvoke(getMsgData, scAddress, address, callValue, smartContractStore.GetSmartContracts());
            var vm = new SolidityVm();
            var code = "60606040526000357c01000000000000000000000000000000000000000000000000000000009004806360fe47b11460415780636d4ce63c14605757603f565b005b605560048080359060200190919050506078565b005b606260048050506086565b6040518082815260200191505060405180910390f35b806000600050819055505b50565b600060006000505490506094565b9056";
            var payload = code.FromHexString().ToList();
            var setProg = new SolidityProgram(payload, setPgInvoke);
            var getProg = new SolidityProgram(payload, getPgInvoke);

            while (!setProg.IsStopped()) // SET THE VALUE 69.
            {
                vm.Step(setProg, true);
            }

            while (!getProg.IsStopped()) // GET THE VALUE 69
            {
                vm.Step(getProg, true);
            }

            var hResult = getProg.GetResult().GetHReturn().ToHexString();
            Assert.IsTrue(hResult == "0000000000000000000000000000000000000000000000000000000000000045");
        }

        [TestMethod]
        public void WhenExecuteComplexContract4()
        {
            var excepted = "00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000004636f646500000000000000000000000000000000000000000000000000000000";
            const string codeValue = "code";
            RemoveSmartContracts(); // REMOVE THE SMART CONTRACTS.

            var serviceProvider = BuildServiceProvider(); // GET THE DEPENDENCIES.

            var smartContractStore = serviceProvider.GetService<ISmartContractStore>();
            smartContractStore.Switch(Core.Networks.MainNet);

            var operationSignature = "addMedicalPrestation(string,string)"; // ADD MEDICAL PRESTATION PARAMETER.
            var hash = HashFactory.Crypto.SHA3.CreateKeccak256();
            var operationPayload = hash.ComputeBytes(Encoding.ASCII.GetBytes(operationSignature)).GetBytes().Take(4);
            var codeParameterSize = (new DataWord(Encoding.ASCII.GetBytes(codeValue).Length).GetData());
            var codeParameter = (new DataWord(Encoding.ASCII.GetBytes(codeValue)).GetReverseData());
            var inamiPrescriberSizeParameter = DataWord.ZERO.GetData();
            var inamiPrescriberParameter = DataWord.ZERO.GetData(); //(new DataWord(Encoding.ASCII.GetBytes("12222333485"))).GetData();
            var addMedicalPrestMsgData = new List<byte>();
            var offsetFirstParameter = (new DataWord(32 * 2)).GetData();
            var offsetSecondParameter = (new DataWord(32 * 4)).GetData();
            addMedicalPrestMsgData.AddRange(operationPayload);
            addMedicalPrestMsgData.AddRange(offsetFirstParameter);
            addMedicalPrestMsgData.AddRange(offsetSecondParameter);
            addMedicalPrestMsgData.AddRange(codeParameterSize);
            addMedicalPrestMsgData.AddRange(codeParameter);
            addMedicalPrestMsgData.AddRange(inamiPrescriberSizeParameter);

            var address = new DataWord(_adr.FromHexString().ToArray());
            var callValue = new DataWord("00".FromHexString().ToArray());
            var scAddress = "0000000000000000000000000000000000000001".FromHexString();
            var addMedicalPrestPgInvoke = new SolidityProgramInvoke(addMedicalPrestMsgData, scAddress, address, callValue, smartContractStore.GetSmartContracts());
            var vm = new SolidityVm();
            string code = "606060405260043610610041576000357c0100000000000000000000000000000000000000000000000000000000900463ffffffff1680635395133814610046575b600080fd5b341561005157600080fd5b6100e4600480803590602001908201803590602001908080601f0160208091040260200160405190810160405280939291908181526020018383808284378201915050505050509190803590602001908201803590602001908080601f0160208091040260200160405190810160405280939291908181526020018383808284378201915050505050509190505061015f565b6040518080602001828103825283818151815260200191508051906020019080838360005b83811015610124578082015181840152602081019050610109565b50505050905090810190601f1680156101515780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b61016761033b565b600060606040519081016040528084815260200160011515815260200182805480602002602001604051908101604052809291908181526020016000905b8282101561026257838290600052602060002090018054600181600116156101000203166002900480601f01602080910402602001604051908101604052809291908181526020018280546001816001161561010002031660029004801561024e5780601f106102235761010080835404028352916020019161024e565b820191906000526020600020905b81548152906001019060200180831161023157829003601f168201915b5050505050815260200190600101906101a5565b505050508152506002856040518082805190602001908083835b6020831015156102a1578051825260208201915060208101905060208303925061027c565b6001836020036101000a038019825116818451168082178552505050505050905001915050908152602001604051809103902060008201518160000190805190602001906102f092919061034f565b5060208201518160010160006101000a81548160ff021916908315150217905550604082015181600201908051906020019061032d9291906103cf565b509050508391505092915050565b602060405190810160405280600081525090565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f1061039057805160ff19168380011785556103be565b828001600101855582156103be579182015b828111156103bd5782518255916020019190600101906103a2565b5b5090506103cb919061042f565b5090565b82805482825590600052602060002090810192821561041e579160200282015b8281111561041d57825182908051906020019061040d92919061034f565b50916020019190600101906103ef565b5b50905061042b9190610454565b5090565b61045191905b8082111561044d576000816000905550600101610435565b5090565b90565b61047d91905b8082111561047957600081816104709190610480565b5060010161045a565b5090565b90565b50805460018160011615610100020316600290046000825580601f106104a657506104c5565b601f0160209004906000526020600020908101906104c4919061042f565b5b505600a165627a7a72305820ceb094aca4da1700d50eefb9fe93047aca9bf3551b8467a7a0f877949fc8df010029";
            var payload = code.FromHexString().ToList();
            var addMedicalPrestProg = new SolidityProgram(payload, addMedicalPrestPgInvoke);

            while (!addMedicalPrestProg.IsStopped()) // ADD MEDICAL PRESCRIPTION.
            {
                vm.Step(addMedicalPrestProg, true);
            }

            var r = addMedicalPrestProg.GetResult().GetHReturn().ToHexString();
            Assert.IsTrue(r == excepted);
        }

        #endregion
        
        private void RemoveSmartContracts()
        {
            var assm = Assembly.LoadFrom("SimpleBlockChain.Core.dll");
            var path = Path.Combine(Path.GetDirectoryName(assm.Location), string.Format("sc_{0}.dat", SmartContracts.GetDirectoryName(_network)));
            if (!Directory.Exists(path))
            {
                return;
            }

            Directory.Delete(path, true);
        }

        private IServiceProvider BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            _assemblyHelperMock = new Mock<IAssemblyHelper>();
            var assm = Assembly.LoadFrom("SimpleBlockChain.Core.dll");
            _assemblyHelperMock.Setup(a => a.GetEntryAssembly()).Returns(assm);
            serviceCollection.AddTransient<ISmartContractFactory, SmartContractFactory>();
            serviceCollection.AddTransient<ISmartContractStore, SmartContractStore>();
            serviceCollection.AddSingleton<IAssemblyHelper>(_assemblyHelperMock.Object);
            return serviceCollection.BuildServiceProvider();
        }
    }
}

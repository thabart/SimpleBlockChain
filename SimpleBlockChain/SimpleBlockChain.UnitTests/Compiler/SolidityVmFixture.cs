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
            const string prescriberVal = "prescriber";
            const string nomenCodeValue = "nomen";
            RemoveSmartContracts(); // REMOVE THE SMART CONTRACTS.

            var serviceProvider = BuildServiceProvider(); // GET THE DEPENDENCIES.

            var smartContractStore = serviceProvider.GetService<ISmartContractStore>();
            smartContractStore.Switch(Core.Networks.MainNet);

            var operationSignature = "addMedicalPrestation(string,string)"; // ADD MEDICAL PRESTATION PARAMETER.
            var hash = HashFactory.Crypto.SHA3.CreateKeccak256();
            var operationPayload = hash.ComputeBytes(Encoding.ASCII.GetBytes(operationSignature)).GetBytes().Take(4);
            var codeParameterSize = (new DataWord(Encoding.ASCII.GetBytes(codeValue).Length).GetData());
            var codeParameter = (new DataWord(Encoding.ASCII.GetBytes(codeValue)).GetReverseData());
            var inamiPrescriberSizeParameter = (new DataWord(Encoding.ASCII.GetBytes(prescriberVal).Length).GetData());
            var inamiPrescriberSizeValue = (new DataWord(Encoding.ASCII.GetBytes(prescriberVal)).GetReverseData());
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
            addMedicalPrestMsgData.AddRange(inamiPrescriberSizeValue);

            string x = addMedicalPrestMsgData.ToHexString();

            var addNomenCodeSignature = "addNomencode(string,string)"; // ADD NOMEN CODE PARAMETER.
            var addNomenPayload = hash.ComputeBytes(Encoding.ASCII.GetBytes(addNomenCodeSignature)).GetBytes().Take(4);
            var nomenCodeSizeParameter = (new DataWord(Encoding.ASCII.GetBytes(nomenCodeValue).Length).GetData());
            var nomenCodeValueParameter = (new DataWord(Encoding.ASCII.GetBytes(nomenCodeValue)).GetReverseData());
            var addNomenCodeMsgData = new List<byte>();
            addNomenCodeMsgData.AddRange(addNomenPayload);
            addNomenCodeMsgData.AddRange(offsetFirstParameter);
            addNomenCodeMsgData.AddRange(offsetSecondParameter);
            addNomenCodeMsgData.AddRange(codeParameterSize);
            addNomenCodeMsgData.AddRange(codeParameter);
            addNomenCodeMsgData.AddRange(nomenCodeSizeParameter);
            addNomenCodeMsgData.AddRange(nomenCodeValueParameter);
            
            var getNomenCodeSignature = "getNomencodesFromUnconfirmedPrestation(string)"; // GET THE NOMENCLATURE CODES.
            var nomenCodeOperationPayload = hash.ComputeBytes(Encoding.ASCII.GetBytes(getNomenCodeSignature)).GetBytes().Take(4);
            var getNomenCodeMsgData = new List<byte>();
            getNomenCodeMsgData.AddRange(nomenCodeOperationPayload);
            getNomenCodeMsgData.AddRange((new DataWord(32)).GetData());
            getNomenCodeMsgData.AddRange(codeParameterSize);
            getNomenCodeMsgData.AddRange(codeParameter);

            var xxxx = getNomenCodeMsgData.ToHexString();

            var address = new DataWord(_adr.FromHexString().ToArray());
            var callValue = new DataWord("00".FromHexString().ToArray());
            var scAddress = "0000000000000000000000000000000000000001".FromHexString();
            var addMedicalPrestPgInvoke = new SolidityProgramInvoke(addMedicalPrestMsgData, scAddress, address, callValue, smartContractStore.GetSmartContracts());
            var addNomenCodePgInvoke = new SolidityProgramInvoke(addNomenCodeMsgData, scAddress, address, callValue, smartContractStore.GetSmartContracts());
            var getNomenCodePgInvoke = new SolidityProgramInvoke(getNomenCodeMsgData, scAddress, address, callValue, smartContractStore.GetSmartContracts());
            var vm = new SolidityVm();
            // string code = "60606040526004361061006d576000357c0100000000000000000000000000000000000000000000000000000000900463ffffffff16806347d2ac95146100725780635395133814610148578063648b8d1a14610261578063846f52d01461028e578063d1be5c2914610303575b600080fd5b341561007d57600080fd5b6100cd600480803590602001908201803590602001908080601f016020809104026020016040519081016040528093929190818152602001838380828437820191505050505050919050506103bb565b6040518080602001828103825283818151815260200191508051906020019080838360005b8381101561010d5780820151818401526020810190506100f2565b50505050905090810190601f16801561013a5780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b341561015357600080fd5b6101e6600480803590602001908201803590602001908080601f0160208091040260200160405190810160405280939291908181526020018383808284378201915050505050509190803590602001908201803590602001908080601f016020809104026020016040519081016040528093929190818152602001838380828437820191505050505050919050506108e2565b6040518080602001828103825283818151815260200191508051906020019080838360005b8381101561022657808201518184015260208101905061020b565b50505050905090810190601f1680156102535780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b341561026c57600080fd5b610274610abe565b604051808215151515815260200191505060405180910390f35b341561029957600080fd5b6102e9600480803590602001908201803590602001908080601f01602080910402602001604051908101604052809392919081815260200183838082843782019150505050505091905050610ac7565b604051808215151515815260200191505060405180910390f35b341561030e57600080fd5b6103a1600480803590602001908201803590602001908080601f0160208091040260200160405190810160405280939291908181526020018383808284378201915050505050509190803590602001908201803590602001908080601f01602080910402602001604051908101604052809392919081815260200183838082843782019150505050505091905050610cf8565b604051808215151515815260200191505060405180910390f35b6103c3610f4b565b6000806103ce610f4b565b60008060006103db610f4b565b600080600080151560028d6040518082805190602001908083835b60208310151561041b57805182526020820191506020810190506020830392506103f6565b6001836020036101000a038019825116818451168082178552505050505050905001915050908152602001604051809103902060010160009054906101000a900460ff16151514156104925760006040518059106104765750595b9080825280601f01601f19166020018201604052509a506108d3565b60028c6040518082805190602001908083835b6020831015156104ca57805182526020820191506020810190506020830392506104a5565b6001836020036101000a0380198251168184511680821785525050505050509050019150509081526020016040518091039020600201995060009850600095505b898054905086101561060557898681548110151561052557fe5b90600052602060002090018054600181600116156101000203166002900480601f0160208091040260200160405190810160405280929190818152602001828054600181600116156101000203166002900480156105c45780601f10610599576101008083540402835291602001916105c4565b820191906000526020600020905b8154815290600101906020018083116105a757829003601f168201915b505050505097506105d58851610e3d565b965060ff8760ff16141515156105ea57600080fd5b87518760010160ff160189019850858060010196505061050b565b60009450886040518059106106175750595b9080825280601f01601f19166020018201604052509350600092505b89805490508310156108cf57898381548110151561064d57fe5b90600052602060002090018054600181600116156101000203166002900480601f0160208091040260200160405190810160405280929190818152602001828054600181600116156101000203166002900480156106ec5780601f106106c1576101008083540402835291602001916106ec565b820191906000526020600020905b8154815290600101906020018083116106cf57829003601f168201915b505050505097506106fd8851610e3d565b9650867f010000000000000000000000000000000000000000000000000000000000000002848681518110151561073057fe5b9060200101907effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916908160001a9053508480600101955050600091505b8660ff1682101561080857816001880360ff160360080260020a885181151561079257fe5b047f01000000000000000000000000000000000000000000000000000000000000000284868151811015156107c357fe5b9060200101907effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916908160001a9053508480600101955050818060010192505061076d565b600090505b87518110156108c257878181518110151561082457fe5b9060200101517f010000000000000000000000000000000000000000000000000000000000000090047f010000000000000000000000000000000000000000000000000000000000000002848681518110151561087d57fe5b9060200101907effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916908160001a9053508480600101955050808060010191505061080d565b8280600101935050610633565b839a505b50505050505050505050919050565b6108ea610f5f565b600060606040519081016040528084815260200160011515815260200182805480602002602001604051908101604052809291908181526020016000905b828210156109e557838290600052602060002090018054600181600116156101000203166002900480601f0160208091040260200160405190810160405280929190818152602001828054600181600116156101000203166002900480156109d15780601f106109a6576101008083540402835291602001916109d1565b820191906000526020600020905b8154815290600101906020018083116109b457829003601f168201915b505050505081526020019060010190610928565b505050508152506002856040518082805190602001908083835b602083101515610a2457805182526020820191506020810190506020830392506109ff565b6001836020036101000a03801982511681845116808217855250505050505090500191505090815260200160405180910390206000820151816000019080519060200190610a73929190610f73565b5060208201518160010160006101000a81548160ff0219169083151502179055506040820151816002019080519060200190610ab0929190610ff3565b509050508391505092915050565b60006001905090565b600080600015156002846040518082805190602001908083835b602083101515610b065780518252602082019150602081019050602083039250610ae1565b6001836020036101000a038019825116818451168082178552505050505050905001915050908152602001604051809103902060010160009054906101000a900460ff1615151415610b5b5760009150610cf2565b6002836040518082805190602001908083835b602083101515610b935780518252602082019150602081019050602083039250610b6e565b6001836020036101000a038019825116818451168082178552505050505050905001915050908152602001604051809103902090507f65807b1284be500e2df52e16de82bcc141d464d2d6e0ddc9588d04c5d9a8b1608382600001604051808060200180602001838103835285818151815260200191508051906020019080838360005b83811015610c32578082015181840152602081019050610c17565b50505050905090810190601f168015610c5f5780820380516001836020036101000a031916815260200191505b50838103825284818154600181600116156101000203166002900481526020019150805460018160011615610100020316600290048015610ce15780601f10610cb657610100808354040283529160200191610ce1565b820191906000526020600020905b815481529060010190602001808311610cc457829003601f168201915b505094505050505060405180910390a15b50919050565b60008015156002846040518082805190602001908083835b602083101515610d355780518252602082019150602081019050602083039250610d10565b6001836020036101000a038019825116818451168082178552505050505050905001915050908152602001604051809103902060010160009054906101000a900460ff1615151415610d8a5760009050610e37565b6002836040518082805190602001908083835b602083101515610dc25780518252602082019150602081019050602083039250610d9d565b6001836020036101000a03801982511681845116808217855250505050505090500191505090815260200160405180910390206002018054806001018281610e0a9190611053565b916000526020600020900160008490919091509080519060200190610e3092919061107f565b5050600190505b92915050565b60007fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff8211151515610e6e57600080fd5b60008210158015610e82575060ff80168211155b15610e905760019050610f46565b60ff801682118015610ea6575061ffff80168211155b15610eb45760029050610f46565b61ffff80168210158015610ecd575063ffffffff801682105b15610edb5760049050610f46565b63ffffffff80168210158015610efa575067ffffffffffffffff801682105b15610f085760089050610f46565b67ffffffffffffffff80168210158015610f3357506fffffffffffffffffffffffffffffffff801682105b15610f415760109050610f46565b602090505b919050565b602060405190810160405280600081525090565b602060405190810160405280600081525090565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f10610fb457805160ff1916838001178555610fe2565b82800160010185558215610fe2579182015b82811115610fe1578251825591602001919060010190610fc6565b5b509050610fef91906110ff565b5090565b828054828255906000526020600020908101928215611042579160200282015b82811115611041578251829080519060200190611031929190610f73565b5091602001919060010190611013565b5b50905061104f9190611124565b5090565b81548183558181151161107a578183600052602060002091820191016110799190611124565b5b505050565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f106110c057805160ff19168380011785556110ee565b828001600101855582156110ee579182015b828111156110ed5782518255916020019190600101906110d2565b5b5090506110fb91906110ff565b5090565b61112191905b8082111561111d576000816000905550600101611105565b5090565b90565b61114d91905b8082111561114957600081816111409190611150565b5060010161112a565b5090565b90565b50805460018160011615610100020316600290046000825580601f106111765750611195565b601f01602090049060005260206000209081019061119491906110ff565b5b505600a165627a7a7230582026cfe3ef1904a37866b3ad5d1b6e709e74b065a8f52d19e79f65e6d4e46de5530029";
            string code = "60606040526004361061006d576000357c0100000000000000000000000000000000000000000000000000000000900463ffffffff16806347d2ac95146100725780635395133814610148578063648b8d1a14610261578063846f52d01461028e578063d1be5c2914610303575b600080fd5b341561007d57600080fd5b6100cd600480803590602001908201803590602001908080601f016020809104026020016040519081016040528093929190818152602001838380828437820191505050505050919050506103bb565b6040518080602001828103825283818151815260200191508051906020019080838360005b8381101561010d5780820151818401526020810190506100f2565b50505050905090810190601f16801561013a5780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b341561015357600080fd5b6101e6600480803590602001908201803590602001908080601f0160208091040260200160405190810160405280939291908181526020018383808284378201915050505050509190803590602001908201803590602001908080601f016020809104026020016040519081016040528093929190818152602001838380828437820191505050505050919050506108e2565b6040518080602001828103825283818151815260200191508051906020019080838360005b8381101561022657808201518184015260208101905061020b565b50505050905090810190601f1680156102535780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b341561026c57600080fd5b610274610abe565b604051808215151515815260200191505060405180910390f35b341561029957600080fd5b6102e9600480803590602001908201803590602001908080601f01602080910402602001604051908101604052809392919081815260200183838082843782019150505050505091905050610ac7565b604051808215151515815260200191505060405180910390f35b341561030e57600080fd5b6103a1600480803590602001908201803590602001908080601f0160208091040260200160405190810160405280939291908181526020018383808284378201915050505050509190803590602001908201803590602001908080601f01602080910402602001604051908101604052809392919081815260200183838082843782019150505050505091905050610cf8565b604051808215151515815260200191505060405180910390f35b6103c3610f4b565b6000806103ce610f4b565b60008060006103db610f4b565b600080600080151560028d6040518082805190602001908083835b60208310151561041b57805182526020820191506020810190506020830392506103f6565b6001836020036101000a038019825116818451168082178552505050505050905001915050908152602001604051809103902060010160009054906101000a900460ff16151514156104925760006040518059106104765750595b9080825280601f01601f19166020018201604052509a506108d3565b60028c6040518082805190602001908083835b6020831015156104ca57805182526020820191506020810190506020830392506104a5565b6001836020036101000a0380198251168184511680821785525050505050509050019150509081526020016040518091039020600201995060009850600095505b898054905086101561060557898681548110151561052557fe5b90600052602060002090018054600181600116156101000203166002900480601f0160208091040260200160405190810160405280929190818152602001828054600181600116156101000203166002900480156105c45780601f10610599576101008083540402835291602001916105c4565b820191906000526020600020905b8154815290600101906020018083116105a757829003601f168201915b505050505097506105d58851610e3d565b965060ff8760ff16141515156105ea57600080fd5b87518760010160ff160189019850858060010196505061050b565b60009450886040518059106106175750595b9080825280601f01601f19166020018201604052509350600092505b89805490508310156108cf57898381548110151561064d57fe5b90600052602060002090018054600181600116156101000203166002900480601f0160208091040260200160405190810160405280929190818152602001828054600181600116156101000203166002900480156106ec5780601f106106c1576101008083540402835291602001916106ec565b820191906000526020600020905b8154815290600101906020018083116106cf57829003601f168201915b505050505097506106fd8851610e3d565b9650867f010000000000000000000000000000000000000000000000000000000000000002848681518110151561073057fe5b9060200101907effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916908160001a9053508480600101955050600091505b8660ff1682101561080857816001880360ff160360080260020a885181151561079257fe5b047f01000000000000000000000000000000000000000000000000000000000000000284868151811015156107c357fe5b9060200101907effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916908160001a9053508480600101955050818060010192505061076d565b600090505b87518110156108c257878181518110151561082457fe5b9060200101517f010000000000000000000000000000000000000000000000000000000000000090047f010000000000000000000000000000000000000000000000000000000000000002848681518110151561087d57fe5b9060200101907effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916908160001a9053508480600101955050808060010191505061080d565b8280600101935050610633565b839a505b50505050505050505050919050565b6108ea610f5f565b600060606040519081016040528084815260200160011515815260200182805480602002602001604051908101604052809291908181526020016000905b828210156109e557838290600052602060002090018054600181600116156101000203166002900480601f0160208091040260200160405190810160405280929190818152602001828054600181600116156101000203166002900480156109d15780601f106109a6576101008083540402835291602001916109d1565b820191906000526020600020905b8154815290600101906020018083116109b457829003601f168201915b505050505081526020019060010190610928565b505050508152506002856040518082805190602001908083835b602083101515610a2457805182526020820191506020810190506020830392506109ff565b6001836020036101000a03801982511681845116808217855250505050505090500191505090815260200160405180910390206000820151816000019080519060200190610a73929190610f73565b5060208201518160010160006101000a81548160ff0219169083151502179055506040820151816002019080519060200190610ab0929190610ff3565b509050508391505092915050565b60006001905090565b600080600015156002846040518082805190602001908083835b602083101515610b065780518252602082019150602081019050602083039250610ae1565b6001836020036101000a038019825116818451168082178552505050505050905001915050908152602001604051809103902060010160009054906101000a900460ff1615151415610b5b5760009150610cf2565b6002836040518082805190602001908083835b602083101515610b935780518252602082019150602081019050602083039250610b6e565b6001836020036101000a038019825116818451168082178552505050505050905001915050908152602001604051809103902090507f65807b1284be500e2df52e16de82bcc141d464d2d6e0ddc9588d04c5d9a8b1608382600001604051808060200180602001838103835285818151815260200191508051906020019080838360005b83811015610c32578082015181840152602081019050610c17565b50505050905090810190601f168015610c5f5780820380516001836020036101000a031916815260200191505b50838103825284818154600181600116156101000203166002900481526020019150805460018160011615610100020316600290048015610ce15780601f10610cb657610100808354040283529160200191610ce1565b820191906000526020600020905b815481529060010190602001808311610cc457829003601f168201915b505094505050505060405180910390a15b50919050565b60008015156002846040518082805190602001908083835b602083101515610d355780518252602082019150602081019050602083039250610d10565b6001836020036101000a038019825116818451168082178552505050505050905001915050908152602001604051809103902060010160009054906101000a900460ff1615151415610d8a5760009050610e37565b6002836040518082805190602001908083835b602083101515610dc25780518252602082019150602081019050602083039250610d9d565b6001836020036101000a03801982511681845116808217855250505050505090500191505090815260200160405180910390206002018054806001018281610e0a9190611053565b916000526020600020900160008490919091509080519060200190610e3092919061107f565b5050600190505b92915050565b60007fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff8211151515610e6e57600080fd5b60008210158015610e82575060ff80168211155b15610e905760019050610f46565b60ff801682118015610ea6575061ffff80168211155b15610eb45760029050610f46565b61ffff80168210158015610ecd575063ffffffff801682105b15610edb5760049050610f46565b63ffffffff80168210158015610efa575067ffffffffffffffff801682105b15610f085760089050610f46565b67ffffffffffffffff80168210158015610f3357506fffffffffffffffffffffffffffffffff801682105b15610f415760109050610f46565b602090505b919050565b602060405190810160405280600081525090565b602060405190810160405280600081525090565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f10610fb457805160ff1916838001178555610fe2565b82800160010185558215610fe2579182015b82811115610fe1578251825591602001919060010190610fc6565b5b509050610fef91906110ff565b5090565b828054828255906000526020600020908101928215611042579160200282015b82811115611041578251829080519060200190611031929190610f73565b5091602001919060010190611013565b5b50905061104f9190611124565b5090565b81548183558181151161107a578183600052602060002091820191016110799190611124565b5b505050565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f106110c057805160ff19168380011785556110ee565b828001600101855582156110ee579182015b828111156110ed5782518255916020019190600101906110d2565b5b5090506110fb91906110ff565b5090565b61112191905b8082111561111d576000816000905550600101611105565b5090565b90565b61114d91905b8082111561114957600081816111409190611150565b5060010161112a565b5090565b90565b50805460018160011615610100020316600290046000825580601f106111765750611195565b601f01602090049060005260206000209081019061119491906110ff565b5b505600a165627a7a723058206aab832aaf5aeaeebccc37697b3bfe60ee55ec6815a5ea8444c1e75c95f689c40029";
            var payload = code.FromHexString().ToList();
            var addMedicalPrestProg = new SolidityProgram(payload, addMedicalPrestPgInvoke);
            var addNomenCodeProg = new SolidityProgram(payload, addNomenCodePgInvoke);
            var getNomenCodeProg = new SolidityProgram(payload, getNomenCodePgInvoke);

            while (!addMedicalPrestProg.IsStopped()) // ADD MEDICAL PRESCRIPTION.
            {
                vm.Step(addMedicalPrestProg, true);
            }

            var r = addMedicalPrestProg.GetResult().GetHReturn().ToHexString();
            Assert.IsTrue(r == excepted);

            while (!addNomenCodeProg.IsStopped()) // ADD NOMEN CODE.
            {
                vm.Step(addNomenCodeProg, true);
            }

            while (!getNomenCodeProg.IsStopped()) // GET NOMEN CODE.
            {
                vm.Step(getNomenCodeProg, true);
            }

            var hR = getNomenCodeProg.GetResult().GetHReturn().ToHexString();
            r = Encoding.ASCII.GetString(getNomenCodeProg.GetResult().GetHReturn());
            Assert.IsTrue(r == "0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000701056e6f6d656e00000000000000000000000000000000000000000000000000");
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

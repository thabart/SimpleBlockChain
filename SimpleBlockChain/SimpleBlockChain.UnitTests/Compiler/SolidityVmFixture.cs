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

            operationSignature = "get()"; // CREATE GET PARAMETER.
            var getMsgData = hash.ComputeBytes(Encoding.ASCII.GetBytes(operationSignature)).GetBytes().Take(4);

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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleBlockChain.Core.Compiler;

namespace SimpleBlockChain.UnitTests.Compiler
{
    [TestClass]
    public  class SolidityCompilerFixture
    {
        [TestMethod]
        public void Test()
        {
            string contract =
                "pragma solidity ^0.4.7;\n" +
                        "\n" +
                        "contract a {\n" +
                        "\n" +
                        "        mapping(address => string) private mailbox;\n" +
                        "\n" +
                        "        event Mailed(address from, string message);\n" +
                        "        event Read(address from, string message);\n" +
                        "\n" +
                        "}";
            SolidityCompiler.Compile(contract);
            string s = "";
        }
    }
}

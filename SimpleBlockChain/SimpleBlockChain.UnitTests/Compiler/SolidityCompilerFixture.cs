using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleBlockChain.Core.Compiler;

namespace SimpleBlockChain.UnitTests.Compiler
{
    [TestClass]
    public  class SolidityCompilerFixture
    {
        [TestMethod]
        public void WhenCompile()
        {
            string contract =
                "contract Multiply8 { "+
                    "function multiply() returns(string) { "+
                        "return \"hello world\"; "+
                     "} "+
                 "}";
            var binaries = SolidityCompiler.Compile(contract);
            string s = "";
        }
    }
}

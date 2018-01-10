using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleBlockChain.Core.Compiler;
using System.Linq;

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

        [TestMethod]
        public void WhenGetAbi()
        {
            string contract =
                "contract Multiply8 { " +
                    "function multiply() returns(string) { " +
                        "return \"hello world\"; " +
                     "} " +
                 "}" +
                 "contract Multiply6 { " +
                    "function multiply() returns(string) { " +
                        "return \"hello world\"; " +
                     "} " +
                 "}";

            var arr = SolidityCompiler.Compile(contract);

            Assert.IsTrue(arr.Count() == 2);
        }
    }
}

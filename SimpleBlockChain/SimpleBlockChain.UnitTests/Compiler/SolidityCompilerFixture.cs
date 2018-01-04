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
                "contract Multiply7 { "+
                    "event Print(uint); "+
                    "function multiply(uint input) returns(uint) "+
                    "{ "+
                        "Print(input * 7); "+
                        "return input * 7; "+
                    "} "+
                "} " + 
                "contract Multiply8 { " +
                    "event Print(uint); " +
                    "function multiply(uint input) returns(uint) " +
                    "{ " +
                        "Print(input * 7); " +
                        "return input * 7; " +
                    "} " +
                "}";
            var binaries = SolidityCompiler.Compile(contract);
        }
    }
}

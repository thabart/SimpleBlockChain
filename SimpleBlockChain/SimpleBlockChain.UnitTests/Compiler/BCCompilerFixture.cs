using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleBlockChain.Core.Compiler;

namespace SimpleBlockChain.UnitTests.Compiler
{
    [TestClass]
    public class BCCompilerFixture
    {
        [TestMethod]
        public void WhenCompileContract()
        {
            var code = @"
                using System;
                public class SimpleTest
                {
                    private string y {get; set;}
                    private string z;
                    public string Test2(string parameter)
                    {
                        return parameter;
                    }
                    private string Test()
                    {
                        return ""1"";
                    }
                }";
            var compiler = new DotnetCompiler();
            compiler.Compile(code);
        }
    }
}

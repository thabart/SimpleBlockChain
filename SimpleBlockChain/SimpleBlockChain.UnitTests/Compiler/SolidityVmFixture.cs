using SimpleBlockChain.Core.Compiler;
using SimpleBlockChain.Core.Extensions;
using System.Linq;

namespace SimpleBlockChain.UnitTests.Compiler
{
    public class SolidityVmFixture
    {
        public void WhenExecuteSimpleScript()
        {
            string code = "600035";
            var payload = code.FromHexString().ToList();
            var program = new SolidityProgram(payload);
            var vm = new SolidityVm();

            vm.Step(program);
            vm.Step(program);

            var pop = program.StackPop();
        }
    }
}

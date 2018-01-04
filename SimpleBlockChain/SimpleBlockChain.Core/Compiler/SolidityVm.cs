using System;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityVm
    {
        private Dictionary<SolidityOpCodes, int> _sizeSolidityCodes = new Dictionary<SolidityOpCodes, int>
        {
            { SolidityOpCodes.PUSH1, 1 }
        };

        public void Step(SolidityProgram program)
        {
            if (program == null) // http://solidity.readthedocs.io/en/develop/assembly.html
            {
                throw new ArgumentNullException(nameof(program));
            }

            var solidityOpCode = SolidityOpCode.Instance();
            var opCode = solidityOpCode.GetCode(program.GetCurrentOpCode());
            switch(opCode)
            {
                case SolidityOpCodes.PUSH1:
                    program.Step();
                    var nPush = _sizeSolidityCodes[opCode.Value];
                    var data = program.Sweep(nPush);
                    program.StackPush(data);
                    break;
                case SolidityOpCodes.CALLDATALOAD:
                    var pop = program.StackPop();
                    var dataValue = program.GetDataValue(pop);
                    program.StackPush(dataValue);
                    program.Step();
                    break;
                case SolidityOpCodes.CALLDATACOPY:
                    var memOffsetData = program.StackPop();
                    var dataOffsetData = program.StackPop();
                    var lengthData = program.StackPop();
                    var msgData = program.GetDataCopy(dataOffsetData, lengthData);
                    program.SaveMemory(msgData);
                    program.Step();
                    // copy s bytes from calldata at position f to mem at position t
                    break;
            }
        }
    }
}

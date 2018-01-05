using System;
using System.Collections.Generic;
using System.Numerics;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityVm
    {
        private static BigInteger BG_32 = new BigInteger(32);

        private Dictionary<SolidityOpCodes, int> _sizeSolidityCodes = new Dictionary<SolidityOpCodes, int>
        {
            { SolidityOpCodes.PUSH1, 1 },
            { SolidityOpCodes.PUSH2, 2 },
            { SolidityOpCodes.PUSH3, 3 },
            { SolidityOpCodes.PUSH4, 4 },
            { SolidityOpCodes.PUSH5, 5 },
            { SolidityOpCodes.PUSH6, 6 },
            { SolidityOpCodes.PUSH7, 7 },
            { SolidityOpCodes.PUSH8, 8 },
            { SolidityOpCodes.PUSH9, 9 },
            { SolidityOpCodes.PUSH10, 10 },
            { SolidityOpCodes.PUSH11, 11 },
            { SolidityOpCodes.PUSH12, 12 }
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
                case SolidityOpCodes.PUSH2:
                case SolidityOpCodes.PUSH3:
                case SolidityOpCodes.PUSH4:
                case SolidityOpCodes.PUSH5:
                case SolidityOpCodes.PUSH6:
                case SolidityOpCodes.PUSH7:
                case SolidityOpCodes.PUSH8:
                case SolidityOpCodes.PUSH9:
                case SolidityOpCodes.PUSH10:
                case SolidityOpCodes.PUSH11:
                case SolidityOpCodes.PUSH12:
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
                    break;
                case SolidityOpCodes.ADDRESS:
                    var address = program.GetOwnerAddress();
                    program.StackPush(address);
                    program.Step();
                    break;
                case SolidityOpCodes.AND:
                    var word1 = program.StackPop();
                    var word2 = program.StackPop();
                    word1.And(word2);
                    program.StackPush(word1);
                    program.Step();
                    break;
                case SolidityOpCodes.OR:
                    var w1 = program.StackPop();
                    var w2 = program.StackPop();
                    w1.Or(w2);
                    program.StackPush(w1);
                    program.Step();
                    break;
                case SolidityOpCodes.XOR:
                    var xorW1 = program.StackPop();
                    var xorW2 = program.StackPop();
                    xorW1.XOR(xorW2);
                    program.StackPush(xorW1);
                    program.Step();
                    break;
                case SolidityOpCodes.BYTE:
                    var bw1 = program.StackPop();
                    var bw2 = program.StackPop();
                    DataWord result = new DataWord();
                    if (!bw1.GetValue().Equals(BG_32))
                    {
                        byte tmp = bw2.GetData()[bw1.GetInt()];
                        bw2.And(DataWord.ZERO);
                        bw2.GetData()[31] = tmp;
                        result = bw2;
                    }

                    program.StackPush(result);
                    program.Step();
                    break;
            }
        }
    }
}

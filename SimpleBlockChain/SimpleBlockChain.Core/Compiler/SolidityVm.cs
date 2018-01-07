using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityVm
    {
        private static readonly BigInteger BG_32 = BigInteger.ValueOf(32);

        public static Dictionary<SolidityOpCodes, int> SizeSolidityCodes = new Dictionary<SolidityOpCodes, int>
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
            { SolidityOpCodes.PUSH12, 12 },
            { SolidityOpCodes.PUSH13, 13 },
            { SolidityOpCodes.PUSH14, 14 },
            { SolidityOpCodes.PUSH15, 15 },
            { SolidityOpCodes.PUSH16, 16 },
            { SolidityOpCodes.PUSH17, 17 },
            { SolidityOpCodes.PUSH18, 18 },
            { SolidityOpCodes.PUSH19, 19 },
            { SolidityOpCodes.PUSH20, 20 },
            { SolidityOpCodes.PUSH21, 21 },
            { SolidityOpCodes.PUSH22, 22 },
            { SolidityOpCodes.PUSH23, 23 },
            { SolidityOpCodes.PUSH24, 24 },
            { SolidityOpCodes.PUSH25, 25 },
            { SolidityOpCodes.PUSH26, 26 },
            { SolidityOpCodes.PUSH27, 27 },
            { SolidityOpCodes.PUSH28, 28 },
            { SolidityOpCodes.PUSH29, 29 },
            { SolidityOpCodes.PUSH30, 30 },
            { SolidityOpCodes.PUSH31, 31 },
            { SolidityOpCodes.PUSH32, 32 },
            { SolidityOpCodes.DUP1, 1 },
            { SolidityOpCodes.DUP2, 2 },
            { SolidityOpCodes.DUP3, 3 },
            { SolidityOpCodes.DUP4, 4 },
            { SolidityOpCodes.DUP5, 5 },
            { SolidityOpCodes.SWAP1, 2 },
            { SolidityOpCodes.SWAP2, 3 },
            { SolidityOpCodes.SWAP3, 4 },
            { SolidityOpCodes.SWAP4, 5 },
            { SolidityOpCodes.SWAP5, 6 },
            { SolidityOpCodes.SWAP6, 7 },
            { SolidityOpCodes.SWAP7, 8 },
            { SolidityOpCodes.SWAP8, 9 },
            { SolidityOpCodes.SWAP9, 10 },
            { SolidityOpCodes.SWAP10, 11 }
        };

        public void Step(SolidityProgram program)
        {
            if (program == null) // http://solidity.readthedocs.io/en/develop/assembly.html
            {
                throw new ArgumentNullException(nameof(program));
            }

            var solidityOpCode = SolidityOpCode.Instance();
            var currentOpCode = program.GetCurrentOpCode();
            var opCode = solidityOpCode.GetCode(currentOpCode);
            var stack = program.GetStack();
            switch (opCode)
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
                case SolidityOpCodes.PUSH13:
                case SolidityOpCodes.PUSH14:
                case SolidityOpCodes.PUSH15:
                case SolidityOpCodes.PUSH16:
                case SolidityOpCodes.PUSH17:
                case SolidityOpCodes.PUSH18:
                case SolidityOpCodes.PUSH19:
                case SolidityOpCodes.PUSH20:
                case SolidityOpCodes.PUSH21:
                case SolidityOpCodes.PUSH22:
                case SolidityOpCodes.PUSH23:
                case SolidityOpCodes.PUSH24:
                case SolidityOpCodes.PUSH25:
                case SolidityOpCodes.PUSH26:
                case SolidityOpCodes.PUSH27:
                case SolidityOpCodes.PUSH28:
                case SolidityOpCodes.PUSH29:
                case SolidityOpCodes.PUSH30:
                case SolidityOpCodes.PUSH31:
                case SolidityOpCodes.PUSH32:
                    if (opCode == SolidityOpCodes.PUSH32)
                    {
                        string sss = "";
                    }
                    program.Step();
                    var nPush = SizeSolidityCodes[opCode.Value];
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
                    program.SaveMemory(memOffsetData.GetIntValueSafe(), msgData.ToArray());
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
                    var vBw1 = bw1.GetValue();
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
                case SolidityOpCodes.ISZERO:
                    var isZeroW1 = program.StackPop();
                    if (isZeroW1.IsZero())
                    {
                        isZeroW1.GetData()[31] = 1;
                    }
                    else
                    {
                        isZeroW1.And(DataWord.ZERO);
                    }
                    
                    program.StackPush(isZeroW1);
                    program.Step();
                    break;
                case SolidityOpCodes.EQ:
                    var eqW1 = program.StackPop();
                    var eqW2 = program.StackPop();
                    if (eqW1.XOR(eqW2).IsZero())
                    {
                        eqW1.And(DataWord.ZERO);
                        eqW1.GetData()[31] = 1;
                    }
                    else
                    {
                        eqW1.And(DataWord.ZERO);
                    }

                    program.StackPush(eqW1);
                    program.Step();
                    break;
                case SolidityOpCodes.GT:
                    var gtW1 = program.StackPop();
                    var gtW2 = program.StackPop();
                    if (gtW1.GetValue().CompareTo(gtW2.GetValue()) == 1)
                    {
                        gtW1.And(DataWord.ZERO);
                        gtW1.GetData()[31] = 1;
                    }
                    else
                    {
                        gtW1.And(DataWord.ZERO);
                    }
                    
                    program.StackPush(gtW1);
                    program.Step();
                    break;
                case SolidityOpCodes.SGT:
                    var sgtW1 = program.StackPop();
                    var sgtW2 = program.StackPop();
                    if (sgtW1.GetSValue().CompareTo(sgtW2.GetSValue()) == 1)
                    {
                        sgtW1.And(DataWord.ZERO);
                        sgtW1.GetData()[31] = 1;
                    }
                    else
                    {
                        sgtW1.And(DataWord.ZERO);
                    }

                    program.StackPush(sgtW1);
                    program.Step();
                    break;
                case SolidityOpCodes.LT:
                    var ltW1 = program.StackPop();
                    var ltW2 = program.StackPop();
                    if (ltW1.GetValue().CompareTo(ltW2.GetValue()) == -1)
                    {
                        ltW1.And(DataWord.ZERO);
                        ltW1.GetData()[31] = 1;
                    }
                    else
                    {
                        ltW1.And(DataWord.ZERO);
                    }

                    program.StackPush(ltW1);
                    program.Step();
                    break;
                case SolidityOpCodes.SLT:
                    var sltW1 = program.StackPop();
                    var sltW2 = program.StackPop();
                    if (sltW1.GetSValue().CompareTo(sltW2.GetSValue()) == -1)
                    {
                        sltW1.And(DataWord.ZERO);
                        sltW1.GetData()[31] = 1;
                    }
                    else
                    {
                        sltW1.And(DataWord.ZERO);
                    }

                    program.StackPush(sltW1);
                    program.Step();
                    break;
                case SolidityOpCodes.MSTORE:
                    var msStoreW1 = program.StackPop();
                    var msStoreW2 = program.StackPop();
                    program.SaveMemory(msStoreW1, msStoreW2);
                    program.Step();
                    break;
                case SolidityOpCodes.CALLVALUE:
                    var callValue = program.GetCallValue();
                    program.StackPush(callValue);
                    program.Step();
                    break;
                case SolidityOpCodes.JUMPI:
                    var pos = program.StackPop();
                    var cond = program.StackPop();
                    if (!cond.IsZero())
                    {
                        int nextPC = program.VerifyJumpDest(pos);
                        program.SetPc(nextPC);
                    }
                    else
                    {
                        program.Step();
                    }
                    break;
                case SolidityOpCodes.JUMP:
                    var jumpW1 = program.StackPop();
                    var nextPc = program.VerifyJumpDest(jumpW1);
                    program.SetPc(nextPc);
                    break;
                case SolidityOpCodes.JUMPDEST:
                    program.Step();
                    break;
                case SolidityOpCodes.DUP1:
                case SolidityOpCodes.DUP2:
                case SolidityOpCodes.DUP3:
                case SolidityOpCodes.DUP4:
                case SolidityOpCodes.DUP5:
                    var n = SizeSolidityCodes[opCode.Value] - SizeSolidityCodes[SolidityOpCodes.DUP1] + 1;
                    var dup1W1 = stack[stack.Count() - n];
                    program.StackPush(dup1W1);
                    program.Step();
                    break;
                case SolidityOpCodes.CODECOPY:
                    byte[] fullCode = ByteUtil.EMPTY_BYTE_ARRAY;
                    fullCode = program.GetCode().ToArray();
                    int copyDataW1 = program.StackPop().GetIntValueSafe();
                    int copyDataW2 = program.StackPop().GetIntValueSafe();
                    int copyDataW3 = program.StackPop().GetIntValueSafe();
                    int sizeToBeCopied =
                            (long)copyDataW2 + copyDataW3 > fullCode.Length ?
                                    (fullCode.Length < copyDataW2 ? 0 : fullCode.Length - copyDataW2)
                                    : copyDataW3;
                    byte[] codeCopy = new byte[copyDataW3];
                    if (copyDataW2 < fullCode.Length) { Array.Copy(fullCode, copyDataW2, codeCopy, 0, sizeToBeCopied); }
                    program.SaveMemory(copyDataW1, codeCopy);
                    program.Step();
                    break;
                case SolidityOpCodes.RETURN:
                    DataWord retW1 = program.StackPop();
                    DataWord retW2 = program.StackPop();
                    byte[] hReturn = program.ChunkMemory(retW1.GetIntValueSafe(), retW2.GetIntValueSafe());
                    program.SetHReturn(hReturn);
                    program.Step();
                    program.Stop();
                    break;
                case SolidityOpCodes.SWAP1:
                case SolidityOpCodes.SWAP2:
                case SolidityOpCodes.SWAP3:
                case SolidityOpCodes.SWAP4:
                case SolidityOpCodes.SWAP5:
                case SolidityOpCodes.SWAP6:
                case SolidityOpCodes.SWAP7:
                case SolidityOpCodes.SWAP8:
                case SolidityOpCodes.SWAP9:
                case SolidityOpCodes.SWAP10:
                    var sn = SizeSolidityCodes[opCode.Value] - SizeSolidityCodes[SolidityOpCodes.SWAP1] + 2;
                    stack.Swap(stack.Count() - 1, stack.Count() - sn);
                    program.Step();
                    break;
                case SolidityOpCodes.DIV:
                    var dW1 = program.StackPop();
                    var dW2 = program.StackPop();
                    dW1.Div(dW2);
                    program.StackPush(dW1);
                    program.Step();
                    break;
                case SolidityOpCodes.POP:
                    program.StackPop();
                    program.Step();
                    break;
                case SolidityOpCodes.MLOAD:
                    var mloadW1 = program.StackPop();
                    var memoryW2 = program.LoadMemory(mloadW1);
                    program.StackPush(memoryW2);
                    program.Step();
                    break;
                case SolidityOpCodes.ADD:
                    var addW1 = program.StackPop();
                    var addW2 = program.StackPop();
                    addW1.Add(addW2);
                    program.StackPush(addW1);
                    program.Step();
                    break;
                case SolidityOpCodes.SUB:
                    var subW1 = program.StackPop();
                    var subW2 = program.StackPop();
                    subW1.Add(subW2);
                    program.StackPush(subW1);
                    program.Step();
                    break;
                case SolidityOpCodes.MUL:
                    var mulW1 = program.StackPop();
                    var mulW2 = program.StackPop();
                    mulW1.Add(mulW2);
                    program.StackPush(mulW1);
                    program.Step();
                    break;
                case SolidityOpCodes.CALL:
                    // MESSAGE CALL INTO ACCOUNT.
                    program.StackPop();
                    program.StackPop();
                    program.StackPop();
                    program.StackPop();
                    program.Step();
                    break;
                case SolidityOpCodes.NOT:
                    var notW1 = program.StackPop();
                    notW1.BNot();
                    program.StackPush(notW1);
                    program.Step();
                    break;
            }
        }
    }
}

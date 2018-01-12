using HashLib;
using Org.BouncyCastle.Math;
using SimpleBlockChain.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            { SolidityOpCodes.DUP6, 6 },
            { SolidityOpCodes.DUP7, 7 },
            { SolidityOpCodes.DUP8, 8 },
            { SolidityOpCodes.DUP9, 9 },
            { SolidityOpCodes.DUP10, 10 },
            { SolidityOpCodes.DUP11, 11 },
            { SolidityOpCodes.DUP12, 12 },
            { SolidityOpCodes.DUP13, 13 },
            { SolidityOpCodes.DUP14, 14 },
            { SolidityOpCodes.DUP15, 15 },
            { SolidityOpCodes.DUP16, 16 },
            { SolidityOpCodes.SWAP1, 2 },
            { SolidityOpCodes.SWAP2, 3 },
            { SolidityOpCodes.SWAP3, 4 },
            { SolidityOpCodes.SWAP4, 5 },
            { SolidityOpCodes.SWAP5, 6 },
            { SolidityOpCodes.SWAP6, 7 },
            { SolidityOpCodes.SWAP7, 8 },
            { SolidityOpCodes.SWAP8, 9 },
            { SolidityOpCodes.SWAP9, 10 },
            { SolidityOpCodes.SWAP10, 11 },
            { SolidityOpCodes.SWAP11, 12 },
            { SolidityOpCodes.SWAP12, 13 },
            { SolidityOpCodes.SWAP13, 14 },
            { SolidityOpCodes.SWAP14, 15 },
            { SolidityOpCodes.SWAP15, 16 },
            { SolidityOpCodes.SWAP16, 17 }
        };

        public void Step(SolidityProgram program, bool trace = false)
        {
            if (program == null) // http://solidity.readthedocs.io/en/develop/assembly.html
            {
                throw new ArgumentNullException(nameof(program));
            }

            var solidityOpCode = SolidityOpCode.Instance();
            var currentOpCode = program.GetCurrentOpCode();
            var opCode = solidityOpCode.GetCode(currentOpCode);
            var stack = program.GetStack();

            if (trace)
            {
                Trace.WriteLine("Operation " + Enum.GetName(typeof(SolidityOpCodes), opCode) + " " + program.GetPc());
                Trace.WriteLine("Stack: ");
                foreach (var s in program.GetStack())
                {
                    Trace.WriteLine(s.GetData().ToHexString());
                }

                Trace.WriteLine("Memory: ");
                Trace.WriteLine(program.GetMemory().ToHexString());
            }

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
                    // var isZeroW1 = stack.Last();
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
                case SolidityOpCodes.MSTORE8:
                     var addr = program.StackPop();
                     var value = program.StackPop();
                     byte[] byteVal = { value.GetData()[31] };
                     program.SaveMemory(addr.GetIntValueSafe(), byteVal);
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
                    if (!cond.IsZero() && !pos.IsZero())
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
                case SolidityOpCodes.DUP6:
                case SolidityOpCodes.DUP7:
                case SolidityOpCodes.DUP8:
                case SolidityOpCodes.DUP9:
                case SolidityOpCodes.DUP10:
                case SolidityOpCodes.DUP11:
                case SolidityOpCodes.DUP12:
                case SolidityOpCodes.DUP13:
                case SolidityOpCodes.DUP14:
                case SolidityOpCodes.DUP15:
                case SolidityOpCodes.DUP16:
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
                case SolidityOpCodes.REVERT:
                    int retW1 = program.StackPop().GetIntValueSafe();
                    int retW2 = program.StackPop().GetIntValueSafe();
                    byte[] hReturn = program.ChunkMemory(retW1, retW2);
                    program.SetHReturn(hReturn);
                    program.Step();
                    program.Stop();
                    if (opCode == SolidityOpCodes.REVERT)
                    {
                        program.GetResult().SetRevert();
                    }
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
                case SolidityOpCodes.SWAP11:
                case SolidityOpCodes.SWAP12:
                case SolidityOpCodes.SWAP13:
                case SolidityOpCodes.SWAP14:
                case SolidityOpCodes.SWAP15:
                case SolidityOpCodes.SWAP16:
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
                    subW1.Sub(subW2);
                    program.StackPush(subW1);
                    program.Step();
                    break;
                case SolidityOpCodes.MUL:
                    var mulW1 = program.StackPop();
                    var mulW2 = program.StackPop();
                    mulW1.Mul(mulW2);
                    program.StackPush(mulW1);
                    program.Step();
                    break;
                case SolidityOpCodes.CALL: // CONTINUE TO DEVELOP METHOD CALL.
                    program.StackPop();
                    var codeAddress = program.StackPop();
                    var cv = program.StackPop();

                    DataWord inDataOffs = program.StackPop();
                    DataWord inDataSize = program.StackPop();
                    DataWord outDataOffs = program.StackPop();
                    DataWord outDataSize = program.StackPop();

                    program.MemoryExpand(outDataOffs, outDataSize);
                    var msg = new SolidityMessageCall(opCode.Value, null, codeAddress, cv, inDataOffs, inDataSize, outDataOffs, outDataSize);
                    var contract = SolidityPrecompiledContract.GetContractForAddress(codeAddress);

                    if (contract != null)
                    {
                        program.CallPrecompiledAddress(msg, contract);
                    }

                    program.Step();
                    break;
                case SolidityOpCodes.NOT:
                    var notW1 = program.StackPop();
                    notW1.BNot();
                    program.StackPush(notW1);
                    program.Step();
                    break;
                case SolidityOpCodes.EXP:
                    var expWord1 = program.StackPop();
                    var expWord2 = program.StackPop();
                    expWord1.Exp(expWord2);
                    program.StackPush(expWord1);
                    program.Step();
                    break;
                case SolidityOpCodes.CALLDATASIZE:
                    var dataSize = program.GetDataSize();
                    program.StackPush(dataSize);
                    program.Step();
                    break;
                case SolidityOpCodes.SSTORE:
                    var sstoreWord1 = program.StackPop();
                    var sstoreWord2 = program.StackPop();
                    var tt = sstoreWord1.GetData().ToHexString();
                    var tt2 = sstoreWord2.GetData().ToHexString();
                    program.SaveStorage(sstoreWord1, sstoreWord2);
                    program.Step();
                    break;
                case SolidityOpCodes.STOP:
                    program.SetHReturn(SolidityMemory.EMPTY_BYTE_ARRAY);
                    program.Stop();
                    break;
                case SolidityOpCodes.SLOAD:
                    var sLoadKey = program.StackPop();
                    var sLoadVal = program.StorageLoad(sLoadKey);
                    program.StackPush(sLoadVal);program.Step();
                    break;
                case SolidityOpCodes.SHA3:
                    var sha3MemOffsetData = program.StackPop();
                    var sha3LengthData = program.StackPop();
                    byte[] buffer = program.ChunkMemory(sha3MemOffsetData.GetIntValueSafe(), sha3LengthData.GetIntValueSafe());
                    var hash = HashFactory.Crypto.SHA3.CreateBlake256();
                    var shaWord = hash.ComputeBytes(buffer).GetBytes();
                    program.StackPush(shaWord);
                    program.Step();
                    break;
                case SolidityOpCodes.MSIZE:
                    int memSize = program.GetMemorySize();
                    var sizeW = new DataWord(memSize);
                    program.StackPush(sizeW);
                    program.Step();
                    break;
            }
        }
    }
}

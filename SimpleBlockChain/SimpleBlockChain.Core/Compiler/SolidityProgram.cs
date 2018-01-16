using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityProgram
    {
        private readonly ICollection<byte> _ops;
        private readonly SolidityProgramInvoke _progInvoke;
        private int _pc;
        private SolidityStack _stack;
        private SolidityMemory _memory;
        private SolidityProgramResult _result;
        private SolidityProgramPreCompile programPrecompile;
        private bool _stopped;

        public SolidityProgram(ICollection<byte> ops, SolidityProgramInvoke progInvoke)
        {
            _ops = ops;
            _progInvoke = progInvoke;
            _pc = 0;
            _stack = new SolidityStack();
            _memory = new SolidityMemory();
            _result = new SolidityProgramResult();
            _stopped = false;
        }

        public byte GetCurrentOpCode()
        {
            return _ops.ElementAt(_pc);
        }

        public int GetPc()
        {
            return _pc;
        }

        public void SetPc(int pc)
        {
            _pc = pc;
        }

        public DataWord GetOwnerAddress()
        {
            return _progInvoke.GetOwnerAddress();
        }

        public SolidityProgramResult GetResult()
        {
            return _result;
        }

        public int VerifyJumpDest(DataWord nextPC)
        {
            if (nextPC.GetBytesOccupied() > 4)
            {
                throw new InvalidOperationException("Bad jump destination");
            }

            int ret = nextPC.GetInt();
            if (!GetProgramPrecompile().HasJumpDest(ret))
            {
                throw new InvalidOperationException("Bad jump destination");
            }

            return ret;
        }
        
        public SolidityProgramPreCompile GetProgramPrecompile()
        {
            if (programPrecompile == null)
            {
                programPrecompile = SolidityProgramPreCompile.Compile(_ops.ToArray());
            }

            return programPrecompile;
        }

        public ICollection<byte> GetCode()
        {
            return _ops;
        }

        public void SaveStorage(DataWord w1, DataWord w2)
        {
            var scAddr = _progInvoke.GetSmartContractAddress();
            _progInvoke.GetStorage().AddStorageRow(scAddr, w1, w2, _progInvoke.GetAddInTransaction());
        }

        public void SaveStorage(IEnumerable<byte> key, IEnumerable<byte> val)
        {
            var scAddr = _progInvoke.GetSmartContractAddress();
            var keyWord = new DataWord(key.ToArray());
            var valWord = new DataWord(val.ToArray());
            _progInvoke.GetStorage().AddStorageRow(scAddr, keyWord, valWord, _progInvoke.GetAddInTransaction());
        }

        public DataWord StorageLoad(DataWord keyWord)
        {
            var scAddr = _progInvoke.GetSmartContractAddress();
            return _progInvoke.GetStorage().GetStorageRow(scAddr, keyWord);
        }

        public void Step()
        {
            SetPc(_pc + 1);
        }

        public void SetHReturn(byte[] buff)
        {
            _result.SetHReturn(buff);
        }

        public bool IsStopped()
        {
            return _stopped;
        }

        public void Stop()
        {
            _stopped = true;
        }

        public DataWord GetCallValue()
        {
            return _progInvoke.GetCallValue();
        }

        public void StackPush(IEnumerable<byte> data)
        {
            StackPush(new DataWord(data.ToArray()));
        }

        public void StackPush(DataWord data)
        {
            _stack.Add(data);
        }

        public void MemoryExpand(DataWord outDataOffs, DataWord outDataSize)
        {
            if (!outDataSize.IsZero())
            {
                _memory.Extend(outDataOffs.GetInt(), outDataSize.GetInt());
            }
        }

        public SolidityStack GetStack()
        {
            return _stack;
        }

        public DataWord LoadMemory(DataWord addr)
        {
            return _memory.ReadWord(addr.GetInt());
        }

        public DataWord GetDataSize()
        {
            return _progInvoke.GetDataSize();
        }

        public DataWord StackPop()
        {
            return _stack.Pop();
        }

        public DataWord GetStackElt(int i)
        {
            return _stack.ElementAt(i);
        }

        public int GetMemorySize()
        {
            return _memory.GetSize();
        }

        public byte[] GetMemory()
        {
            return _memory.Read(0, _memory.GetSize());
        }

        public IEnumerable<byte> GetDataCopy(DataWord offsetData, DataWord lengthData)
        {
            return _progInvoke.GetDataCopy(offsetData, lengthData);
        }

        public DataWord GetDataValue(DataWord data)
        {
            return _progInvoke.GetDataValue(data);
        }

        public void SaveMemory(DataWord addrB, DataWord value)
        {
            _memory.Write(addrB.GetInt(), value.GetData(), value.GetData().Length, false);
        }

        public void SaveMemory(int addr, byte[] value)
        {
            _memory.Write(addr, value, value.Length, false);
        }

        public void SaveMemory(int addr, int allocSize, byte[] value)
        {
            _memory.ExtendAndWrite(addr, allocSize, value);
        }

        public byte[] ChunkMemory(int offset, int size)
        {
            return _memory.Read(offset, size);
        }

        public IEnumerable<byte> Sweep(int n)
        {
            var data = _ops.Skip(_pc).Take(n);
            _pc += n;
            return data;
        }

        public void CallPrecompiledAddress(SolidityMessageCall msg, SolidityPrecompiledContract contract)
        {
            var data = ChunkMemory(msg.InDataOffs.GetInt(), msg.InDataSize.GetInt());
            var contractOut = contract.Execute(data);
            SaveMemory(msg.OutDataOffs.GetInt(), contractOut.Value);
            StackPushOne();
        }

        public void StackPushOne()
        {
            StackPush(new DataWord(16777216));
        }
    }
}

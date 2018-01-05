using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityProgram
    {
        private readonly ICollection<byte> _ops;
        private readonly SolidityProgramInvoke _progInvoke;
        private int _pc;
        private List<DataWord> _stack;
        private SolidityMemory _memory;
        private SolidityProgramResult _result;
        private bool _stopped;

        public SolidityProgram(ICollection<byte> ops, SolidityProgramInvoke progInvoke)
        {
            _ops = ops;
            _progInvoke = progInvoke;
            _pc = 0;
            _stack = new List<DataWord>();
            _memory = new SolidityMemory();
            _result = new SolidityProgramResult();
            _stopped = false;
        }

        public byte GetCurrentOpCode()
        {
            return _ops.ElementAt(_pc);
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
            /*
            if (nextPC.bytesOccupied() > 4)
            {
                throw Program.Exception.badJumpDestination(-1);
            }

            int ret = nextPC.GetInt();
            if (!GetProgramPrecompile().hasJumpDest(ret))
            {
                throw Program.Exception.badJumpDestination(ret);
            }
            return ret;
            */
            return 0;
        }

        /*
        public ProgramPrecompile GetProgramPrecompile()
        {
            if (programPrecompile == null)
            {
                if (codeHash != null && commonConfig.precompileSource() != null)
                {
                    programPrecompile = commonConfig.precompileSource().get(codeHash);
                }

                if (programPrecompile == null)
                {
                    programPrecompile = ProgramPrecompile.compile(ops);

                    if (codeHash != null && commonConfig.precompileSource() != null)
                    {
                        commonConfig.precompileSource().put(codeHash, programPrecompile);
                    }
                }
            }
            return programPrecompile;
        }
        */
        public ICollection<byte> GetCode()
        {
            return _ops;
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

        public List<DataWord> GetStack()
        {
            return _stack;
        }

        public DataWord StackPop()
        {
            var result = _stack.Last();
            _stack.Remove(_stack.Last());
            return result;
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
    }
}

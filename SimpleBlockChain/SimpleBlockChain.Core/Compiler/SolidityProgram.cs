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
        private readonly List<IEnumerable<byte>> _memory;

        public SolidityProgram(ICollection<byte> ops, SolidityProgramInvoke progInvoke)
        {
            _ops = ops;
            _progInvoke = progInvoke;
            _pc = 0;
            _stack = new List<DataWord>();
            _memory = new List<IEnumerable<byte>>();
        }

        public byte GetCurrentOpCode()
        {
            return _ops.ElementAt(_pc);
        }

        public void SetPc(int pc)
        {
            _pc = pc;
        }
        
        public void Step()
        {
            SetPc(_pc + 1);
        }

        public void StackPush(IEnumerable<byte> data)
        {
            StackPush(new DataWord(data));
        }

        public void StackPush(DataWord data)
        {
            _stack.Add(data);
        }

        public DataWord StackPop()
        {
            var result = _stack.Last();
            _stack.Remove(_stack.Last());
            return result;
        }

        public IEnumerable<byte> GetDataCopy(DataWord offsetData, DataWord lengthData)
        {
            return _progInvoke.GetDataCopy(offsetData, lengthData);
        }

        public DataWord GetDataValue(DataWord data)
        {
            return _progInvoke.GetDataValue(data);
        }

        public void SaveMemory(IEnumerable<byte> value)
        {
            _memory.Add(value);
        }

        public List<IEnumerable<byte>> GetMemory()
        {
            return _memory;
        }

        public IEnumerable<byte> Sweep(int n)
        {
            var data = _ops.Skip(_pc).Take(n);
            _pc += n;
            return data;
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityProgram
    {
        private int _pc;
        private ICollection<byte> _ops;
        private List<IEnumerable<byte>> _stack = new List<IEnumerable<byte>>();

        public SolidityProgram(ICollection<byte> ops)
        {
            _ops = ops;
            _pc = 0;
            _stack = new List<IEnumerable<byte>>();
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
            _stack.Add(data);
        }

        public IEnumerable<byte> StackPop()
        {
            return _stack.Last();
        }

        public IEnumerable<byte> GetDataValue(IEnumerable<byte> data)
        {
            var result = new List<byte>();
            for (var i = 0; i < 32 - data.Count(); i++) {
                result.Add(0);
            }

            result.AddRange(data);
            return result;
        }

        public IEnumerable<byte> Sweep(int n)
        {
            var data = _ops.Skip(_pc).Take(n);
            _pc += n;
            return data;
        }
    }
}

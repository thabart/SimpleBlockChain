using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SimpleBlockChain.Core.Compiler
{
    public class DataWord
    {
        private IEnumerable<byte> _data;

        public DataWord(IEnumerable<byte> data)
        {
            Init(data);
        }

        private void Init(IEnumerable<byte> data)
        {
            var result = new List<byte>();
            for(var i = 0; i < 32 - data.Count(); i++)
            {
                result.Add(0);
            }

            result.AddRange(data);
            _data = result;
        }

        public IEnumerable<byte> GetData()
        {
            return _data;
        }

        public BigInteger GetValue()
        {
            return new BigInteger(GetDataWithoutFixSize().ToArray());
        }

        private IEnumerable<byte> GetDataWithoutFixSize()
        {
            var startIndex = 0;
            for(var i = 0; i < _data.Count(); i++)
            {
                var r = _data.ElementAt(i);
                if (r != 0x00)
                {
                    startIndex = i;
                }
            }

            return _data.Skip(startIndex);
        }
    }
}

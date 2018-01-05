using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityProgramInvoke
    {
        private static int _size = 32;
        private IEnumerable<byte> _msgDataRaw;
        private DataWord _address;

        public SolidityProgramInvoke(IEnumerable<byte> msgDataRaw, DataWord address)
        {
            _msgDataRaw = msgDataRaw;
            _address = address;
        }

        public DataWord GetOwnerAddress()
        {
            return _address;
        }

        public void SetOwnerAddress(DataWord address)
        {
            _address = address;
        }

        public IEnumerable<byte> GetDataCopy(DataWord offsetData, DataWord lengthData)
        {
            var offset = (int)offsetData.GetValue();
            var length = (int)lengthData.GetValue();
            byte[] data = new byte[length];
            if (_msgDataRaw == null)
            {
                return data;
            }

            if (offset > _msgDataRaw.Count())
            {
                return data;
            }

            var res = _msgDataRaw.Skip(offset).Take(length).ToList();
            var mod = res.Count % _size;
            if (mod != 0)
            {
                for (int i = 0; i < (_size - mod); i++)
                {
                    res.Add(0);
                }
            }

            return res;
        }

        public DataWord GetDataValue(DataWord indexData)
        {
            byte[] data = new byte[_size];
            var index = (int)indexData.GetValue();
            int size = _size;
            if (_msgDataRaw == null)
            {
                return new DataWord(data);
            }

            if (index > _msgDataRaw.Count())
            {
                return new DataWord(data);
            }

            if (index + _size > _msgDataRaw.Count())
            {
                size = _msgDataRaw.Count() - index;
            }

            var res = _msgDataRaw.Skip(index).Take(size).ToList();
            for (int i = res.Count(); i < _size; i++)
            {
                res.Add(0);
            }

            return new DataWord(res.ToArray());
        }
    }
}

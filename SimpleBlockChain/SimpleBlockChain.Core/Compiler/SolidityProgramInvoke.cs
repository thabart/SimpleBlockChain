using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityProgramInvoke
    {
        private IEnumerable<byte> _msgDataRaw;

        public SolidityProgramInvoke(IEnumerable<byte> msgDataRaw)
        {
            _msgDataRaw = msgDataRaw;
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
            return res;
        }

        public DataWord GetDataValue(DataWord indexData)
        {
            byte[] data = new byte[32];
            var index = (int)indexData.GetValue();
            int size = 32;
            if (_msgDataRaw == null)
            {
                return new DataWord(data);
            }

            if (index > _msgDataRaw.Count())
            {
                return new DataWord(data);
            }

            if (index + 32 > _msgDataRaw.Count())
            {
                size = _msgDataRaw.Count() - index;
            }

            var res = _msgDataRaw.Skip(index).Take(size).ToList();
            for (int i = res.Count(); i < 32; i++)
            {
                res.Add(0);
            }

            return new DataWord(res);
        }
    }
}

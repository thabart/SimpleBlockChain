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

        public DataWord GetDataValue(DataWord indexData)
        {
            byte[] data = new byte[32];
            var v = indexData.GetValue();
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

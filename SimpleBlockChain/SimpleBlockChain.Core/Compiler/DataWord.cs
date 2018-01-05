using Org.BouncyCastle.Math;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Compiler
{
    public class DataWord
    {
        private byte[] _data;
        public static DataWord ZERO = new DataWord(new byte[32]);
        public static DataWord ZERO_EMPTY_ARRAY = new DataWord(new byte[0]); 

        public DataWord()
        {
            _data = new byte[32];
        }

        public DataWord(byte[] data)
        {
            Init(data);
        }

        private void Init(byte[] data)
        {
            var result = new List<byte>();
            for(var i = 0; i < 32 - data.Count(); i++)
            {
                result.Add(0);
            }

            result.AddRange(data);
            _data = result.ToArray();
        }

        public DataWord And(DataWord w2)
        {
            for (int i = 0; i < _data.Count(); ++i)
            {
                _data[i] &= w2.GetData()[i];
            }

            return this;
        }

        public int GetInt()
        {
            int intVal = 0;
            foreach(byte aData in _data)
            {
                intVal = (intVal << 8) + (aData & 0xff);
            }

            return intVal;
        }

        public int GetIntValueSafe()
        {
            return GetInt();
            /*
            int bytesOccupied = bytesOccupied();
            int intValue = intValue();
            if (bytesOccupied > 4 || intValue < 0) return Integer.MAX_VALUE;
            return intValue;
            */
        }

        public DataWord Or(DataWord w2)
        {
            for (int i = 0; i < _data.Count(); ++i)
            {
                _data[i] |= w2.GetData()[i];
            }
            return this;
        }

        public DataWord XOR(DataWord w2)
        {

            for (int i = 0; i < _data.Count(); ++i)
            {
                _data[i] ^= w2.GetData()[i];
            }
            return this;
        }

        public byte[] GetData()
        {
            return _data;
        }

        public bool IsZero()
        {
            foreach (byte tmp in _data)
            {
                if (tmp != 0) return false;
            }

            return true;
        }

        public BigInteger GetValue()
        {
            return new BigInteger(1, _data);
        }

        public BigInteger GetSValue()
        {
            return new BigInteger(_data);
        }

        private byte[] GetDataWithoutFixSize()
        {
            var startIndex = 0;
            for(var i = 0; i < _data.Count(); i++)
            {
                var r = _data.ElementAt(i);
                if (r != 0x00)
                {
                    startIndex = i;
                    break;
                }
            }

            var result = _data.ToList().Skip(startIndex).ToArray();
            return result;
        }
    }
}

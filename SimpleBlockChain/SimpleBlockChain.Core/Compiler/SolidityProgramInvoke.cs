using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityProgramInvoke
    {
        private readonly SmartContracts _smartContracts;
        private static int _size = 32;
        private IEnumerable<byte> _msgDataRaw;
        private IEnumerable<byte> _smartContractAddress;
        private DataWord _ownerAddress;
        private DataWord _callValue;

        public SolidityProgramInvoke(IEnumerable<byte> smartContractAddress, DataWord ownerAddress, DataWord callValue, SmartContracts smartContracts)
        {
            _smartContractAddress = smartContractAddress;
            _ownerAddress = ownerAddress;
            _callValue = callValue;
            _smartContracts = smartContracts;
        }

        public SolidityProgramInvoke(IEnumerable<byte> msgDataRaw, IEnumerable<byte> smartContractAddress, DataWord ownerAddress, DataWord callValue, SmartContracts smartContracts)
        {
            _msgDataRaw = msgDataRaw;
            _smartContractAddress = smartContractAddress;
            _ownerAddress = ownerAddress;
            _callValue = callValue;
            _smartContracts = smartContracts;
        }

        public SmartContracts GetStorage()
        {
            return _smartContracts;
        }

        public IEnumerable<byte> GetSmartContractAddress()
        {
            return _smartContractAddress;
        }

        public void SetSmartContractAddress(IEnumerable<byte> smartContractAddress)
        {
            _smartContractAddress = smartContractAddress;
        }

        public DataWord GetCallValue()
        {
            return _callValue;
        }

        public DataWord GetOwnerAddress()
        {
            return _ownerAddress;
        }

        public void SetOwnerAddress(DataWord address)
        {
            _ownerAddress = address;
        }

        public DataWord GetDataSize()
        {
            int size = _msgDataRaw.Count();
            return new DataWord(size);
        }

        public IEnumerable<byte> GetDataCopy(DataWord offsetData, DataWord lengthData)
        {
            var offset = offsetData.GetValue().IntValue;
            var length = lengthData.GetValue().IntValue;
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
            var index = indexData.GetValue().IntValue;
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

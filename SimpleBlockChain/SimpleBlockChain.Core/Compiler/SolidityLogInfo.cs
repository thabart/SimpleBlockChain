using System.Collections.Generic;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityLogInfo
    {
        List<DataWord> _topics = new List<DataWord>();
        byte[] _data = new byte[] { };

        public SolidityLogInfo(List<DataWord> topics, byte[] data)
        {
            _topics = (topics != null) ? topics : new List<DataWord>();
            _data = (data != null) ? data : new byte[] { };
        }

        public List<DataWord> GetTopics()
        {
            return _topics;
        }

        public byte[] GetData()
        {
            return _data;
        }
    }
}

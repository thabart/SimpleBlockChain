using System.Collections.Generic;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityStorage
    {
        private  Dictionary<DataWord, DataWord> _storageCache;

        public SolidityStorage()
        {
            _storageCache = new Dictionary<DataWord, DataWord>();
        }

        public void AddStorageRow(DataWord key, DataWord value)
        {
            _storageCache.Add(key, value);
        }
    }
}

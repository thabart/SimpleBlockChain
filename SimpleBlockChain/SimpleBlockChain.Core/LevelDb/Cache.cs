using System;

namespace SimpleBlockChain.Core.LevelDb
{
    public class Cache
    {
        public IntPtr Handle { get; private set; }
        public int Capacity { get; private set; }

        public Cache(int capacity)
        {
            Capacity = capacity;
            Handle = Native.leveldb_cache_create_lru((UIntPtr)capacity);
        }

        ~Cache()
        {
            Native.leveldb_cache_destroy(Handle);
        }
    }
}

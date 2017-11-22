using System;

namespace SimpleBlockChain.Core.LevelDb
{
    public class ReadOptions
    {
        public static readonly ReadOptions Default = new ReadOptions();
        internal readonly IntPtr handle = Native.leveldb_readoptions_create();

        public bool VerifyChecksums
        {
            set
            {
                Native.leveldb_readoptions_set_verify_checksums(handle, value);
            }
        }

        public bool FillCache
        {
            set
            {
                Native.leveldb_readoptions_set_fill_cache(handle, value);
            }
        }

        public Snapshot Snapshot
        {
            set
            {
                Native.leveldb_readoptions_set_snapshot(handle, value.Handle);
            }
        }

        ~ReadOptions()
        {
            Native.leveldb_readoptions_destroy(handle);
        }
    }
}

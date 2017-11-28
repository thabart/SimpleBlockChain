using System;

namespace SimpleBlockChain.Core.LevelDb
{
    public class Options
    {
#pragma warning disable 414
        Cache f_BlockCache;
#pragma warning restore 414

        public IntPtr Handle { get; private set; }
        public bool CreateIfMissing
        {
            set
            {
                Native.leveldb_options_set_create_if_missing(Handle, value);
            }
        }

        public bool ErrorIfExists
        {
            set
            {
                Native.leveldb_options_set_error_if_exists(Handle, value);
            }
        }

        public bool ParanoidChecks
        {
            set
            {
                Native.leveldb_options_set_paranoid_checks(Handle, value);
            }
        }

        public int WriteBufferSize
        {
            set
            {
                Native.leveldb_options_set_write_buffer_size(Handle, value);
            }
        }

        public int MaxOpenFiles
        {
            set
            {
                Native.leveldb_options_set_max_open_files(Handle, value);
            }
        }

        public Cache BlockCache
        {
            set
            {
                // keep a reference to Cache so it doesn't get GCed
                f_BlockCache = value;
                if (value == null)
                {
                    Native.leveldb_options_set_cache(Handle, IntPtr.Zero);
                }
                else
                {
                    Native.leveldb_options_set_cache(Handle, value.Handle);
                }
            }
        }

        public int BlockSize
        {
            set
            {
                Native.leveldb_options_set_block_size(Handle, value);
            }
        }

        public int BlockRestartInterval
        {
            set
            {
                Native.leveldb_options_set_write_buffer_size(Handle, value);
            }
        }

        public CompressionType Compression
        {
            set
            {
                Native.leveldb_options_set_compression(Handle, (int)value);
            }
        }

        public Options()
        {
            Handle = Native.leveldb_options_create();
        }

        ~Options()
        {
            Native.leveldb_options_destroy(Handle);
        }
    }
}

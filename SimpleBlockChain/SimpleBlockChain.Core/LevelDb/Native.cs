using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SimpleBlockChain.Core.LevelDb
{
    public enum CompressionType : byte
    {
        kNoCompression = 0x0,
        kSnappyCompression = 0x1
    }

    public static class Native
    {
        public static void CheckError(string error)
        {
            if (String.IsNullOrEmpty(error))
            {
                return;
            }
        }

        public static UIntPtr GetStringLength(string value)
        {
            if (value == null || value.Length == 0)
            {
                return UIntPtr.Zero;
            }
            return new UIntPtr((uint)System.Text.Encoding.UTF8.GetByteCount(value));
        }

#if NET46
        static Native()
        {
            string platform = IntPtr.Size == 8 ? "x64" : "x86";
            LoadLibrary(Path.Combine(AppContext.BaseDirectory, platform, "leveldb"));
        }

        [DllImport("kernel32")]
        private static extern IntPtr LoadLibrary(string dllToLoad);
#endif

        [DllImport("leveldb", CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_compact_range(IntPtr db,
                                                        string startKey,
                                                        UIntPtr startKeyLen,
                                                        string limitKey,
                                                        UIntPtr limitKeyLen);
        public static void leveldb_compact_range(IntPtr db,
                                                 string startKey,
                                                 string limitKey)
        {
            leveldb_compact_range(db,
                                  startKey, GetStringLength(startKey),
                                  limitKey, GetStringLength(limitKey));
        }
        #region Logger
        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_logger_create(IntPtr /* Action<string> */ logger);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_logger_destroy(IntPtr /* logger*/ option);
        #endregion

        public static string GetAndReleaseString(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) {
                return null;
            }

            var str = Marshal.PtrToStringAnsi(ptr);
            leveldb_free(ptr);
            return str;
        }

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_open(IntPtr /* Options*/ options, string name, out IntPtr error);

        public static IntPtr leveldb_open(IntPtr options, string name, out string error)
        {
            IntPtr errorPtr;
            var db = leveldb_open(options, name, out errorPtr);
            error = GetAndReleaseString(errorPtr);
            return db;
        }

        public static IntPtr leveldb_open(IntPtr options, string name)
        {
            string error;
            var db = leveldb_open(options, name, out error);
            CheckError(error);
            return db;
        }


        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_close(IntPtr /*DB */ db);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_put(IntPtr db,
                                              IntPtr writeOptions,
                                              string key,
                                              UIntPtr keyLength,
                                              string value,
                                              UIntPtr valueLength,
                                              out IntPtr error);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_delete(IntPtr db, IntPtr writeOptions, string key, UIntPtr keylen, out IntPtr error);
        public static void leveldb_delete(IntPtr db, IntPtr writeOptions, string key, UIntPtr keylen, out string error)
        {
            IntPtr errorPtr;
            leveldb_delete(db, writeOptions, key, keylen, out errorPtr);
            error = GetAndReleaseString(errorPtr);
        }

        public static void leveldb_delete(IntPtr db, IntPtr writeOptions, string key)
        {
            string error;
            var keyLength = GetStringLength(key);
            leveldb_delete(db, writeOptions, key, keyLength, out error);
            CheckError(error);
        }

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_write(IntPtr /* DB */ db, IntPtr /* WriteOptions*/ options, IntPtr /* WriteBatch */ batch, out IntPtr errptr);
        public static void leveldb_write(IntPtr db, IntPtr writeOptions, IntPtr writeBatch, out string error)
        {
            IntPtr errorPtr;
            leveldb_write(db, writeOptions, writeBatch, out errorPtr);
            error = GetAndReleaseString(errorPtr);
        }

        public static void leveldb_write(IntPtr db, IntPtr writeOptions, IntPtr writeBatch)
        {
            string error;
            leveldb_write(db, writeOptions, writeBatch, out error);
            CheckError(error);
        }

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_get(IntPtr db,
                                                IntPtr readOptions,
                                                string key,
                                                UIntPtr keyLength,
                                                out UIntPtr valueLength,
                                                out IntPtr error);
        public static IntPtr leveldb_get(IntPtr db,
                                         IntPtr readOptions,
                                         string key,
                                         UIntPtr keyLength,
                                         out UIntPtr valueLength,
                                         out string error)
        {
            IntPtr errorPtr;
            var valuePtr = leveldb_get(db, readOptions, key, keyLength,
                                       out valueLength, out errorPtr);
            error = GetAndReleaseString(errorPtr);
            return valuePtr;
        }

        public static string leveldb_get(IntPtr db,
                                         IntPtr readOptions,
                                         string key)
        {
            UIntPtr valueLength;
            string error;
            var keyLength = GetStringLength(key);
            var valuePtr = leveldb_get(db, readOptions, key, keyLength,
                                       out valueLength, out error);
            CheckError(error);
            if (valuePtr == IntPtr.Zero || valueLength == UIntPtr.Zero)
            {
                return null;
            }

            var value = Marshal.PtrToStringAnsi(valuePtr, (int)valueLength);
            // leveldb_free(valuePtr);
            return value;
        }

        //[DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        //static extern void leveldb_approximate_sizes(IntPtr /* DB */ db, int num_ranges, byte[] range_start_key, long range_start_key_len, byte[] range_limit_key, long range_limit_key_len, out long sizes);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_create_iterator(IntPtr /* DB */ db, IntPtr /* ReadOption */ options);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_create_snapshot(IntPtr /* DB */ db);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_release_snapshot(IntPtr /* DB */ db, IntPtr /* SnapShot*/ snapshot);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_property_value_native(IntPtr /* DB */ db, string propname);
        public static string leveldb_property_value(IntPtr db, string propname)
        {
            var valuePtr = leveldb_property_value_native(db, propname);
            if (valuePtr == IntPtr.Zero)
            {
                return null;
            }
            var value = Marshal.PtrToStringAnsi(valuePtr);
            leveldb_free(valuePtr);
            return value;
        }

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_repair_db(IntPtr /* Options*/ options, string name, out IntPtr error);
        public static void leveldb_repair_db(IntPtr options, string path, out string error)
        {
            IntPtr errorPtr;
            leveldb_repair_db(options, path, out errorPtr);
            error = GetAndReleaseString(errorPtr);
        }

        public static void leveldb_repair_db(IntPtr options, string path)
        {
            string error;
            leveldb_repair_db(options, path, out error);
            CheckError(error);
        }

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_destroy_db(IntPtr /* Options*/ options, string name, out IntPtr error);

        public static void leveldb_destroy_db(IntPtr options, string path, out string error)
        {
            IntPtr errorPtr;
            leveldb_destroy_db(options, path, out errorPtr);
            error = GetAndReleaseString(errorPtr);
        }

        public static void leveldb_destroy_db(IntPtr options, string path)
        {
            string error;
            leveldb_destroy_db(options, path, out error);
            CheckError(error);
        }


        #region extensions 

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_free(IntPtr /* void */ ptr);

        #endregion

        #region Env
        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_create_default_env();

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_env_destroy(IntPtr /*Env*/ cache);
        #endregion

        #region Iterator
        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_iter_destroy(IntPtr /*Iterator*/ iterator);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool leveldb_iter_valid(IntPtr /*Iterator*/ iterator);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_iter_seek_to_first(IntPtr /*Iterator*/ iterator);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_iter_seek_to_last(IntPtr /*Iterator*/ iterator);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_iter_seek(IntPtr iter, string key, UIntPtr keyLength);
        public static void leveldb_iter_seek(IntPtr iter, string key)
        {
            var keyLength = GetStringLength(key);
            leveldb_iter_seek(iter, key, keyLength);
        }


        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_iter_next(IntPtr /*Iterator*/ iterator);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_iter_prev(IntPtr /*Iterator*/ iterator);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_iter_key(IntPtr /*Iterator*/ iterator, out UIntPtr length);
        public static string leveldb_iter_key(IntPtr iter)
        {
            UIntPtr keyLength;
            var keyPtr = leveldb_iter_key(iter, out keyLength);
            if (keyPtr == IntPtr.Zero || keyLength == UIntPtr.Zero)
            {
                return null;
            }
            var key = Marshal.PtrToStringAnsi(keyPtr, (int)keyLength);
            return key;
        }


        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_iter_value(IntPtr /*Iterator*/ iterator, out UIntPtr length);
        public static string leveldb_iter_value(IntPtr iter)
        {
            UIntPtr valueLength;
            var valuePtr = leveldb_iter_value(iter, out valueLength);
            if (valuePtr == IntPtr.Zero || valueLength == UIntPtr.Zero)
            {
                return null;
            }
            var value = Marshal.PtrToStringAnsi(valuePtr, (int)valueLength);
            return value;
        }

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_iter_get_error(IntPtr /*Iterator*/ iterator, out IntPtr error);
        #endregion

        #region Options
        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_options_create();

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_options_destroy(IntPtr /*Options*/ options);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_options_set_create_if_missing(IntPtr /*Options*/ options, [MarshalAs(UnmanagedType.U1)] bool o);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_options_set_error_if_exists(IntPtr /*Options*/ options, [MarshalAs(UnmanagedType.U1)] bool o);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_options_set_info_log(IntPtr /*Options*/ options, IntPtr /* Logger */ logger);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_options_set_paranoid_checks(IntPtr /*Options*/ options, [MarshalAs(UnmanagedType.U1)] bool o);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_options_set_env(IntPtr /*Options*/ options, IntPtr /*Env*/ env);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_options_set_write_buffer_size(IntPtr /*Options*/ options, UIntPtr size);
        public static void leveldb_options_set_write_buffer_size(IntPtr options, int size)
        {
            leveldb_options_set_write_buffer_size(options, (UIntPtr)size);
        }

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_options_set_max_open_files(IntPtr /*Options*/ options, int max);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_options_set_cache(IntPtr /*Options*/ options, IntPtr /*Cache*/ cache);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_options_set_block_size(IntPtr /*Options*/ options, UIntPtr size);
        public static void leveldb_options_set_block_size(IntPtr options, int size)
        {
            leveldb_options_set_block_size(options, (UIntPtr)size);
        }

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_options_set_block_restart_interval(IntPtr /*Options*/ options, int interval);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_options_set_compression(IntPtr /*Options*/ options, int level);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_options_set_comparator(IntPtr /*Options*/ options, IntPtr /*Comparator*/ comparer);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_options_set_filter_policy(IntPtr /*Options*/ options, IntPtr /*FilterPolicy*/ policy);
        #endregion

        #region ReadOptions
        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_readoptions_create();

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_readoptions_destroy(IntPtr /*ReadOptions*/ options);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_readoptions_set_verify_checksums(IntPtr /*ReadOptions*/ options, [MarshalAs(UnmanagedType.U1)] bool o);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_readoptions_set_fill_cache(IntPtr /*ReadOptions*/ options, [MarshalAs(UnmanagedType.U1)] bool o);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_readoptions_set_snapshot(IntPtr /*ReadOptions*/ options, IntPtr /*SnapShot*/ snapshot);
        #endregion

        #region WriteBatch
        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_writebatch_create();

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_writebatch_destroy(IntPtr /* WriteBatch */ batch);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_writebatch_clear(IntPtr /* WriteBatch */ batch);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_writebatch_put(IntPtr /* WriteBatch */ batch, byte[] key, UIntPtr keylen, byte[] val, UIntPtr vallen);

        public static void leveldb_put(IntPtr db,
                                       IntPtr writeOptions,
                                       string key,
                                       UIntPtr keyLength,
                                       string value,
                                       UIntPtr valueLength,
                                       out string error)
        {
            IntPtr errorPtr;
            leveldb_put(db, writeOptions, key, keyLength, value, valueLength,
                        out errorPtr);
            error = GetAndReleaseString(errorPtr);
        }

        public static void leveldb_put(IntPtr db,
                                       IntPtr writeOptions,
                                       string key,
                                       string value)
        {
            string error;
            var keyLength = GetStringLength(key);
            var valueLength = GetStringLength(value);
            Native.leveldb_put(db, writeOptions,
                               key, keyLength,
                               value, valueLength, out error);
            CheckError(error);
        }


        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_writebatch_delete(IntPtr /* WriteBatch */ batch, byte[] key, UIntPtr keylen);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_writebatch_iterate(IntPtr /* WriteBatch */ batch, object state, Action<object, byte[], int, byte[], int> put, Action<object, byte[], int> deleted);
        #endregion

        #region WriteOptions
        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_writeoptions_create();

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_writeoptions_destroy(IntPtr /*WriteOptions*/ options);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_writeoptions_set_sync(IntPtr /*WriteOptions*/ options, [MarshalAs(UnmanagedType.U1)] bool o);
        #endregion

        #region Cache 
        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_cache_create_lru(UIntPtr capacity);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_cache_destroy(IntPtr /*Cache*/ cache);
        #endregion

        #region Comparator

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* leveldb_comparator_t* */
            leveldb_comparator_create(
            IntPtr /* void* */ state,
            IntPtr /* void (*)(void*) */ destructor,
            IntPtr
                /* int (*compare)(void*,
                                  const char* a, size_t alen,
                                  const char* b, size_t blen) */
                compare,
            IntPtr /* const char* (*)(void*) */ name);

        [DllImport("leveldb", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_comparator_destroy(IntPtr /* leveldb_comparator_t* */ cmp);

        #endregion
    }

    internal static class NativeHelper
    {
        public static void CheckError(IntPtr error)
        {
            if (error != IntPtr.Zero)
            {
                string message = Marshal.PtrToStringAnsi(error);
                Native.leveldb_free(error);
                throw new Exception(message);
            }
        }
    }
}

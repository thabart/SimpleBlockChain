using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.LevelDb
{
    public class Iterator : IEnumerator<KeyValuePair<string, string>>
    {
        /// <summary>
        /// Native handle
        /// </summary>
        public IntPtr Handle { get; private set; }
        DB DB { get; set; }
        ReadOptions ReadOptions { get; set; }
        bool IsFirstMove { get; set; }

        public bool IsValid
        {
            get
            {
                return Native.leveldb_iter_valid(Handle);
            }
        }

        public string Key
        {
            get
            {
                return Native.leveldb_iter_key(Handle);
            }
        }

        public string Value
        {
            get
            {
                return Native.leveldb_iter_value(Handle);
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public KeyValuePair<string, string> Current
        {
            get
            {
                return new KeyValuePair<string, string>(Key, Value);
            }
        }

        public Iterator(DB db, ReadOptions readOptions)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            DB = db;
            // keep reference so it doesn't get GCed
            ReadOptions = readOptions;
            if (ReadOptions == null)
            {
                ReadOptions = new ReadOptions();
            }
            Handle = Native.leveldb_create_iterator(db.Handle, ReadOptions.handle);
            IsFirstMove = true;
        }

        ~Iterator()
        {
            if (DB.Handle != IntPtr.Zero)
            {
                Native.leveldb_iter_destroy(Handle);
            }
        }

        public void SeekToFirst()
        {
            Native.leveldb_iter_seek_to_first(Handle);
        }

        public void SeekToLast()
        {
            Native.leveldb_iter_seek_to_last(Handle);
        }

        public void Seek(string key)
        {
            Native.leveldb_iter_seek(Handle, key);
        }

        public void Previous()
        {
            Native.leveldb_iter_prev(Handle);
        }

        public void Next()
        {
            Native.leveldb_iter_next(Handle);
        }

        public void Reset()
        {
            IsFirstMove = true;
            SeekToFirst();
        }

        public bool MoveNext()
        {
            if (IsFirstMove)
            {
                SeekToFirst();
                IsFirstMove = false;
                return IsValid;
            }
            Next();
            return IsValid;
        }

        public void Dispose()
        {
            // ~Iterator() takes already care
        }
    }
}

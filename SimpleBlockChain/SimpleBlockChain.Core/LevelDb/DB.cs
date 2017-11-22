using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.LevelDb
{
    public class DB : IDisposable, IEnumerable<KeyValuePair<string, string>>
    {
        public IntPtr Handle { get; private set; }
        Options Options { get; set; }
        bool Disposed { get; set; }

        public string this[string key]
        {
            get
            {
                return Get(null, key);
            }
            set
            {
                Put(null, key, value);
            }
        }

        public DB(Options options, string path)
        {
            if (options == null)
            {
                options = new Options();
            }

            Options = options;
            Handle = Native.leveldb_open(options.Handle, path);
        }

        ~DB()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            var disposed = Disposed;
            if (disposed)
            {
                return;
            }
            Disposed = true;

            if (disposing)
            {
                // free managed
                Options = null;
            }
            // free unmanaged
            var handle = Handle;
            if (handle != IntPtr.Zero)
            {
                Handle = IntPtr.Zero;
                Native.leveldb_close(handle);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static DB Open(Options options, string path)
        {
            return new DB(options, path);
        }

        public static void Repair(Options options, string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (options == null)
            {
                options = new Options();
            }
            Native.leveldb_repair_db(options.Handle, path);
        }

        public static void Destroy(Options options, string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (options == null)
            {
                options = new Options();
            }
            Native.leveldb_destroy_db(options.Handle, path);
        }

        public void Put(WriteOptions options, string key, string value)
        {
            CheckDisposed();
            if (options == null)
            {
                options = new WriteOptions();
            }

            Native.leveldb_put(Handle, options.handle, key, value);
        }

        public void Put(string key, string value)
        {
            Put(null, key, value);
        }

        public void Delete(WriteOptions options, string key)
        {
            CheckDisposed();
            if (options == null)
            {
                options = new WriteOptions();
            }
            Native.leveldb_delete(Handle, options.handle, key);
        }

        public void Delete(string key)
        {
            Delete(null, key);
        }

        public void Write(WriteOptions writeOptions, WriteBatch writeBatch)
        {
            CheckDisposed();
            if (writeOptions == null)
            {
                writeOptions = new WriteOptions();
            }
            if (writeBatch == null)
            {
                throw new ArgumentNullException("writeBatch");
            }
            Native.leveldb_write(Handle, writeOptions.handle, writeBatch.handle);
        }

        public void Write(WriteBatch writeBatch)
        {
            Write(null, writeBatch);
        }

        public string Get(ReadOptions options, string key)
        {
            CheckDisposed();
            if (options == null)
            {
                options = new ReadOptions();
            }
            return Native.leveldb_get(Handle, options.handle, key);
        }

        public string Get(string key)
        {
            return Get(null, key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            CheckDisposed();
            return new Iterator(this, null);
        }

        public Snapshot CreateSnapshot()
        {
            CheckDisposed();
            return new Snapshot(this);
        }

        public void Compact()
        {
            CompactRange(null, null);
        }
        
        public void CompactRange(string startKey, string limitKey)
        {
            CheckDisposed();
            Native.leveldb_compact_range(Handle, startKey, limitKey);
        }

        public string GetProperty(string property)
        {
            CheckDisposed();
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            return Native.leveldb_property_value(Handle, property);
        }

        void CheckDisposed()
        {
            if (!Disposed)
            {
                return;
            }
            throw new ObjectDisposedException(this.GetType().Name);
        }
    }
}

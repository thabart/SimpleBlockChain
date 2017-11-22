using System;

namespace SimpleBlockChain.Core.LevelDb
{
    public class Snapshot
    {
        /// <summary>
        /// Native handle
        /// </summary>
        public IntPtr Handle { get; private set; }
        DB DB { get; set; }

        public Snapshot(DB db)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            DB = db;
            Handle = Native.leveldb_create_snapshot(db.Handle);
        }

        ~Snapshot()
        {
            var db = DB.Handle;
            if (db != IntPtr.Zero)
            {
                Native.leveldb_release_snapshot(db, Handle);
            }
        }
    }
}

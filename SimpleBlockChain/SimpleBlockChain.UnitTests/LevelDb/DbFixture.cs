using LevelDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace SimpleBlockChain.UnitTests.LevelDb
{
    [TestClass]
    public class DbFixture
    {
        public static byte[] ToByteArray(string HexString)
        {
            int NumberChars = HexString.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 16);
            }
            return bytes;
        }

        [TestMethod]
        public void WhenWriteAndReadSomeData()
        {
            var databasePath = @"c:\Project\SimpleBlockChain\SimpleBlockChain\db.dat";
            var options = new Options
            {
                CreateIfMissing = true
            };
            var database = new DB(databasePath, options);
            database.Put("key1", "key1");
            database.Put("key2", "key2");

            var res = database.Find(ReadOptions.Default, "key");
            database.Delete("key2");

            string s = "";
        }
    }
}

using LevelDB;
using System;
using System.Linq;
using System.Numerics;
using System.Text;
using Xunit;

namespace SimpleBlockChain.UnitTests.LevelDb
{
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

        [Fact]
        public void WhenWriteAndReadSomeData()
        {
            var databasePath = @"c:\Project\SimpleBlockChain\SimpleBlockChain\db.dat";
            var options = new Options
            {
                CreateIfMissing = true
            };
            var database = DB.Open(databasePath, options);
            database.Put(null, "key1", "value1");
            database.Put(null, "key2", "value2");
            database.Put(null, "key3", "value3");
            database.Put(null, "key4", "value4");

            var value1 = database.Get(null, "key1");
            var value2 = database.Get(null, "key2");
            var value3 = database.Get(null, "key3");
            var value4 = database.Get(null, "key4");

            database.Delete(null, "key2");

            string s = "";
        }
    }
}

using SimpleBlockChain.Core.LevelDb;
using Xunit;

namespace SimpleBlockChain.UnitTests.LevelDb
{
    public class DbFixture
    {
        [Fact]
        public void WhenWriteAndReadSomeData()
        {
            var databasePath = @"c:\Project\SimpleBlockChain\SimpleBlockChain\db.dat";
            var options = new Options
            {
                CreateIfMissing = true
            };
            var database = new DB(options, databasePath);
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

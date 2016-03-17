using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Movie_Searcher;

namespace MovieSearcherTest
{
    [TestClass]
    public class DatabaseTests
    {
        [TestMethod]
        public void DatabaseFileCreate()
        {
            File.Delete("database.db");
            var db = new Database();
            db.Load();
            db.Dispose();
            Assert.IsTrue(File.Exists("database.db"));
            File.Delete("database.db");
        }
    }
}
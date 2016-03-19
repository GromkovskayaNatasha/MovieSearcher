using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Movie_Searcher;
using Movie_Searcher.Model;

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

        [TestMethod]
        public void AddRecord()
        {
            File.Delete("database.db");
            var db = new Database();
            db.Load();
            var m = new Movie
            {
                Country = "country1",
                Id = "id1",
                Plot = "plot1",
                Poster = "poster1",
                Runtime = "110 min",
                Success = true,
                Title = "title1",
                Year = "1990"
            };
            db.AddToFavorites(m);
            db.Dispose();

            var db2 = new Database();
            db2.Load();
            var favs = db2.GetAllFavorites().ToArray();
            db2.Dispose();
            Assert.AreEqual(favs.Length, 1);
            Assert.AreEqual(favs[0].Title, m.Title);
            Assert.AreEqual(favs[0].Country, m.Country);
            Assert.AreEqual(favs[0].Id, m.Id);
            Assert.AreEqual(favs[0].Plot, m.Plot);
            Assert.AreEqual(favs[0].Poster, m.Poster);
            Assert.AreEqual(favs[0].Runtime, m.Runtime);
            Assert.AreEqual(favs[0].Success, m.Success);
            Assert.AreEqual(favs[0].Year, m.Year);
            File.Delete("database.db");
        }

        [TestMethod]
        public void DeleteRecord()
        {
            File.Delete("database.db");
            var db = new Database();
            db.Load();
            var m = new Movie
            {
                Country = "country1",
                Id = "id1",
                Plot = "plot1",
                Poster = "poster1",
                Runtime = "110 min",
                Success = true,
                Title = "title1",
                Year = "1990"
            };
            db.AddToFavorites(m);
            db.Dispose();

            var db2 = new Database();
            db2.Load();
            db2.RemoveFromFavorites(m.Id);
            db2.Dispose();

            var db3 = new Database();
            db3.Load();
            var favs = db3.GetAllFavorites().ToArray();
            db3.Dispose();
            Assert.AreEqual(favs.Length, 0);
            File.Delete("database.db");
        }

        [TestMethod]
        public void CheckRecordExistance()
        {
            File.Delete("database.db");
            var db = new Database();
            db.Load();
            var m = new Movie
            {
                Country = "country1",
                Id = "id1",
                Plot = "plot1",
                Poster = "poster1",
                Runtime = "110 min",
                Success = true,
                Title = "title1",
                Year = "1990"
            };
            db.AddToFavorites(m);
            db.Dispose();

            var db2 = new Database();
            db2.Load();
            Assert.IsTrue(db2.MovieExists(m.Id));
            Assert.IsFalse(db2.MovieExists("some non-existing id"));
            db2.Dispose();
            File.Delete("database.db");
        }
    }
}
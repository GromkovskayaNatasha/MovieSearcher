using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Movie_Searcher.Model;

namespace Movie_Searcher
{
    public class Database : IDisposable
    {
        private const string DbName = @"database.db";
        private SQLiteConnection _connection;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerable<Movie> GetAllFavourites()
        {
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = @"SELECT * FROM favourites;";
                command.CommandType = CommandType.Text;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    yield return
                        new Movie
                        {
                            Title = reader["title"].ToString(),
                            Id = reader["id"].ToString(),
                            Year = reader["year"].ToString(),
                            Success = true,
                            Country = reader["country"].ToString(),
                            Runtime = reader["runtime"].ToString(),
                            Plot = reader["plot"].ToString(),
                            Poster = reader["poster"].ToString()
                        };
                }
            }
        }

        public void AddToFavourites(Movie movie)
        {
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = string.Format(@"INSERT INTO favourites
                (title, year, runtime, plot, country, poster, id)
                VALUES (""{0}"", ""{1}"", ""{2}"", ""{3}"", ""{4}"", ""{5}"", ""{6}"" );", movie.Title, movie.Year,
                    movie.Runtime, movie.Plot, movie.Country, movie.Poster, movie.Id);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
        }

        public void RemoveFromFavourites(string id)
        {
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = string.Format(@"DELETE FROM favourites WHERE id = ""{0}"";", id);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
        }

        public bool MovieExists(string id)
        {
            var exists = false;
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = string.Format(@"SELECT id FROM favourites WHERE id = ""{0}"";", id);
                command.CommandType = CommandType.Text;
                var reader = command.ExecuteReader();
                exists = reader.Read();
            }

            return exists;
        }

        public void Load()
        {
            if (File.Exists(DbName))
            {
                InitiateConnection();
            }
            else
            {
                SQLiteConnection.CreateFile(DbName);
                InitiateConnection();

                using (var command = new SQLiteCommand(_connection))
                {
                    command.CommandText = @"CREATE TABLE favourites (
                    ""index"" integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                    title char(100) NOT NULL,
                    year char(100) NOT NULL,
                    runtime char(100) NOT NULL,
                    plot char(100) NOT NULL,
                    country char(100) NOT NULL,
                    poster char(100) NOT NULL,
                    id char(100) NOT NULL
                    );";
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }

        private void InitiateConnection()
        {
            _connection = new SQLiteConnection("data source='" + DbName + "'");
            _connection.Open();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection.Dispose();
            }
        }
    }
}
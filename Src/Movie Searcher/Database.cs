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
                command.CommandText = @"INSERT INTO favourites
                (title, year, runtime, plot, country, poster, id)
                VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7);";
                command.CommandType = CommandType.Text;
                command.Parameters.Add(new SQLiteParameter("@p1", movie.Title));
                command.Parameters.Add(new SQLiteParameter("@p2", movie.Year));
                command.Parameters.Add(new SQLiteParameter("@p3", movie.Runtime));
                command.Parameters.Add(new SQLiteParameter("@p4", movie.Plot));
                command.Parameters.Add(new SQLiteParameter("@p5", movie.Country));
                command.Parameters.Add(new SQLiteParameter("@p6", movie.Poster));
                command.Parameters.Add(new SQLiteParameter("@p7", movie.Id));
                command.ExecuteNonQuery();
            }
        }

        public void RemoveFromFavourites(string id)
        {
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = @"DELETE FROM favourites WHERE id = @p1;";
                command.CommandType = CommandType.Text;
                command.Parameters.Add(new SQLiteParameter("@p1", id));
                command.ExecuteNonQuery();
            }
        }

        public bool MovieExists(string id)
        {
            bool exists;
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = @"SELECT id FROM favourites WHERE id = @p1;";
                command.CommandType = CommandType.Text;
                command.Parameters.Add(new SQLiteParameter("@p1", id));
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
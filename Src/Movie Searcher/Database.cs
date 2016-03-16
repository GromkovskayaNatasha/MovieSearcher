using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Movie_Searcher.Model;

namespace Movie_Searcher
{
    public class Database
    {
        private const string DbName = @"database.db";
        private const string TableName = @"Favourites";
        private DataSet _dataSet;
        private DataTable _dataTable;

        public IEnumerable<Movie> GetAllFavourites()
        {
            return from row in _dataTable.AsEnumerable()
                select new Movie
                {
                    Country = row.Field<string>("Country"),
                    Id = row.Field<string>("Id"),
                    Plot = row.Field<string>("Plot"),
                    Poster = row.Field<string>("Poster"),
                    Runtime = row.Field<string>("Runtime"),
                    Title = row.Field<string>("Title"),
                    Year = row.Field<string>("Year"),
                    Success = true
                };
        }

        public void AddToFavourites(Movie movie)
        {
            _dataTable.Rows.Add(movie.Title, movie.Year, movie.Runtime, movie.Plot, movie.Country, movie.Poster,
                movie.Id);
        }

        public void RemoveFromFavourites(string id)
        {
            for (var i = 0; i < _dataTable.Rows.Count; ++i)
            {
                if (_dataTable.Rows[i]["Id"].ToString() == id)
                {
                    _dataTable.Rows.RemoveAt(i);
                    break;
                }
            }
        }

        public bool MovieExists(string id)
        {
            return (from row in _dataTable.AsEnumerable()
                where row.Field<string>("Id") == id
                select true).Any();
        }

        public void Load()
        {
            _dataSet = new DataSet();
            if (File.Exists(DbName))
            {
                _dataSet.ReadXml(DbName);
                _dataTable = _dataSet.Tables[TableName];
            }
            else
            {
                _dataTable = new DataTable(TableName);
                _dataTable.Columns.AddRange(new[]
                {
                    new DataColumn("Title"), new DataColumn("Year"), new DataColumn("Runtime"), new DataColumn("Plot"),
                    new DataColumn("Country"), new DataColumn("Poster"), new DataColumn("Id")
                });
                _dataSet.Tables.Add(_dataTable);
            }
        }

        public void Save()
        {
            if (_dataTable.Rows.Count > 0) _dataSet.WriteXml(DbName);
        }
    }
}
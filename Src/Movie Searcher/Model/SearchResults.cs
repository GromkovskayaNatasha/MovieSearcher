using System.Collections.Generic;
using RestSharp.Deserializers;

namespace Movie_Searcher.Model
{
    public class SearchResults
    {
        [DeserializeAs(Name = "Search")]
        public List<MovieSummary> Movies { get; set; }

        [DeserializeAs(Name = "Response")]
        public bool Success { get; set; }
    }
}
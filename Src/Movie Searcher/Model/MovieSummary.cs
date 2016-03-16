using RestSharp.Deserializers;

namespace Movie_Searcher.Model
{
    public class MovieSummary
    {
        public string Title { get; set; }
        public string Year { get; set; }

        [DeserializeAs(Name = "imdbID")]
        public string Id { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, {1}", Title, Year);
        }
    }
}
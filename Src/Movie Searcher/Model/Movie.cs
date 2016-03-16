using RestSharp.Deserializers;

namespace Movie_Searcher.Model
{
    public class Movie
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string Runtime { get; set; }
        public string Plot { get; set; }
        public string Country { get; set; }
        public string Poster { get; set; }
        public string Id { get; set; }
        [DeserializeAs(Name = "Response")]
        public bool Success { get; set; }
    }
}
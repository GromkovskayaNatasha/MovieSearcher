using System;
using System.Collections.Generic;
using Movie_Searcher.Model;
using RestSharp;

namespace Movie_Searcher
{
    public class Api
    {
        private readonly RestClient _client = new RestClient(@"http://www.omdbapi.com");
        private readonly object _locker = new object();
        private RestRequestAsyncHandle _currentMovieRequestHandle;

        public void Search(string title, Action<List<MovieSummary>> callback)
        {
            var request = new RestRequest("?s={title}&r=json", Method.GET);
            request.AddUrlSegment("title", title);
            _client.ExecuteAsync<SearchResults>(request, response =>
            {
                if (response.Data != null && response.Data.Success)
                {
                    callback(response.Data.Movies);
                }
                else
                {
                    callback(null);
                }
            });
        }

        public void GetMovieInfo(string id, Action<Movie> callback)
        {
            lock (_locker)
            {
                if (_currentMovieRequestHandle != null)
                {
                    _currentMovieRequestHandle.Abort();
                    _currentMovieRequestHandle = null;
                }
            }

            var request = new RestRequest("?i={id}&r=json", Method.GET);
            request.AddUrlSegment("id", id);
            lock (_locker)
            {
                _currentMovieRequestHandle = _client.ExecuteAsync<Movie>(request, response =>
                {
                    lock (_locker)
                    {
                        _currentMovieRequestHandle = null;
                    }

                    if (response.Data != null && response.Data.Success)
                    {
                        callback(response.Data);
                    }
                    else
                    {
                        callback(null);
                    }
                });
            }
        }
    }
}
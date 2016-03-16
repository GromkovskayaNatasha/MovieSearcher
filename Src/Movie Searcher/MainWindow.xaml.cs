using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Movie_Searcher.Model;
using RestSharp;

namespace Movie_Searcher
{
    public partial class MainWindow
    {
        private readonly RestClient _client = new RestClient(@"http://www.omdbapi.com");
        private readonly Database _db = new Database();
        private readonly object _locker = new object();
        private RestRequestAsyncHandle _currentMovieRequestHandle;

        public MainWindow()
        {
            InitializeComponent();
            _db.Load();
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                searchButton.IsEnabled = false;
                var request = new RestRequest("?s={title}&r=json", Method.GET);
                request.AddUrlSegment("title", searchTextBox.Text);
                _client.ExecuteAsync<SearchResults>(request, response =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        searchButton.IsEnabled = true;
                        if (response.Data != null && response.Data.Success)
                        {
                            PopulateSearchResults(response.Data.Movies);
                        }
                        else
                        {
                            PopulateSearchResults(null);
                        }
                    });
                });
            }
        }

        private void searchListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var summary = (MovieSummary) searchListBox.Items[searchListBox.SelectedIndex];

            lock (_locker)
            {
                if (_currentMovieRequestHandle != null)
                {
                    _currentMovieRequestHandle.Abort();
                    _currentMovieRequestHandle = null;
                }
            }

            var request = new RestRequest("?i={id}&r=json", Method.GET);
            request.AddUrlSegment("id", summary.Id);
            _client.ExecuteAsync<Movie>(request, response =>
            {
                Dispatcher.Invoke(() =>
                {
                    lock (_locker)
                    {
                        _currentMovieRequestHandle = null;
                    }
                    if (response.Data != null && response.Data.Success)
                    {
                        PopulateMovie(response.Data);
                    }
                });
            });
        }

        private void favButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void favListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void PopulateSearchResults(IEnumerable<MovieSummary> summaries)
        {
            searchListBox.Items.Clear();
            if (summaries != null)
            {
                foreach (var summary in summaries)
                {
                    searchListBox.Items.Add(summary);
                }
            }
        }

        private void PopulateFavourites(IEnumerable<Movie> movies)
        {
            favListBox.Items.Clear();
            if (movies != null)
            {
                foreach (var movie in movies)
                {
                    searchListBox.Items.Add(movie);
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _db.Save();
        }

        private void PopulateMovie(Movie movie)
        {
            titleTextBlock.Text = movie.Title;
            yearTextBlock.Text = movie.Year;
            countryTextBlock.Text = movie.Country;
            plotTextBlock.Text = movie.Plot;
            runtimeTextBlock.Text = movie.Runtime;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(movie.Poster, UriKind.Absolute);
            bitmap.EndInit();
            posterImage.Source = bitmap;
        }
    }
}
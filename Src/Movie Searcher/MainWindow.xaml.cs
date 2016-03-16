using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        private Movie[] _currentFavourites;
        private Movie _currentMovie;
        private RestRequestAsyncHandle _currentMovieRequestHandle;

        public MainWindow()
        {
            InitializeComponent();
            _db.Load();
            _currentFavourites = _db.GetAllFavourites().ToArray();
            PopulateFavourites(_currentFavourites);
            PopulateMovie(null);
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

        private void favButton_Click(object sender, RoutedEventArgs e)
        {
            if (favButton.Content.ToString() == "Remove from Favaourites")
            {
                _db.RemoveFromFavourites(_currentMovie.Id);
                favButton.Content = "Add to Favaourites";
            }
            else
            {
                _db.AddToFavourites(_currentMovie);
                favButton.Content = "Remove from Favaourites";
            }
            _currentFavourites = _db.GetAllFavourites().ToArray();
            PopulateFavourites(_currentFavourites);
        }

        private void searchListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowMovieFromSearchList();
        }

        private void searchListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ShowMovieFromSearchList();
        }

        private void ShowMovieFromSearchList()
        {
            if (searchListBox.SelectedIndex == -1)
            {
                _currentMovie = null;
                PopulateMovie(null);
            }
            else
            {
                var summary = (MovieSummary) searchListBox.Items[searchListBox.SelectedIndex];

                if (_currentMovieRequestHandle != null)
                {
                    _currentMovieRequestHandle.Abort();
                    _currentMovieRequestHandle = null;
                }

                var request = new RestRequest("?i={id}&r=json", Method.GET);
                request.AddUrlSegment("id", summary.Id);
                _client.ExecuteAsync<Movie>(request, response =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        _currentMovieRequestHandle = null;
                        if (response.Data != null && response.Data.Success)
                        {
                            PopulateMovie(response.Data);
                            _currentMovie = response.Data;
                        }
                    });
                });
            }
        }

        private void favListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowMovieFromFavList();
        }

        private void favListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ShowMovieFromFavList();
        }

        private void ShowMovieFromFavList()
        {
            if (favListBox.SelectedIndex == -1)
            {
                _currentMovie = null;
            }
            else
            {
                _currentMovie = _currentFavourites[favListBox.SelectedIndex];
            }
            PopulateMovie(_currentMovie);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _db.Save();
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
                    favListBox.Items.Add(movie);
                }
            }
        }

        private void PopulateMovie(Movie movie)
        {
            if (movie == null)
            {
                titleTextBlock.Text = "";
                yearTextBlock.Text = "";
                countryTextBlock.Text = "";
                plotTextBlock.Text = "";
                runtimeTextBlock.Text = "";
                posterImage.Source = null;
                favButton.Visibility = Visibility.Hidden;
            }
            else
            {
                favButton.Visibility = Visibility.Visible;
                titleTextBlock.Text = movie.Title;
                yearTextBlock.Text = movie.Year;
                countryTextBlock.Text = movie.Country;
                plotTextBlock.Text = movie.Plot;
                runtimeTextBlock.Text = movie.Runtime;

                if (_db.MovieExists(movie.Id))
                {
                    favButton.Content = "Remove from Favaourites";
                }
                else
                {
                    favButton.Content = "Add to Favaourites";
                }

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(movie.Poster, UriKind.Absolute);
                bitmap.EndInit();
                posterImage.Source = bitmap;
            }
        }
    }
}
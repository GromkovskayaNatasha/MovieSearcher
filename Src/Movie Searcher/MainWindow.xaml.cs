using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Movie_Searcher.Model;

namespace Movie_Searcher
{
    public partial class MainWindow
    {
        private readonly Api _api = new Api();
        private readonly Database _db = new Database();
        private readonly object _locker = new object();
        private Movie[] _currentFavorites;
        private Movie _currentMovie;
        private const string AddToFavText = "Add to Favorites";
        private const string RemoveFromFavText = "Remove from Favorites";

        public MainWindow()
        {
            InitializeComponent();
            _db.Load();
            _currentFavorites = _db.GetAllFavorites().ToArray();
            PopulateFavorites(_currentFavorites);
            PopulateMovie(null);
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                searchButton.IsEnabled = false;
                _api.Search(searchTextBox.Text, summaries =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        searchButton.IsEnabled = true;
                        PopulateSearchResults(summaries);
                    });
                });
            }
        }

        private void favButton_Click(object sender, RoutedEventArgs e)
        {
            if (favButton.Content.ToString() == RemoveFromFavText)
            {
                _db.RemoveFromFavorites(_currentMovie.Id);
                favButton.Content = AddToFavText;
            }
            else
            {
                _db.AddToFavorites(_currentMovie);
                favButton.Content = RemoveFromFavText;
            }
            _currentFavorites = _db.GetAllFavorites().ToArray();
            PopulateFavorites(_currentFavorites);
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
                lock (_locker)
                {
                    _currentMovie = null;
                    PopulateMovie(null);
                }
            }
            else
            {
                var summary = (MovieSummary) searchListBox.Items[searchListBox.SelectedIndex];
                _api.GetMovieInfo(summary.Id, movie =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        lock (_locker)
                        {
                            _currentMovie = movie;
                            PopulateMovie(_currentMovie);
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
            lock (_locker)
            {
                if (favListBox.SelectedIndex == -1)
                {
                    _currentMovie = null;
                }
                else
                {
                    _currentMovie = _currentFavorites[favListBox.SelectedIndex];
                }
                PopulateMovie(_currentMovie);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _db.Dispose();
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

        private void PopulateFavorites(IEnumerable<Movie> movies)
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
                yearTextBlock.Text = "Year: " + movie.Year;
                countryTextBlock.Text = "Country: " + movie.Country;
                plotTextBlock.Text = movie.Plot;
                runtimeTextBlock.Text = "Duration: " + movie.Runtime;

                if (_db.MovieExists(movie.Id))
                {
                    favButton.Content = RemoveFromFavText;
                }
                else
                {
                    favButton.Content = AddToFavText;
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
using System;
using System.Windows.Media.Imaging;
using Movie_Searcher.Model;

namespace Movie_Searcher
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void searchButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void searchListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void PopulateSearchResults(MovieSummary[] movies)
        {
            searchListBox.Items.Clear();
            foreach (var movie in movies)
            {
                searchListBox.Items.Add(string.Format("{0}, {1}", movie.Title, movie.Year));
            }
        }

        private void PopulateMovie(Movie movie)
        {
            titleLabel.Content = movie.Title;
            yearLabel.Content = movie.Year;
            countryLabel.Content = movie.Country;
            plotLabel.Content = movie.Plot;
            runtimeLabel.Content = movie.Runtime;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(movie.Poster, UriKind.Absolute);
            bitmap.EndInit();
            posterImage.Source = bitmap;
        }
    }
}
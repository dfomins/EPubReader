using EPubReader.Models;
using EPubReader.ViewModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace EPubReader.Views
{
    public partial class AllBooks : Window
    {
        private AllBooksViewModel booksViewModel;
        private bool isBookMarksFilterEnabled { get; set; }
        private int timerMinutes { get; set; }
        private bool showTimer { get; set; }

        public AllBooks()
        {
            InitializeComponent();
            booksViewModel = new AllBooksViewModel();
            booksListBox.ItemsSource = booksViewModel.books;
            DataContext = booksViewModel;
        }

        private void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            Book selectedBook = (Book)booksListBox.SelectedItem;
            if (booksViewModel.DeleteBookCommand.CanExecute(selectedBook))
                booksViewModel.DeleteBookCommand.Execute(selectedBook);
        }

        private void Label_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Book selectedBook = (Book)booksListBox.SelectedItem;

            var minutes = timerMinutes;
            if (Timer_Checked.IsChecked != true)
                minutes = 0;

            if (ReaderSelectBtn_1.IsChecked == true)
            {
                booksViewModel.OpenBook(selectedBook, 0, minutes, showTimer);
            }
            else if (ReaderSelectBtn_2.IsChecked == true)
            {
                booksViewModel.OpenBook(selectedBook, 1, minutes, showTimer);
            }
        }

        private void TimerMenu_Click(object sender, RoutedEventArgs e)
        {
            Timer timerWindow = new Timer(timerMinutes, showTimer);
            if (timerWindow.ShowDialog() == true)
            {
                timerMinutes = timerWindow.timerMinutes;
                showTimer = timerWindow.showTimer;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchBox.Text;
            Book[] searchedBooks;
            if (isBookMarksFilterEnabled)
            {
                Book[] bookmarksBooks = booksViewModel.books.Where(book => book.IsFavorite).ToArray();
                searchedBooks = booksViewModel.searchBooksByTitle(bookmarksBooks, searchText);
            } else
            {
                searchedBooks = booksViewModel.searchBooksByTitle(booksViewModel.books.ToArray(), searchText);
            }
            booksListBox.ItemsSource = searchedBooks;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Clear();
            SearchButton_Click(sender, e);
        }

        private void AddOrRemoveToFavorite_Click(object sender, RoutedEventArgs e)
        {
            Book selectedBook = (Book)booksListBox.SelectedItem;
            if (booksViewModel.ChangeFavoriteCommand.CanExecute(selectedBook))
                booksViewModel.ChangeFavoriteCommand.Execute(selectedBook);
        }

        private void EnableBookmarksButton_Click(object sender, RoutedEventArgs e)
        {
            Book[] bookmarksBooks = booksViewModel.books.Where(book => book.IsFavorite).ToArray();
            booksListBox.ItemsSource = bookmarksBooks;
            BookmarksButton.Content = "All books";
            isBookMarksFilterEnabled = true;
            booksViewModel.booksCounter = bookmarksBooks.Count();
        }

        private void DisableBookmarksButton_Click(object sender, RoutedEventArgs e)
        {
            booksListBox.ItemsSource = booksViewModel.books;
            BookmarksButton.Content = "Favorites";
            isBookMarksFilterEnabled = false;
            booksViewModel.BooksCount();
        }

    }
}

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
        AllBooksViewModel booksViewModel = new AllBooksViewModel();

        int hours { get; set; }
        int minutes { get; set; }

        public AllBooks()
        {
            InitializeComponent();
            booksListBox.ItemsSource = booksViewModel.books;
            this.DataContext = booksViewModel;
            hours = minutes = 0;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Book selectedBook = (Book)booksListBox.SelectedItem;
            if (booksViewModel.DeleteBookCommand.CanExecute(selectedBook))
                booksViewModel.DeleteBookCommand.Execute(selectedBook);
        }

        private void Label_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Book selectedBook = (Book)booksListBox.SelectedItem;
            int timer = 0;

            if (Timer_Checked.IsChecked == true)
                timer = hours * 3600 + minutes * 60;

            if (ReaderSelectBtn_1.IsChecked == true)
            {
                booksViewModel.OpenBook(selectedBook, 0, timer);
            }
            else if (ReaderSelectBtn_2.IsChecked == true)
            {
                booksViewModel.OpenBook(selectedBook, 1, timer);
            }
        }

        private void AddNewBookCommand(object sender, RoutedEventArgs e)
        {
            booksViewModel.AddNewBook();
        }

        private void TimerMenu_Click(object sender, RoutedEventArgs e)
        {
            Timer timerWindow = new Timer(hours, minutes);
            if (timerWindow.ShowDialog() == true)
            {
                hours = timerWindow.hours;
                minutes = timerWindow.minutes;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchBox.Text;
            Book[] searchedBooks = booksViewModel.books.Where(Book => Book.Title.ToLower().Contains(searchText.ToLower())).ToArray();
            booksListBox.ItemsSource = searchedBooks;
        }

        private void ClickButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Clear();
            SearchButton_Click(sender, e);
        }
    }
}

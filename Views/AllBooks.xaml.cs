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

        int Hours { get; set; }
        int Minutes { get; set; }

        public AllBooks()
        {
            InitializeComponent();
            this.DataContext = booksViewModel;
            Hours = Minutes = 0;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Book selectedBook = (Book)booksListBox.SelectedItem;
            if (booksViewModel.DeleteBookCommand.CanExecute(selectedBook))
                booksViewModel.DeleteBookCommand.Execute(selectedBook);
        }

        private void Label_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Book selectedBook = (Book)booksListBox.SelectedItem;
            int timer = 0;

            if (ReaderSelectBtn_1.IsChecked == true)
            {
                if (Timer_Checked.IsChecked == true)
                    timer = Hours * 3600 + Minutes * 60;

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

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Timer timerWindow = new Timer(Hours, Minutes);
            if (timerWindow.ShowDialog() == true)
            {
                Hours = timerWindow.Hours;
                Minutes = timerWindow.Minutes;
            }
        }
    }
}

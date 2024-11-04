using EPubReader.Models;
using EPubReader.ViewModel;
using System.Windows;

namespace EPubReader.Views
{
    public partial class AllBooks : Window
    {
        AllBooksViewModel booksViewModel = new AllBooksViewModel();

        public AllBooks()
        {
            InitializeComponent();
            this.DataContext = booksViewModel;
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

            if (booksViewModel.OpenBookCommand.CanExecute(selectedBook))
                booksViewModel.OpenBookCommand.Execute(selectedBook);
        }
    }
}

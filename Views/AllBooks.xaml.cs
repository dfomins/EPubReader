using EPubReader.Models;
using EPubReader.ViewModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace EPubReader.Views
{
    /// <summary>
    /// Interaction logic for AllBooks.xaml
    /// </summary>
    public partial class AllBooks : Window
    {
        public AllBooks()
        {
            InitializeComponent();
            AllBooksViewModel booksViewModel = new AllBooksViewModel();
            this.DataContext = booksViewModel;
        }

        private void DeleteBooks_Click(object sender, RoutedEventArgs e)
        {
            var model = (AllBooksViewModel)DataContext;

            var selectedBook = (Book)booksListBox.SelectedItem;

            if (selectedBook != null)
            {
                if (model.DeleteBookCommand.CanExecute(selectedBook))
                {
                    model.DeleteBookCommand.Execute(selectedBook);
                }
            }
        }

        private void Label_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var model = (AllBooksViewModel)DataContext;
             
            var selectedBook = (Book)booksListBox.SelectedItem;

            if (selectedBook != null)
            {
                if (model.OpenBookCommand.CanExecute(selectedBook))
                {
                    model.OpenBookCommand.Execute(selectedBook);
                } else
                {
                    MessageBox.Show("Book path does not exist!", "Error");
                }
            }
        }
    }
}

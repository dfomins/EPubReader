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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

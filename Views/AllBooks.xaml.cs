using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VersOne.Epub;
using EPubReader.Models;

namespace EPubReader.Views
{
    /// <summary>
    /// Interaction logic for AllBooks.xaml
    /// </summary>
    public partial class AllBooks : UserControl
    {
        private string jsonPath = "books.json";
        List<Book>? books = new List<Book>();

        public AllBooks()
        {
            InitializeComponent();
            LoadFromJson();
        }

        private void Add_Book(object sender, RoutedEventArgs e)
        {
            string bookPath = string.Empty;
            OpenFileDialog ofd = new OpenFileDialog();
            EpubBook book;
            ofd.Filter = "Epub files (*.epub) | *.epub";
            if (ofd.ShowDialog() == true)
            {
                bookPath = ofd.FileName;
                book = EpubReader.ReadBook(bookPath);

                LoadFromJson();
                if (books != null) AddBookToJson(books, book, bookPath);
            }
        }

        private void AddBookToJson(List<Book> books, EpubBook book, string bookPath)
        {
            Book newBook = new Book() { Id = books.Count > 0 ? books[books.Count - 1].Id + 1 : 1, Title = book.Title, Path = bookPath, AddingDate = DateTime.Now.ToString("mm/dd/yyyy")};
            books.Add(newBook);
            SaveJson();
            LoadFromJson();

            if (!Directory.Exists("book-covers")) System.IO.Directory.CreateDirectory("book-covers");
            if (book.CoverImage != null) File.WriteAllBytes("book-covers/" + newBook.Id + ".jpg", book.CoverImage);
        }

        private void LoadFromJson()
        {
            if (File.Exists(jsonPath))
            {
                string json = File.ReadAllText(jsonPath);
                books = JsonConvert.DeserializeObject<List<Book>>(json);
                bookList.ItemsSource = books;
            }
        }

        private void SaveJson()
        {
            string json = JsonConvert.SerializeObject(books);
            File.WriteAllText(jsonPath, json);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Book selected = (sender as Button).DataContext as Book;

            if (selected != null)
            {
                books.Remove(books.FirstOrDefault(b => b.Id == selected.Id));

                string imgPath = $"book-covers/{selected.Id}.jpg";
                if (File.Exists(imgPath)) File.Delete(imgPath);
            }
            SaveJson();
            LoadFromJson();
        }

        private void bookList_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var booklist = sender as ItemsControl;

            var model = booklist.DataContext as Book;

            if (model != null)
            {
                MessageBox.Show(model.Title);
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            var viewModel = grid.DataContext as Book;
            if (viewModel != null)
            {
                MessageBox.Show(viewModel.Path);
            }
        }
    }
}

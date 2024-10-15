using EPubReader.Models;
using EPubReader.Views;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

//using System.Windows.Navigation;
//using System.Windows.Shapes;
using VersOne.Epub;
using static System.Reflection.Metadata.BlobBuilder;

namespace EPubReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string jsonPath = "books.json";
        List<Book>? books = new List<Book>();
        public MainWindow()
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
            Book newBook = new Book() { Id = books.Count > 0 ? books[books.Count - 1].Id + 1 : 1, Title = book.Title, Path = bookPath, AddingDate = DateTime.Now.ToString("mm/dd/yyyy") };
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

        //private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    var grid = sender as Grid;
        //    var viewModel = grid.DataContext as Book;
        //    if (viewModel != null)
        //    {
        //        MessageBox.Show(viewModel.Path);
        //    }
        //}

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var grid = sender as Grid;
                var book = grid.DataContext as Book;
                if (book != null)
                {
                    BookWindow bookWindow = new BookWindow();
                    bookWindow.Book = book;
                    bookWindow.Show();
                }
            }
        }
    }
}
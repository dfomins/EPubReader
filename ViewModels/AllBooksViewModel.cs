using EPubReader.Commands;
using EPubReader.Core;
using EPubReader.Models;
using EPubReader.Views;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using VersOne.Epub;

namespace EPubReader.ViewModel
{
    public class AllBooksViewModel : ObservableObject
    {
        public ObservableCollection<Book> books { get; set; }

        private Book _selectedBook;
        public Book selectedBook
        {
            get { return _selectedBook; }
            set { _selectedBook = value; OnPropertyChanged(); }
        }

        private int _booksCounter;
        public int booksCounter
        {
            get { return _booksCounter; }
            set { _booksCounter = value; OnPropertyChanged(); }
        }

        public ICommand DeleteBookCommand { get; set; }

        private void BooksCount()
        {
            booksCounter = books.Count;
        }

        public AllBooksViewModel()
        {
            books = new ObservableCollection<Book>();
            LoadFromJson("books.json");
            BooksCount();

            DeleteBookCommand = new RelayCommand(DeleteBook, CanDeleteBook);
        }

        public void AddNewBook()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            EpubBook book;
            ofd.Filter = "Epub files (*.epub) | *.epub";
            if (ofd.ShowDialog() == true)
            {
                book = EpubReader.ReadBook(ofd.FileName);
                AddBookToJson(books, book, ofd.FileName);
            }
            BooksCount();
        }

        public void OpenBook(Book selectedBook, int selectedReader, int timer)
        {
            if (File.Exists(selectedBook.Path))
            {
                if (selectedBook != null)
                {
                    if (selectedReader == 0)
                    {
                        FlowBookWindow bookWindow = new FlowBookWindow(selectedBook.Path, timer);
                        bookWindow.ShowDialog();
                    }
                    else if (selectedReader == 1)
                    {
                        RichTextBoxBookWindow richTextBoxBookWindow = new RichTextBoxBookWindow(selectedBook.Path, timer);
                        richTextBoxBookWindow.ShowDialog();
                    }
                }
            }
        }

        private bool CanDeleteBook(object obj)
        {
            if (obj != null)
            {
                return true;
            }
            return false;
        }

        private void DeleteBook(object obj)
        {
            Book selectedBook = (Book)obj;
            books.Remove(selectedBook);
            SaveJson("books.json");
            MessageBox.Show($"Book '{selectedBook.Title}' deleted.", "Book deleted", MessageBoxButton.OK, MessageBoxImage.Information);
            GarbageCollector();
            DeleteCover(selectedBook.Id);
            BooksCount();
        }

        private void DeleteCover(int id)
        {
            if (File.Exists($"book-covers/{id}.png"))
            {
                File.Delete($"book-covers/{id}.png");
            }
        }

        private void AddBookToJson(ObservableCollection<Book> books, EpubBook book, string bookPath)
        {
            if (books.Any(b => b.Title == book.Title))
            {
                if (MessageBox.Show("There is already book with this title in the list, do you want to add it anyways?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes == false)
                {
                    return;
                }
            }

            Book newBook = new Book()
            {
                Id = books.Count > 0 ? books[books.Count - 1].Id + 1 : 1,
                Title = book.Title,
                Description = book.Description,
                Path = bookPath,
                AddingDate = DateTime.Now.ToString("dd/MM/yyyy")
            };
            books.Add(newBook);
            
            SaveJson("books.json");

            CreateCoverImage("book-covers", book, newBook);
        }

        private void SaveJson(string jsonPath)
        {
            string json = JsonConvert.SerializeObject(books, Formatting.Indented);
            File.WriteAllText(jsonPath, json);
        }

        private void LoadFromJson(string jsonPath)
        {
            if (File.Exists(jsonPath))
            {
                string json = File.ReadAllText(jsonPath);
                books = JsonConvert.DeserializeObject<ObservableCollection<Book>>(json);
            }
        }

        private void CreateCoverImage(string dirname, EpubBook book, Book newBook)
        {
            if (!Directory.Exists(dirname))
                Directory.CreateDirectory(dirname);

            GarbageCollector();
            if (book.CoverImage != null)
                File.WriteAllBytes($"book-covers/{newBook.Id}.png", book.CoverImage);
        }

        private void GarbageCollector()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}

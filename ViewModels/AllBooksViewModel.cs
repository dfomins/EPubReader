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
        public ObservableCollection<Book> Books { get; set; }

        private Book _selectedBook;
        public Book SelectedBook
        {
            get { return _selectedBook; }
            set { _selectedBook = value; OnPropertyChanged(); }
        }

        private int _booksCounter;
        public int BooksCounter
        {
            get { return _booksCounter; }
            set { _booksCounter = value; OnPropertyChanged(); }
        }

        //public ICommand AddNewBookCommand { get; set; }
        public ICommand DeleteBookCommand { get; set; }

        private void BooksCount()
        {
            BooksCounter = Books.Count;
        }

        public AllBooksViewModel()
        {
            Books = new ObservableCollection<Book>();
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
                AddBookToJson(Books, book, ofd.FileName);
            }
            BooksCount();
        }
        public void OpenBook(Book selectedBook, int readerIndex)
        {
            if (File.Exists(selectedBook.Path))
            {
                if (selectedBook != null)
                {
                    if (readerIndex == 0)
                    {
                        FlowBookWindow bookWindow = new FlowBookWindow(selectedBook.Path);
                        bookWindow.ShowDialog();
                    }
                    else if (readerIndex == 1)
                    {
                        RichTextBoxBookWindow richTextBoxBookWindow = new RichTextBoxBookWindow(selectedBook.Path);
                        richTextBoxBookWindow.ShowDialog();
                    }
                }
            }
        }

        private bool CanOpenBook(object obj)
        {
            if (File.Exists(((Book)obj).Path))
            {
                return true;
            }
            return false;
        }

        private bool CanDeleteBook(object obj)
        {
            return true;
        }

        private void DeleteBook(object obj)
        {
            if (obj != null)
            {
                Book selectedBook = (Book)obj;
                Books.Remove(selectedBook);
                SaveJson("books.json");
                MessageBox.Show($"Book '{selectedBook.Title}' deleted.", "Book deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                GarbageCollector();
                if (File.Exists($"book-covers/{selectedBook.Id}.png"))
                {
                    File.Delete($"book-covers/{selectedBook.Id}.png");
                }
                BooksCount();
            }
        }

        private void AddBookToJson(ObservableCollection<Book> Books, EpubBook book, string bookPath)
        {
            if (Books.Any(Book => Book.Title == book.Title))
            {
                if (MessageBox.Show("There is already book with this title in the list, do you want to add it anyways?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes == false)
                {
                    return;
                }
            }

            Book newBook = new Book()
            {
                Id = Books.Count > 0 ? Books[Books.Count - 1].Id + 1 : 1,
                Title = book.Title,
                Description = book.Description,
                Path = bookPath,
                AddingDate = DateTime.Now.ToString("dd/MM/yyyy")
            };
            Books.Add(newBook);
            
            SaveJson("books.json");

            CreateCoverImage("book-covers", book, newBook);
        }

        private void SaveJson(string jsonPath)
        {
            string json = JsonConvert.SerializeObject(Books, Formatting.Indented);
            File.WriteAllText(jsonPath, json);
        }

        private void LoadFromJson(string jsonPath)
        {
            if (File.Exists(jsonPath))
            {
                string json = File.ReadAllText(jsonPath);
                Books = JsonConvert.DeserializeObject<ObservableCollection<Book>>(json);
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

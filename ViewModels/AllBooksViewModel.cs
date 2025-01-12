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
        private string jsonPath { get; }
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

        public void BooksCount()
        {
            booksCounter = books.Count;
        }

        // Commmands
        public ICommand AddNewBookCommand { get; set; }
        public ICommand ChangeFavoriteCommand { get; set; }
        public ICommand DeleteBookCommand { get; set; }

        public AllBooksViewModel()
        {
            jsonPath = "books.json";
            books = new ObservableCollection<Book>();
            LoadFromJson();
            BooksCount();

            AddNewBookCommand = new RelayCommand(AddNewBook, CanAddNewBook);
            ChangeFavoriteCommand = new RelayCommand(ChangeFavorite, CanChangeFavorite);
            DeleteBookCommand = new RelayCommand(DeleteBook, CanDeleteBook);
        }

        // Add new book command
        private bool CanAddNewBook(object obj)
        {
            return true;
        }

        private void AddNewBook(object obj)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            EpubBook book;
            ofd.Filter = "Epub files (*.epub) | *.epub";
            if (ofd.ShowDialog() == true)
            {
                book = EpubReader.ReadBook(ofd.FileName);
                AddBookToJson(book, ofd.FileName);
                BooksCount();
            }
        }

        // Change bookmark commmand
        private bool CanChangeFavorite(object obj)
        {
            return true;
        }

        private void ChangeFavorite(object obj)
        {
            Book selectedBook = (Book)obj;
            if (selectedBook.IsFavorite)
            {
                selectedBook.IsFavorite = false;
            }
            else
            {
                selectedBook.IsFavorite = true;
            }
            SaveJson();
        }

        // Search
        public Book[] searchBooksByTitle(Book[] booksArray, string searchText)
        {
            return booksArray.Where(Book => Book.Title.ToLower().Contains(searchText.ToLower())).ToArray();
        }

        // Open book command
        public void OpenBook(Book selectedBook, int selectedReader, int timerMinutes, bool showTimer)
        {
            if (File.Exists(selectedBook.Path))
            {
                if (selectedBook != null)
                {
                    if (selectedReader == 0)
                    {
                        FlowBookWindow bookWindow = new FlowBookWindow(selectedBook.Path, timerMinutes, showTimer);
                        bookWindow.ShowDialog();
                    }
                    else if (selectedReader == 1)
                    {
                        RichTextBoxBookWindow richTextBoxBookWindow = new RichTextBoxBookWindow(selectedBook.Path, timerMinutes, showTimer);
                        richTextBoxBookWindow.ShowDialog();
                    }
                }
            } else
            {
                MessageBox.Show("Book doesn't exist!");
            }
        }

        // Delete book command
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
            SaveJson();
            MessageBox.Show($"Book '{selectedBook.Title}' deleted.", "Book deleted", MessageBoxButton.OK, MessageBoxImage.Information);
            GarbageCollector();
            DeleteCover(selectedBook.Id);
            BooksCount();
        }

        // Delete book cover after book deletion
        private void DeleteCover(int id)
        {
            if (File.Exists($"book-covers/{id}.png"))
            {
                File.Delete($"book-covers/{id}.png");
            }
        }


        // JSON methods
        private void AddBookToJson(EpubBook book, string bookPath)
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
            
            SaveJson();

            CreateCoverImage("book-covers", book, newBook);
        }

        private void SaveJson()
        {
            string json = JsonConvert.SerializeObject(books, Formatting.Indented);
            File.WriteAllText(jsonPath, json);
        }

        private void LoadFromJson()
        {
            if (File.Exists(jsonPath))
            {
                string json = File.ReadAllText(jsonPath);
                books = JsonConvert.DeserializeObject<ObservableCollection<Book>>(json);
            }
        }
        //

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

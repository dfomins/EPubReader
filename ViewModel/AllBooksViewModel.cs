using EPubReader.Commands;
using EPubReader.Models;
using EPubReader.Views;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using VersOne.Epub;
using static System.Reflection.Metadata.BlobBuilder;

namespace EPubReader.ViewModel
{
    public class AllBooksViewModel
    {
        public ObservableCollection<Book> Books { get; set; }

        private Book _selectedBook;
        
        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
            }
        }

        public int BookCounter
        {
            get { return Books.Count; }
        }

        public ICommand AddNewBookCommand { get; set; }

        public ICommand OpenBookCommand { get; set; }

        public ICommand DeleteBookCommand { get; set; }

        public AllBooksViewModel()
        {
            Books = new ObservableCollection<Book>();
            LoadFromJson("books.json");

            AddNewBookCommand = new RelayCommand(AddNewBook, CanAddNewBook);
            OpenBookCommand = new RelayCommand(OpenBook, CanOpenBook);
            DeleteBookCommand = new RelayCommand(DeleteBook, CanDeleteBook);
        }

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
                AddBookToJson(Books, book, ofd.FileName);
            }
        }

        private bool CanOpenBook(object obj)
        {
            return true;
        }

        private void OpenBook(object obj)
        {
            Book selectedBook = (Book)obj;
            if (selectedBook != null)
            {
                BookWindow bookWindow = new BookWindow(selectedBook);
                bookWindow.ShowDialog();
            }
        }

        private bool CanDeleteBook(object obj)
        {
            return true;
        }

        private void DeleteBook(object obj)
        {
            Book selectedBook = (Book)obj;
            if(selectedBook != null )
            {
                Books.Remove(selectedBook);
                MessageBox.Show($"Book '{selectedBook.Title}' deleted.");
            }
        }

        private void AddBookToJson(ObservableCollection<Book> Books, EpubBook book, string bookPath)
        {
            Book newBook = new Book()
            {
                Id = Books.Count > 0 ? Books[Books.Count - 1].Id + 1 : 1,
                Title = book.Title,
                Path = bookPath,
                AddingDate = DateTime.Now.ToString("dd/MM/yyyy")
            };

            Books.Add(newBook);
            SaveJson("books.json");

            if (!Directory.Exists("book-covers"))
                System.IO.Directory.CreateDirectory("book-covers");

            if (book.CoverImage != null)
                File.WriteAllBytes($"book-covers/{newBook.Id}.jpg", book.CoverImage);
        }

        private void SaveJson(string jsonPath)
        {
            string json = JsonConvert.SerializeObject(Books);
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

    }
}

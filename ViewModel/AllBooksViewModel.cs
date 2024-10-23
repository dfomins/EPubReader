﻿using EPubReader.Commands;
using EPubReader.Core;
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
    public class AllBooksViewModel : ObservableObject
    {
        public ObservableCollection<Book> Books { get; set; }

        private Book _selectedBook;
        public Book SelectedBook
        {
            get {  return _selectedBook; }
            set
            {
                _selectedBook = value;
                OnPropertyChanged();
            }
        }

        private int _booksCounter;
        public int BooksCounter
        {
            get { return _booksCounter; }
            set
            {
                _booksCounter = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddNewBookCommand { get; set; }
        public ICommand OpenBookCommand { get; set; }
        public ICommand DeleteBookCommand { get; set; }

        private void BooksCount(ObservableCollection<Book> Books)
        {
            BooksCounter = Books.Count;
        }

        public AllBooksViewModel()
        {
            Books = new ObservableCollection<Book>();

            LoadFromJson("books.json");

            BooksCount(Books);

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
            BooksCount(Books);
        }

        private bool CanOpenBook(object obj)
        {
            if (File.Exists(((Book)obj).Path))
            {
                return true;
            }
            return false;
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
            if(MessageBox.Show("Are you sure you want to delete this book?", "Deletion confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                if (selectedBook != null)
                {
                    Books.Remove(selectedBook);
                    SaveJson("books.json");
                    MessageBox.Show($"Book '{selectedBook.Title}' deleted.", "Book deleted");
                    GarbageCollector();
                    if (File.Exists($"book-covers/{selectedBook.Id}.png"))
                    {
                        File.Delete($"book-covers/{selectedBook.Id}.png");
                    }
                }
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

            if (!Directory.Exists("book-covers"))
                Directory.CreateDirectory("book-covers");

            GarbageCollector();
            if (book.CoverImage != null)
                File.WriteAllBytes($"book-covers/{newBook.Id}.png", book.CoverImage);
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

        private void GarbageCollector()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}

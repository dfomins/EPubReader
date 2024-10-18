using EPubReader.Commands;
using EPubReader.Models;
using EPubReader.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VersOne.Epub;

namespace EPubReader.ViewModel
{
    public class BookViewModel
    {
        public List<string> chaptersList {  get; set; }
        public Book Book { get; set; }
        public ICommand OptionsCommand { get; set; }

        public BookViewModel(Book bookToRead)
        {
            Book = bookToRead;
            OptionsCommand = new RelayCommand(OpenOptions, CanOpenOptions);
            chaptersList = GetAllChapters(chaptersList);
        }

        private bool CanOpenOptions(object obj)
        {
            return true;
        }

        private void OpenOptions(object obj)
        {
            Options optionsWindow = new Options();
            optionsWindow.ShowDialog();
        }

        private List<string> GetAllChapters(List<string> chaptersList)
        {
            EpubBook epub = EpubReader.ReadBook(Book.Path);
            chaptersList = new List<string>();

            if (epub.Navigation != null)
            {
                foreach (EpubNavigationItem item in epub.Navigation)
                {
                    chaptersList.Add(item.Title);
                }
            }
            
            return chaptersList;
        }
    }
}

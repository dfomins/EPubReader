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
        public string selectedChapter { get; set; }
        public ICommand OptionsCommand { get; set; }
        public string Chapter { get; set; }

        public BookViewModel(Book bookToRead)
        {
            Book = bookToRead;
            OptionsCommand = new RelayCommand(OpenOptions, CanOpenOptions);
            chaptersList = GetAllChapters(chaptersList);
            EpubBook epub = EpubReader.ReadBook(Book.Path);
            Chapter = epub.ReadingOrder[2].Content;
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
                for (int i = 0; i < epub.Navigation.Count; i++)
                {
                    chaptersList.Add($"{i+1}. {epub.Navigation[i].Title}");

                    for (int j = 0; j < epub.Navigation[i].NestedItems.Count; j++)
                    {
                        chaptersList.Add($"  {i+1}.{j+1}. {epub.Navigation[i].NestedItems[j].Title}");
                    }
                }

                //foreach (EpubNavigationItem item in epub.Navigation)
                //{
                //    chaptersList.Add($"{i}. {item.Title}");
                //    foreach (EpubNavigationItem nestedNavigationItem in item.NestedItems)
                //    {
                //        chaptersList.Add($"  {i}.{nestedNavigationItem.Title}");
                //    }
                //    i++;
                //}
            }
            
            return chaptersList;
        }
    }
}

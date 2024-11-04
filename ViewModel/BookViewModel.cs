using EPubReader.Commands;
using EPubReader.Core;
using EPubReader.Models;
using EPubReader.Views;
using System.Windows.Input;
using VersOne.Epub;

namespace EPubReader.ViewModel
{
    public class BookViewModel : ObservableObject
    {
        public List<EpubNavigationItem> NavigationItems { get; set; }
        public Book Book { get; set; }
        public string selectedChapter { get; set; }
        public int CurrentPage { get; set; }
        public ICommand OptionsCommand { get; set; }
        public ICommand NextPageCommand { get; set; }

        private string _chapter;
        public string Chapter
        {
            get { return _chapter; }
            set
            {
                _chapter = value;
                OnPropertyChanged();
            }
        }

        public BookViewModel(Book bookToRead)
        {
            Book = bookToRead;

            CurrentPage = 0;

            OptionsCommand = new RelayCommand(OpenOptions, CanOpenOptions);
            NextPageCommand = new RelayCommand(OpenNextPage, CanOpenNextPage);

            EpubBook epub = EpubReader.ReadBook(Book.Path);

            NavigationItems = epub.Navigation;

            Chapter = epub.Content.Html.Local[CurrentPage].Content;
        }

        private void UpdateChapter()
        {

        }

        private bool CanOpenNextPage(object obj)
        {
            return true;
        }

        private void OpenNextPage(object obj)
        {
            CurrentPage++;
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
    }
}

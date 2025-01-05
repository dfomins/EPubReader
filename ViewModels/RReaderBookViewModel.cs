using EPubReader.Commands;
using EPubReader.Core;
using EPubReader.ViewModel;
using HtmlAgilityPack;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using VersOne.Epub;

namespace EPubReader.ViewModels
{
    public class RReaderBookViewModel : ObservableObject
    {
        private BaseBookViewModel bookViewModel { get; }
        private List<EpubLocalTextContentFile> readingOrder { get; }
        public List<EpubNavigationItem> bookChapters { get; }
        public string bookTitle { get; }
        public int chaptersCount { get; }
        private Section[] sections { get; }
        private int _currentSectionIndex = 0;
        public int currentSectionIndex
        {
            get => _currentSectionIndex;
            set
            {
                _currentSectionIndex = value;
                OnPropertyChanged(nameof(currentSectionIndex));

                (PrevPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
        //public int currentSectionIndex { get; set; } = 0;
        public FlowDocument flowDocument { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand OptionsCommand { get; set; }

        public RReaderBookViewModel(string BookPath)
        {
            bookViewModel = new BaseBookViewModel(BookPath);
            bookTitle = bookViewModel.bookTitle;
            bookChapters = bookViewModel.bookChapters;
            readingOrder = bookViewModel.book.ReadingOrder;
            chaptersCount = readingOrder.Count;
            sections = new Section[chaptersCount];
            flowDocument = bookViewModel.flowDocument;
            PrevPageCommand = new RelayCommand(PrevPage, CanOpenPrevPage);
            NextPageCommand = new RelayCommand(NextPage, CanOpenNextPage);
            OptionsCommand = new RelayCommand(OpenOptionsWindow, CanOpenOptionsWindow);

            RenderSection();
        }

        private bool CanOpenPrevPage(object obj)
        {
            if (currentSectionIndex > 0)
            {
                return true;
            }

            return false;
        }

        private void PrevPage(object obj)
        {
            ChangeSection(-1);
        }

        private bool CanOpenNextPage(object obj)
        {
            if (currentSectionIndex < chaptersCount - 1)
            {
                return true;
            }

            return false;
        }

        private void NextPage(object obj)
        {
            ChangeSection(+1);
        }

        private bool CanOpenOptionsWindow(object obj)
        {
            return true;
        }

        /// <summary>
        /// Opens options window with font and font size settings
        /// </summary>
        private void OpenOptionsWindow(object obj)
        {
            bookViewModel.OpenOptionsWindow(true);
        }

        private void CreateSection()
        {
            bookViewModel.document.LoadHtml(readingOrder[currentSectionIndex].Content);
            sections[currentSectionIndex] = bookViewModel.CreateSection();
        }

        private void UpdateSection()
        {
            flowDocument.Blocks.Add(sections[currentSectionIndex]);
        }

        public void RenderSection()
        {
            CreateSection();
            UpdateSection();
        }

        public void ChangeSection(int direction)
        {
            ClearRichTextBoxIfNotEmpty();
            currentSectionIndex += direction;
            RenderSection();
        }

        public event Action ClearRichTextBox;

        private void ClearRichTextBoxIfNotEmpty()
        {
            ClearRichTextBox?.Invoke();
        }
    }
}

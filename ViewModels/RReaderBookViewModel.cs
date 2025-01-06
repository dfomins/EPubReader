using EPubReader.Commands;
using EPubReader.Core;
using EPubReader.ViewModel;
using EPubReader.Views;
using HtmlAgilityPack;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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
        public FlowDocument flowDocument { get; }
        private int fontSize { get; set; } = 18;
        private int themeColor { get; set; }
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

        public event Action<int> ChangeColorTheme;
        /// <summary>
        /// Opens options window with font family and font size settings
        /// </summary>
        private void OpenOptionsWindow(object obj)
        {
            Options options = new Options(flowDocument.FontFamily, true, fontSize, themeColor);
            if (options.ShowDialog() == true)
            {
                flowDocument.FontFamily = new FontFamily(options.fontFamily.Source);
                fontSize = options.fontSize;
                themeColor = options.themeColor;
                if (options.themeColor == 0)
                {
                    ChangeColorTheme?.Invoke(0);
                }
                else if (options.themeColor == 1)
                {
                    ChangeColorTheme?.Invoke(1);
                }
                RenderSection();
            }
        }


        public void RenderSection()
        {
            CreateSection();
            UpdateSection();
        }

        private void CreateSection()
        {
            ClearRichTextBoxIfNotEmpty();
            bookViewModel.document.LoadHtml(readingOrder[currentSectionIndex].Content);
            sections[currentSectionIndex] = bookViewModel.CreateSection(fontSize);
        }

        private void UpdateSection()
        {
            flowDocument.Blocks.Add(sections[currentSectionIndex]);
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

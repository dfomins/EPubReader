using EPubReader.Commands;
using EPubReader.Core;
using EPubReader.ViewModel;
using EPubReader.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using VersOne.Epub;

namespace EPubReader.ViewModels
{
    public class FReaderBookViewModel : ObservableObject
    {
        private BaseBookViewModel bookViewModel { get; }
        public List<EpubNavigationItem> bookChapters { get; }
        public string bookTitle { get; }
        public FlowDocument flowDocument { get; }
        private int themeColor { get; set; }

        // Commands
        public ICommand OptionsCommand { get; }
        public ICommand ChaptersCommand { get; }

        // Events
        public event Action<Paragraph> ScrollToAnchor;

        // Styling
        private Brush _panelColor;
        public Brush PanelColor
        {
            get { return _panelColor; }
            set { _panelColor = value; OnPropertyChanged(nameof(PanelColor)); }
        }
        private Brush _textColor;
        public Brush TextColor
        {
            get { return _textColor; }
            set { _textColor = value; OnPropertyChanged(nameof(TextColor)); }
        }

        public FReaderBookViewModel(string bookPath)
        {
            bookViewModel = new BaseBookViewModel(bookPath);
            bookChapters = bookViewModel.bookChapters;
            bookTitle = bookViewModel.bookTitle;
            flowDocument = bookViewModel.flowDocument;

            OptionsCommand = new RelayCommand(OpenOptionsWindow, CanOpenOptionsWindow);
            ChaptersCommand = new RelayCommand(OpenChaptersWindow, CanOpenChaptersWindow);

            TextColor = Brushes.Black;

            LoadBookContent();
        }

        // Chapters window
        private bool CanOpenChaptersWindow(object obj)
        {
            return true;
        }

        private void OpenChaptersWindow(object obj)
        {
            ChaptersWindow chaptersWindow = new ChaptersWindow(bookChapters, bookTitle);
            if (chaptersWindow.ShowDialog() == true)
            {
                Section foundSection = FindSectionByAnchor(chaptersWindow.anchor);
                var paragraph = foundSection.Blocks.OfType<Paragraph>().FirstOrDefault();
                if (paragraph != null)
                    ScrollToAnchor?.Invoke(paragraph);
            }
        }

        private Section FindSectionByAnchor(string anchor)
        {
            foreach (var block in flowDocument.Blocks)
            {
                if (block is Section section && section.Tag?.ToString() == anchor)
                {
                    return section;
                }
            }
            return null;
        }

        private bool CanOpenOptionsWindow(object obj)
        {
            return true;
        }

        //public event Action<int> ColorTheme;

        /// <summary>
        /// Opens options window with font family settings
        /// </summary>
        private void OpenOptionsWindow(object obj)
        {
            Options options = new Options(flowDocument.FontFamily, false, 18, themeColor);
            if (options.ShowDialog() == true)
            {
                flowDocument.FontFamily = new FontFamily(options.fontFamily.Source);
                themeColor = options.themeColor;
            }
            if (options.themeColor == 0)
            {
                flowDocument.Background = Brushes.White;
                PanelColor = Brushes.White;
                TextColor = Brushes.Black;
            }
            else if (options.themeColor == 1)
            {
                flowDocument.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
                PanelColor = new SolidColorBrush(Color.FromRgb(30, 30, 30));
                TextColor = Brushes.White;
            }
        }

        private void LoadBookContent()
        {
            foreach (EpubLocalTextContentFile chapter in bookViewModel.book.ReadingOrder)
            {
                bookViewModel.document.LoadHtml(chapter.Content);
                flowDocument.Blocks.Add(bookViewModel.CreateSection(chapter.Key));
            }
        }
    }
}

﻿using EPubReader.Commands;
using EPubReader.Core;
using EPubReader.Models;
using EPubReader.ViewModel;
using EPubReader.Views;
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
        public List<Chapter> bookChapters { get; }
        public string bookTitle { get; }
        public int chaptersCount { get; }
        private Section[] sections { get; }

        private int _currentSectionIndex = 0;
        public int currentSectionIndex
        {
            get { return _currentSectionIndex; }
            set
            {
                _currentSectionIndex = value;
                OnPropertyChanged(nameof(currentSectionIndex));

                (PrevPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public FlowDocument flowDocument { get; }

        private string _timerText;
        public string timerText
        {
            get { return _timerText; }
            set { _timerText = value; OnPropertyChanged(nameof(timerText)); }
        }

        private int fontSize { get; set; } = 18;
        private int themeColor { get; set; }

        // Commands
        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand OptionsCommand { get; set; }

        // Styling
        private Brush _panelColor;
        public Brush PanelColor {
            get { return _panelColor; }
            set { _panelColor = value; OnPropertyChanged(nameof(PanelColor)); }
        }

        private Brush _textColor;
        public Brush TextColor
        {
            get { return _textColor; }
            set { _textColor = value; OnPropertyChanged(nameof(TextColor)); }
        }

        public RReaderBookViewModel(string BookPath, int seconds, bool showTimer)
        {
            bookViewModel = new BaseBookViewModel(BookPath, seconds, showTimer);
            bookTitle = bookViewModel.bookTitle;
            bookChapters = bookViewModel.bookChapters;
            readingOrder = bookViewModel.book.ReadingOrder;
            chaptersCount = readingOrder.Count;
            sections = new Section[chaptersCount];
            flowDocument = bookViewModel.flowDocument;
            PrevPageCommand = new RelayCommand(PrevPage, CanOpenPrevPage);
            NextPageCommand = new RelayCommand(NextPage, CanOpenNextPage);
            OptionsCommand = new RelayCommand(OpenOptionsWindow, CanOpenOptionsWindow);
            TextColor = Brushes.Black;

            // Timer
            bookViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(bookViewModel.timerText))
                {
                    timerText = bookViewModel.timerText;
                }
            };
            timerText = bookViewModel.timerText;

            SetSectionsTags();

            RenderSection();
        }

        private void SetSectionsTags()
        {
            for (int i = 0; i < sections.Length; i++)
            {
                sections[i] = new Section();
                sections[i].Tag = readingOrder[i].Key;
            }
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
                    PanelColor = Brushes.White;
                    TextColor = Brushes.Black;
                }
                else if (options.themeColor == 1)
                {
                    PanelColor = new SolidColorBrush(Color.FromRgb(30, 30, 30));
                    TextColor = Brushes.White;
                }
                RenderSection();
            }
        }

        private void RenderSection()
        {
            ClearRichTextBoxIfNotEmpty();
            if (currentSectionIndex == 0)
            {
                flowDocument.Blocks.Add(bookViewModel.CreateCover());
            } else
            {
                sections[currentSectionIndex] = bookViewModel.CreateSection(readingOrder[currentSectionIndex].Content, readingOrder[currentSectionIndex].Key, fontSize);
                flowDocument.Blocks.Add(sections[currentSectionIndex]);
            }
        }

        private void ChangeSection(int direction)
        {
            currentSectionIndex += direction;
            RenderSection();
        }

        public event Action ClearRichTextBox;
        private void ClearRichTextBoxIfNotEmpty()
        {
            ClearRichTextBox?.Invoke();
        }

        public void RenderSectionByAnchor(string anchor)
        {
            for (int i = 0; i < sections.Length; i++)
            {
                if (sections[i].Tag.ToString() == anchor)
                {
                    currentSectionIndex = i;
                    RenderSection();
                }
            }
        }
    }
}

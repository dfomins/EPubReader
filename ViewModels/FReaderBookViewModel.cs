using EPubReader.Commands;
using EPubReader.Core;
using EPubReader.ViewModel;
using EPubReader.Views;
using HtmlAgilityPack;
using System.Windows;
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
        public string BookTitle { get; }
        public FlowDocument flowDocument { get; }
        public ICommand OptionsCommand { get; }
        public ICommand ChaptersCommand { get; }

        public FReaderBookViewModel(string bookPath)
        {
            bookViewModel = new BaseBookViewModel(bookPath);
            bookChapters = bookViewModel.bookChapters;
            BookTitle = bookViewModel.bookTitle;
            flowDocument = bookViewModel.flowDocument;

            //OptionsCommand = bookViewModel.OptionsCommand;
            OptionsCommand = new RelayCommand(OpenOptionsWindow, CanOpenOptionsWindow);
            ChaptersCommand = new RelayCommand(OpenChaptersWindow, CanOpenChaptersWindow);

            LoadBookContent();
        }

        private bool CanOpenChaptersWindow(object obj)
        {
            return true;
        }

        private void OpenChaptersWindow(object obj)
        {
            ChaptersWindow chaptersWindow = new ChaptersWindow(bookChapters);
            chaptersWindow.ShowDialog();
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
            bookViewModel.OpenOptionsWindow(false);
        }

        private void LoadBookContent()
        {
            foreach (EpubLocalTextContentFile chapter in bookViewModel.book.ReadingOrder)
            {
                bookViewModel.document.LoadHtml(chapter.Content);
                flowDocument.Blocks.Add(bookViewModel.CreateSection());
            }
        }
    }
}

using EPubReader.Commands;
using EPubReader.Core;
using EPubReader.ViewModel;
using EPubReader.Views;
using HtmlAgilityPack;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using VersOne.Epub;

namespace EPubReader.ViewModels
{
    public class FReaderBookViewModel : ObservableObject
    {
        private BaseBookViewModel bookViewModel { get; }
        public string BookTitle { get; }
        public FlowDocument flowDocument { get; }
        public ICommand OptionsCommand { get; set; }
        public string selectedFontFamily { get; set; }

        public FReaderBookViewModel(string bookPath)
        {
            bookViewModel = new BaseBookViewModel(bookPath);
            BookTitle = bookViewModel.bookTitle;
            flowDocument = bookViewModel.flowDocument;
            OptionsCommand = new RelayCommand(OpenOptionsWindow, CanOpenOptionsWindow);

            LoadBookContent();
        }

        private bool CanOpenOptionsWindow(object obj)
        {
            return true;
        }

        private void OpenOptionsWindow(object obj)
        {
            Options options = new Options(flowDocument.FontFamily.Source);
            if (options.ShowDialog() == true)
            {
                selectedFontFamily = options.selectedFontFamily;
                flowDocument.FontFamily = new FontFamily(selectedFontFamily);
            }
        }

        private void LoadBookContent()
        {
            foreach (EpubLocalTextContentFile chapter in bookViewModel.book.ReadingOrder)
            {
                bookViewModel.document.LoadHtml(chapter.Content);
                var bodyNode = bookViewModel.document.DocumentNode.SelectSingleNode("//body");

                var nodes = bodyNode.Descendants();

                Section section = new Section();

                foreach (var node in nodes)
                {
                    if (node.NodeType == HtmlNodeType.Element && node.ParentNode == bodyNode)
                        bookViewModel.ParseNodes(node, section);
                }

                flowDocument.Blocks.Add(section);
            }
        }
    }
}

using EPubReader.Core;
using EPubReader.Views;
using HtmlAgilityPack;
using System.Windows.Documents;
using System.Windows.Input;
using VersOne.Epub;

namespace EPubReader.ViewModel
{
    public class BookViewModel : ObservableObject
    {
        //public List<EpubNavigationItem> NavigationItems { get; set; }
        private EpubBook book { get; set; }
        public string Title { get; set; }
        public FlowDocument flowDocument { get; set; }
        private ICollection<EpubLocalByteContentFile> Images { get; set; }
        private ICommand OptionsCommand { get; set; }

        public BookViewModel(string bookPath)
        {
            book = EpubReader.ReadBook(bookPath);
            Title = book.Title;
            Images = book.Content.Images.Local;
            var schema = book.Schema.Package.Metadata;

            flowDocument = new FlowDocument();
            flowDocument.ColumnWidth = double.PositiveInfinity;

            var document = new HtmlDocument();

            foreach(EpubLocalTextContentFile chapter in book.ReadingOrder)
            {
                document.LoadHtml(chapter.Content);
                var bodyNode = document.DocumentNode.SelectSingleNode("//body");

                var nodes = bodyNode.Descendants();

                Section section = new Section();

                foreach (var node in nodes)
                {
                    if (node.NodeType == HtmlNodeType.Element && node.ParentNode == bodyNode)
                        Utilities.ParseNodes(node, section, book, Images);
                }

                flowDocument.Blocks.Add(section);
            }
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

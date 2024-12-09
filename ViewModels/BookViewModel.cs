using EPubReader.Core;
using EPubReader.Views;
using HtmlAgilityPack;
using System.Reflection.Metadata;
using System.Windows.Documents;
using System.Windows.Input;
using VersOne.Epub;

namespace EPubReader.ViewModel
{
    public class BookViewModel : ObservableObject
    {
        //public List<EpubNavigationItem> NavigationItems { get; set; }
        private EpubBook Book { get; set; }
        public string Title { get; set; }
        public FlowDocument flowDocument { get; set; }
        private ICollection<EpubLocalByteContentFile> Images { get; set; }
        private ICommand OptionsCommand { get; set; }

        public BookViewModel(string BookPath)
        {
            Book = EpubReader.ReadBook(BookPath);
            Title = Book.Title;
            Images = Book.Content.Images.Local;
            var schema = Book.Schema.Package.Metadata;

            flowDocument = new FlowDocument();
            flowDocument.ColumnWidth = double.PositiveInfinity;

            HtmlDocument document = new HtmlDocument();

            LoadBookContent(document);
        }

        private void LoadBookContent(HtmlDocument document)
        {
            foreach (EpubLocalTextContentFile chapter in Book.ReadingOrder)
            {
                document.LoadHtml(chapter.Content);
                var bodyNode = document.DocumentNode.SelectSingleNode("//body");

                var nodes = bodyNode.Descendants();

                Section section = new Section();

                foreach (var node in nodes)
                {
                    if (node.NodeType == HtmlNodeType.Element && node.ParentNode == bodyNode)
                        Utilities.ParseNodes(node, section, Book, Images);
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

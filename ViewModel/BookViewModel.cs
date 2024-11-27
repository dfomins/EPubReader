using EPubReader.Commands;
using EPubReader.Core;
using EPubReader.Models;
using EPubReader.Views;
using HtmlAgilityPack;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VersOne.Epub;

namespace EPubReader.ViewModel
{
    public class BookViewModel : ObservableObject
    {
        public List<EpubNavigationItem> NavigationItems { get; set; }
        public FlowDocument flowDocument { get; set; }
        public ICollection<EpubLocalByteContentFile> images { get; set; }
        public ICommand OptionsCommand { get; set; }
        public ICommand NextPageCommand { get; set; }

        public BookViewModel(string bookPath)
        {
            EpubBook book = EpubReader.ReadBook(bookPath);
            images = book.Content.Images.Local;

            flowDocument = new FlowDocument();
            flowDocument.ColumnWidth = double.PositiveInfinity;

            var document = new HtmlDocument();

            foreach(EpubLocalTextContentFile chapter in book.ReadingOrder)
            {
                document.LoadHtml(chapter.Content);
                var bodyNode = document.DocumentNode.SelectSingleNode("//body");

                //if (bodyNode == null) continue;

                var nodes = bodyNode.Descendants();

                Section section = new Section();

                foreach (var node in nodes)
                {
                    if (node.NodeType == HtmlNodeType.Element && node.ParentNode == bodyNode)
                        ParseNodes(node, section);
                }

                flowDocument.Blocks.Add(section);
            }
        }

        public void ParseNodes(HtmlNode node, Section section)
        {
            switch (node.Name)
            {
                case "section":
                case "div":
                    var childNodes = node.ChildNodes;
                    foreach (var childNode in childNodes)
                    {
                        if ((childNode.InnerHtml).Trim() != "" || childNode.Name == "img")
                            ParseNodes(childNode, section);
                    }
                    return;

                case "a":
                case "strong":
                case "#text":
                case "p":
                    string textWithoutSpaces = node.InnerText.Replace("\n", " ").Replace("\r", " ");
                    Paragraph text = new Paragraph(new Run(textWithoutSpaces));
                    if (node.Name == "p")
                    {
                        text.TextIndent = 20;
                    }
                    else if (node.Name == "strong")
                    {
                        text.FontWeight = FontWeights.Bold;
                    }

                    section.Blocks.Add(text);
                    break;

                case "h1":
                    Paragraph headerParagraph = new Paragraph(new Run(node.InnerText))
                    {
                        FontSize = 24,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center,
                        BreakPageBefore = true
                    };

                    section.Blocks.Add(headerParagraph);
                    break;

                case "h2":
                    headerParagraph = new Paragraph(new Run(node.InnerText))
                    {
                        FontSize = 20,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center,
                        BreakPageBefore = true
                    };

                    section.Blocks.Add(headerParagraph);
                    break;

                case "h3":
                    section.Blocks.Add(new Paragraph(new Run(node.InnerText))
                    {
                        FontSize = 18,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center
                    });
                    break;

                case "img":
                    string fileName = node.Attributes["src"].Value;
                    if (fileName != null)
                    {
                        var imageItem = images.FirstOrDefault(x => x.Key == fileName);
                        BitmapImage bitmapImage = CreateBitmapFromBytes(imageItem.Content);

                        Image image = new Image
                        {
                            Source = bitmapImage,
                            Width = 200,
                        };

                        Paragraph paragraph = new Paragraph()
                        {
                            TextAlignment = TextAlignment.Center,
                        };

                        paragraph.Inlines.Add(new InlineUIContainer(image));

                        section.Blocks.Add(paragraph);
                    }
                    break;
            }
        }

        private BitmapImage CreateBitmapFromBytes(byte[] imageBytes)
        {
            BitmapImage bitmap = new BitmapImage();
            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
            }

            return bitmap;
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

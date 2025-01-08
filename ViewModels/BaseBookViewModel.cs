using EPubReader.Core;
using HtmlAgilityPack;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using VersOne.Epub;

namespace EPubReader.ViewModel
{
    public class BaseBookViewModel : ObservableObject
    {
        public EpubBook book { get; }
        public List<EpubNavigationItem> bookChapters { get; }
        public string bookTitle { get; }
        public HtmlDocument document { get; } = new HtmlDocument();
        private ICollection<EpubLocalByteContentFile> images { get; }
        public FlowDocument flowDocument { get; } = new FlowDocument();

        public BaseBookViewModel(string bookPath)
        {
            book = EpubReader.ReadBook(bookPath);
            bookChapters = book.Navigation.ToList();
            bookTitle = book.Title;
            images = book.Content.Images.Local;
            flowDocument.ColumnWidth = double.PositiveInfinity;
        }

        /// <summary>
        /// Handles all nodes, parse them and then return as section
        /// </summary>
        public Section CreateSection(string chapterKey, int fontSize = 18)
        {
            Section section = new Section();
            if (chapterKey != null)
            {
                section.Tag = chapterKey;
            }
            try
            {
                HtmlNode bodyNode = document.DocumentNode.SelectSingleNode("//body");
                IEnumerable<HtmlNode> nodes = bodyNode.Descendants();

                foreach (var node in nodes)
                {
                    if (node.NodeType == HtmlNodeType.Element && !string.IsNullOrWhiteSpace(node.InnerHtml) && node.ParentNode == bodyNode)
                        ParseNodes(node, section, fontSize);
                }
            } catch (Exception)
            {
                MessageBox.Show("The file format is probably too old, it is not possible to run the reader", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
            return section;
        }

        /// <summary>
        /// Handles each HTML node in a specific section
        /// </summary>
        public void ParseNodes(HtmlNode node, Section section, int fontSize)
        {
            switch (node.Name)
            {
                case "a":
                case "strong":
                case "#text":
                case "p":
                    string textWithoutEnters = node.InnerText.Replace("\n", null).Replace("\r", null);
                    Paragraph text = new Paragraph(new Run(textWithoutEnters)
                    {
                        FontSize = fontSize
                    });

                    if (node.Name == "p")
                    {
                        text.TextIndent = 20;
                    }

                    if (node.Name == "strong")
                    {
                        text.FontWeight = FontWeights.Bold;
                    }

                    section.Blocks.Add(text);
                    break;

                case "h1":
                    Paragraph headerParagraph = new Paragraph(new Run(node.InnerText))
                    {
                        FontSize = fontSize + 4,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center,
                        BreakPageBefore = true
                    };

                    section.Blocks.Add(headerParagraph);
                    break;

                case "h2":
                    headerParagraph = new Paragraph(new Run(node.InnerText))
                    {
                        FontSize = fontSize + 2,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center,
                        BreakPageBefore = true
                    };

                    section.Blocks.Add(headerParagraph);
                    break;

                case "h3":
                    section.Blocks.Add(new Paragraph(new Run(node.InnerText))
                    {
                        FontSize = fontSize,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center
                    });
                    break;

                case "img":
                    string fileName = node.Attributes["src"].Value;
                    if (fileName != null && images != null)
                    {
                        var imageItem = images.FirstOrDefault(x => x.Key == fileName);
                        BitmapImage bitmapImage = CreateBitmapFromBytes(imageItem.Content);

                        Image image = new Image
                        {
                            Source = bitmapImage,
                        };

                        if (fileName == book.Content?.Cover?.Key)
                        {
                            image.MaxHeight = 1000;
                            Thickness thicc = new Thickness(0, 0, 0, 40);
                            image.Margin = thicc;
                        }
                        else
                        {
                            image.MaxHeight = 250;
                        }

                        Paragraph paragraph = new Paragraph()
                        {
                            TextAlignment = TextAlignment.Center,
                        };

                        paragraph.Inlines.Add(new InlineUIContainer(image));

                        section.Blocks.Add(paragraph);
                    }
                    break;

                default:
                    var childNodes = node.ChildNodes;
                    foreach (var childNode in childNodes)
                    {
                        if ((childNode.InnerHtml).Trim() != "" || childNode.Name == "img")
                            ParseNodes(childNode, section, fontSize);
                    }
                    return;
            }
        }

        /// <summary>
        /// Creates an image from byte array
        /// </summary>
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
    }
}

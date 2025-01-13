using EPubReader.Core;
using HtmlAgilityPack;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VersOne.Epub;
using System.Web;
using EPubReader.Models;

namespace EPubReader.ViewModel
{
    public class BaseBookViewModel : ObservableObject
    {
        public EpubBook book { get; }
        public List<Chapter> bookChapters { get; }
        public string bookTitle { get; }
        private ICollection<EpubLocalByteContentFile> images { get; }
        public FlowDocument flowDocument { get; } = new FlowDocument();

        // Timer
        private DispatcherTimer dispatcherTimer;
        private int counter { get; set; }
        private int timerMinutes { get; set; }

        private string _timerText;
        public string timerText
        {
            get { return _timerText; }
            set { _timerText = value; OnPropertyChanged(nameof(timerText)); }
        }
        private bool showTimer { get; set; }

        public BaseBookViewModel(string bookPath, int timerMinutes, bool showTimer)
        {
            book = EpubReader.ReadBook(bookPath);
            List<Chapter> chapters = new List<Chapter>();
            if (book.Navigation != null)
            {
                foreach (EpubNavigationItem chapter in book.Navigation)
                {
                    Chapter cpt = new Chapter(chapter.Title, chapter.Link.ContentFileUrl);
                    chapters.Add(cpt);
                    if (chapter.NestedItems.Count > 0)
                    {
                        foreach (EpubNavigationItem subChapter in chapter.NestedItems)
                        {
                            Chapter cpt2 = new Chapter(subChapter.Title, subChapter.Link.ContentFileUrl);
                            cpt.SubChapter.Add(cpt2);
                        }
                    }
                }
                bookChapters = chapters;
            }
            bookTitle = book.Title;
            images = book.Content.Images.Local;
            flowDocument.ColumnWidth = double.PositiveInfinity;
            this.timerMinutes = timerMinutes;
            this.showTimer = showTimer;

            Timer();
        }

        private void Timer()
        {
            if (timerMinutes > 0)
            {
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Interval = TimeSpan.FromMinutes(1);
                dispatcherTimer.Tick += timer_Tick;
                dispatcherTimer.Start();
                UpdateTimerText();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            counter++;
            if (counter >= timerMinutes)
            {
                dispatcherTimer.Stop();
                UpdateTimerText();
                MessageBoxResult result = MessageBox.Show("Time ended! Want to set new timer?", "Timer ended", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    EPubReader.Views.Timer timerWindow = new EPubReader.Views.Timer(timerMinutes, showTimer);
                    if (timerWindow.ShowDialog() == true)
                    {
                        counter = 0;
                        timerMinutes = timerWindow.timerMinutes;
                        showTimer = timerWindow.showTimer;
                        Timer();
                    }
                } else
                {
                    showTimer = false;
                    UpdateTimerText();
                    return;
                }
            }
            UpdateTimerText();
        }

        private void UpdateTimerText()
        {
            if (showTimer)
            {
                TimeSpan timer = TimeSpan.FromMinutes(timerMinutes);
                TimeSpan timeSpan = TimeSpan.FromMinutes(counter);
                timerText = "Timer: " + timeSpan.ToString(@"hh\:mm") + "/" + timer.ToString(@"hh\:mm");
            } else
            {
                timerText = "";
            }
        }

        public Section CreateCover()
        {
            Section section = new Section();

            BitmapImage bitmapImage = CreateBitmapFromBytes(book.CoverImage);

            Image image = new Image()
            {
                Source = bitmapImage,
                MaxHeight = 1000,
                Margin = new Thickness(0, 0, 0, 40)
            };

            Paragraph paragraph = new Paragraph()
            {
                TextAlignment = TextAlignment.Center,
            };

            paragraph.Inlines.Add(new InlineUIContainer(image));
            section.Blocks.Add(paragraph);

            return section;
        }

        /// <summary>
        /// Handles all nodes, parses and then returns as section
        /// </summary>
        public Section CreateSection(string content, string chapterKey, int fontSize = 18)
        {
            Section section = new Section();
            if (chapterKey != null)
            {
                section.Tag = chapterKey;
            }
            try
            {
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(content);
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
                    string decodedText = HttpUtility.HtmlDecode(textWithoutEnters);
                    Paragraph text = new Paragraph(new Run(decodedText)
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
                        EpubLocalByteContentFile? imageItem = images.FirstOrDefault(x => x.Key == fileName);
                        if (imageItem != null)
                        {
                            BitmapImage bitmapImage = CreateBitmapFromBytes(imageItem.Content);

                            Image image = new Image
                            {
                                Source = bitmapImage,
                            };

                            if (fileName == book.Content?.Cover?.Key)
                            {
                                image.MaxHeight = 1000;
                                image.Margin = new Thickness(0, 0, 0, 40);
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

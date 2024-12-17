using EPubReader.Commands;
using EPubReader.Core;
using EPubReader.Views;
using HtmlAgilityPack;
using System.IO;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VersOne.Epub;

namespace EPubReader.ViewModel
{
    public class BaseBookViewModel : ObservableObject
    {
        public EpubBook book { get; }
        public string bookTitle { get; }
        public HtmlDocument document { get; } = new HtmlDocument();
        private ICollection<EpubLocalByteContentFile> images { get; }
        public FlowDocument flowDocument { get; } = new FlowDocument();
        public ICommand OptionsCommand { get; set; }
        public string selectedFontFamily { get; set; }
        string[] fontsNameList { get; }


        public BaseBookViewModel(string bookPath)
        {
            book = EpubReader.ReadBook(bookPath);
            bookTitle = book.Title;
            images = book.Content.Images.Local;
            flowDocument.ColumnWidth = double.PositiveInfinity;

            OptionsCommand = new RelayCommand(OpenOptionsWindow, CanOpenOptionsWindow);
            // Loads all system fonts for options window
            var allSystemFonts = Fonts.SystemFontFamilies.ToArray();
            fontsNameList = new string[allSystemFonts.Length];
            for (int i = 0; i < allSystemFonts.Length; i++)
            {
                fontsNameList[i] = allSystemFonts[i].Source;
            }
            //
        }

        private bool CanOpenOptionsWindow(object obj)
        {
            return true;
        }

        private void OpenOptionsWindow(object obj)
        {
            Options options = new Options(flowDocument.FontFamily.Source, fontsNameList);
            if (options.ShowDialog() == true)
            {
                selectedFontFamily = options.selectedFontFamily.Source;
                flowDocument.FontFamily = new FontFamily(selectedFontFamily);
            }
        }

        /// <summary>
        /// Handles each HTML node in a specific section
        /// </summary>
        public void ParseNodes(HtmlNode node, Section section, int fontSize = 18)
        {
            switch (node.Name)
            {
                case "section":
                case "div":
                    var childNodes = node.ChildNodes;
                    foreach (var childNode in childNodes)
                    {
                        if ((childNode.InnerHtml).Trim() != "" || childNode.Name == "img")
                            ParseNodes(childNode, section, fontSize);
                    }
                    return;

                case "a":
                case "strong":
                case "#text":
                case "p":
                    string textWithoutEnters = node.InnerText.Replace("\n", " ").Replace("\r", " ");
                    Paragraph text = new Paragraph(new Run(textWithoutEnters)
                    {
                        FontSize = fontSize
                    });

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
                            image.MaxHeight = bitmapImage.Height * 2;
                            Thickness thicc = new Thickness(0, 0, 0, 40);
                            image.Margin = thicc;
                        }
                        else
                        {
                            image.MaxWidth = bitmapImage.Width;
                        }

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

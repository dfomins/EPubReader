using EPubReader.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using VersOne.Epub;
using System.Windows.Media;
using System.Reflection.Metadata;

namespace EPubReader.ViewModel
{
    public class Utilities
    {
        static public HtmlNode GetNodesFromContent(string Content)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(Content);
            return document.DocumentNode.SelectSingleNode("//body");
        }

        static public void ParseNodes(HtmlNode node, Section section, EpubBook book, ICollection<EpubLocalByteContentFile> Images)
        {
            switch (node.Name)
            {
                case "section":
                case "div":
                    var childNodes = node.ChildNodes;
                    foreach (var childNode in childNodes)
                    {
                        if ((childNode.InnerHtml).Trim() != "" || childNode.Name == "img")
                            ParseNodes(childNode, section, book, Images);
                    }
                    return;

                case "a":
                case "strong":
                case "#text":
                case "p":
                    string textWithoutEnters = node.InnerText.Replace("\n", " ").Replace("\r", " ");
                    Paragraph text = new Paragraph(new Run(textWithoutEnters));
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
                    if (fileName != null && Images != null)
                    {
                        var imageItem = Images.FirstOrDefault(x => x.Key == fileName);
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

        static private BitmapImage CreateBitmapFromBytes(byte[] imageBytes)
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

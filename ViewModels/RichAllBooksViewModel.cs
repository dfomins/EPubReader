using EPubReader.Commands;
using EPubReader.Models;
using EPubReader.ViewModel;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Input;
using VersOne.Epub;

namespace EPubReader.ViewModels
{
    public class RichAllBooksViewModel
    {
        public string BookTitle { get; set; }
        public int ChaptersCount { get; }
        public FlowDocument flowDocument { get; set; } = new FlowDocument();
        public int currentSectionIndex { get; set; } = 0;


        EpubBook Book { get; set; }
        ICollection<EpubLocalByteContentFile> Images { get; set; }
        HtmlDocument document { get; set; } = new HtmlDocument();
        Section[] sections { get; set; }

        public RichAllBooksViewModel(string BookPath)
        {
            Book = EpubReader.ReadBook(BookPath);
            ChaptersCount = Book.ReadingOrder.Count;
            BookTitle = Book.Title;
            Images = Book.Content.Images.Local;
            sections = new Section[Book.ReadingOrder.Count];

            RenderSection();
        }

        private void CreateSection()
        {
            var bodyNode = Utilities.GetNodesFromContent(Book.ReadingOrder[currentSectionIndex].Content);
            var nodes = bodyNode.Descendants();

            sections[currentSectionIndex] = new Section();

            foreach (var node in nodes)
            {
                if (node.NodeType == HtmlNodeType.Element && node.ParentNode == bodyNode)
                    Utilities.ParseNodes(node, sections[currentSectionIndex], Book, Images);
            }
        }

        private void UpdateSection()
        {
            flowDocument.Blocks.Add(sections[currentSectionIndex]);
        }

        public void RenderSection()
        {
            CreateSection();
            UpdateSection();
        }
    }
}

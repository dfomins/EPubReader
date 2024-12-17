using EPubReader.Commands;
using EPubReader.Models;
using EPubReader.ViewModel;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VersOne.Epub;

namespace EPubReader.ViewModels
{
    public class RReaderBookViewModel
    {
        private BaseBookViewModel bookViewModel { get; }
        private List<EpubLocalTextContentFile> readingOrder { get; }
        public string bookTitle { get; }
        public int chaptersCount { get; }
        private Section[] sections { get; }
        public int currentSectionIndex { get; set; } = 0;
        public FlowDocument flowDocument { get; }
        public ICommand OptionsCommand { get; set; }

        public RReaderBookViewModel(string BookPath)
        {
            bookViewModel = new BaseBookViewModel(BookPath);
            bookTitle = bookViewModel.bookTitle;
            readingOrder = bookViewModel.book.ReadingOrder;
            chaptersCount = readingOrder.Count;
            sections = new Section[chaptersCount];
            flowDocument = bookViewModel.flowDocument;
            OptionsCommand = bookViewModel.OptionsCommand;
            RenderSection();
        }

        private void CreateSection()
        {
            bookViewModel.document.LoadHtml(readingOrder[currentSectionIndex].Content);
            var bodyNode = bookViewModel.document.DocumentNode.SelectSingleNode("//body");

            var nodes = bodyNode.Descendants();

            sections[currentSectionIndex] = new Section();

            foreach (var node in nodes)
            {
                if (node.NodeType == HtmlNodeType.Element && node.ParentNode == bodyNode)
                    bookViewModel.ParseNodes(node, sections[currentSectionIndex]);
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

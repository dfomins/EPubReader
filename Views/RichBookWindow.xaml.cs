﻿using EPubReader.Models;
using EPubReader.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VersOne.Epub;

namespace EPubReader.Views
{
    /// <summary>
    /// Interaction logic for RichTextBoxBookWindow.xaml
    /// </summary>
    public partial class RichTextBoxBookWindow : Window
    {
        RReaderBookViewModel richBookViewModel { get; set; }
        private string timerText { get; set; }

        public RichTextBoxBookWindow(string BookPath, int seconds, bool showTimer)
        {
            InitializeComponent();
            richBookViewModel = new RReaderBookViewModel(BookPath, seconds, showTimer);
            DataContext = richBookViewModel;
            richBookWindow.Title = richBookViewModel.bookTitle;
            richTextBox.Document = richBookViewModel.flowDocument;
            bookChaptersTreeView.ItemsSource = richBookViewModel.bookChapters;

            // Invokes
            richBookViewModel.ClearRichTextBox += ClearRichTextBoxIfNotEmpty;
        }

        private void ClearRichTextBoxIfNotEmpty()
        {
            if (richTextBox != null)
            {
                richTextBox.Document.Blocks.Clear();
                richTextBoxScrollViewer.ScrollToVerticalOffset(0);
            }
        }

        private void ChapterLabel_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is ContentControl control)
            {
                var chapter = control.DataContext as Chapter;
                var anchor = chapter.Key;
                richBookViewModel.RenderSectionByAnchor(anchor);
            }
        }
    }
}

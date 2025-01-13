using EPubReader.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EPubReader.Views
{
    /// <summary>
    /// Interaction logic for ChaptersWindow.xaml
    /// </summary>
    public partial class ChaptersWindow : Window
    {
        public string anchor { get; set; } = string.Empty;

        public ChaptersWindow(List<Chapter> bookChapters, string bookTitle)
        {
            InitializeComponent();
            chaptersWindow.Title = bookTitle + " - Chapters";
            bookChaptersListBox.ItemsSource = bookChapters;
        }

        private void ChapterLabel_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is ContentControl control)
            {
                var chapter = control.DataContext as Chapter;
                if (chapter != null && chapter.Key != null)
                {
                    anchor = chapter.Key;
                    DialogResult = true;
                    Close();
                }
            }
        }
    }
}

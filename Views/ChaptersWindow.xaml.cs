using EPubReader.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VersOne.Epub;

namespace EPubReader.Views
{
    /// <summary>
    /// Interaction logic for ChaptersWindow.xaml
    /// </summary>
    public partial class ChaptersWindow : Window
    {
        public string anchor { get; set; } = string.Empty;

        public ChaptersWindow(List<EpubNavigationItem> bookChapters, string bookTitle)
        {
            InitializeComponent();
            chaptersWindow.Title = bookTitle + " - Chapters";
            bookChaptersListBox.ItemsSource = bookChapters;
        }

        private void ChapterLabel_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is ContentControl control)
            {
                var chapter = control.DataContext as EpubNavigationItem;
                if (chapter != null && chapter.Link != null)
                {
                    anchor = chapter.Link.ContentFileUrl;
                    DialogResult = true;
                    Close();
                }
            }
        }
    }
}

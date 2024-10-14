using HtmlAgilityPack;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using VersOne.Epub;

namespace EPubReader.Views
{
    /// <summary>
    /// Interaction logic for ReadBook.xaml
    /// </summary>
    public partial class ReadBook : UserControl
    {
        public ReadBook()
        {
            InitializeComponent();
            GetAllChapters();
        }

        private void GetAllChapters()
        {
            EpubBook epub = EpubReader.ReadBook("C:/Users/danie/Downloads/pg74571-images.epub");

            List<string> chapters = new List<string>();

            if (epub.Navigation != null)
            {
                foreach (EpubNavigationItem item in epub.Navigation)
                {
                    chapters.Add(item.Title);
                }
            }

            chaptersList.ItemsSource = chapters;
        }
    }
}

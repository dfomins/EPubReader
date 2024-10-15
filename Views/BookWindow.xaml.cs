using EPubReader.Models;
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
    /// Interaction logic for BookWindow.xaml
    /// </summary>
    public partial class BookWindow : Window
    {
        private Book _book;

        public Book Book
        {
            get { return _book; }
            set
            {
                _book = value;
                if(_book != null)
                {
                    GetAllChapters();
                }
            }
        }

        public BookWindow()
        {
            InitializeComponent();
        }

        private void GetAllChapters()
        {
            EpubBook epub = EpubReader.ReadBook(Book.Path);

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

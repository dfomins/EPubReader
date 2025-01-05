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
        public ChaptersWindow(List<EpubNavigationItem> bookChapters)
        {
            InitializeComponent();
            bookChaptersListBox.ItemsSource = bookChapters;
        }
    }
}

using EPubReader.Models;
using EPubReader.ViewModel;
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
        public BookWindow(string bookPath)
        {
            InitializeComponent();
            BookViewModel bookViewModel = new BookViewModel(bookPath);
            flowDocumentReader.Document = bookViewModel.flowDocument;
        }
    }
}

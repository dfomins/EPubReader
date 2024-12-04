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

namespace EPubReader.Views
{
    /// <summary>
    /// Interaction logic for FlowBookWindow.xaml
    /// </summary>
    public partial class FlowBookWindow : Window
    {
        public FlowBookWindow(string BookPath)
        {
            InitializeComponent();
            BookViewModel bookViewModel = new BookViewModel(BookPath);
            flowBookWindow.Title = bookViewModel.Title;
            flowDocumentReader.Document = bookViewModel.flowDocument;
        }
    }
}

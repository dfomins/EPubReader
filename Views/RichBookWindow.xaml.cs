using EPubReader.Commands;
using EPubReader.ViewModel;
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

namespace EPubReader.Views
{
    /// <summary>
    /// Interaction logic for RichTextBoxBookWindow.xaml
    /// </summary>
    public partial class RichTextBoxBookWindow : Window
    {
        RichAllBooksViewModel bookViewModel { get; set; }

        public RichTextBoxBookWindow(string BookPath)
        {
            InitializeComponent();
            bookViewModel = new RichAllBooksViewModel(BookPath);
            richBookWindow.Title = bookViewModel.BookTitle;
            if (richTextBox != null)
                richTextBox.Document.Blocks.Clear();
            richTextBox.Document = bookViewModel.flowDocument;
        }

        private void Button_Click_Prev(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_Next(object sender, RoutedEventArgs e)
        {
            if (bookViewModel.currentSectionIndex < bookViewModel.ChaptersCount - 1)
            {
                if (richTextBox != null)
                    richTextBox.Document.Blocks.Clear();
                bookViewModel.currentSectionIndex++;
                bookViewModel.RenderSection();
            }
        }
    }
}

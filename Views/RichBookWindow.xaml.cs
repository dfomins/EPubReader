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
using System.Windows.Threading;

namespace EPubReader.Views
{
    /// <summary>
    /// Interaction logic for RichTextBoxBookWindow.xaml
    /// </summary>
    public partial class RichTextBoxBookWindow : Window
    {
        RReaderBookViewModel richBookViewModel { get; set; }
        DispatcherTimer dispatcherTimer;
        int counter;
        int seconds;
        int p = 20, h1 = 24, h2 = 20, h3 = 18;

        public RichTextBoxBookWindow(string BookPath, int seconds)
        {
            InitializeComponent();
            this.seconds = seconds;
            richBookViewModel = new RReaderBookViewModel(BookPath);
            richBookWindow.Title = richBookViewModel.bookTitle;
            ClearRichTextBoxIfNotEmpty();
            richTextBox.Document = richBookViewModel.flowDocument;

            if (seconds > 0)
            {
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
                dispatcherTimer.Tick += timer_Tick;
                dispatcherTimer.Start();
            }
            else
            {
                TimerText.Text = "Timer: disabled";
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            counter++;
            if (counter >= seconds)
            {
                MessageBox.Show("Time ended!");
                dispatcherTimer.Stop();
            }
            TimeSpan timer = TimeSpan.FromSeconds(seconds);
            TimeSpan timeSpan = TimeSpan.FromSeconds(counter);
            TimerText.Text = "Timer: " + timeSpan.ToString(@"hh\:mm") + "/" + timer.ToString(@"hh\:mm");
        }

        private void Button_Click_Prev(object sender, RoutedEventArgs e)
        {
            if (richBookViewModel.currentSectionIndex > 0)
            {
                ClearRichTextBoxIfNotEmpty();
                richBookViewModel.currentSectionIndex--;
                richBookViewModel.RenderSection();
            }
        }

        private void Button_Click_Next(object sender, RoutedEventArgs e)
        {
            if (richBookViewModel.currentSectionIndex < richBookViewModel.chaptersCount - 1)
            {
                ClearRichTextBoxIfNotEmpty();
                richBookViewModel.currentSectionIndex++;
                richBookViewModel.RenderSection();
            }
        }

        private void ZoomOut(object sender, RoutedEventArgs e)
        {
            if (h3 > 10)
            {
                p -= 2;
                h1 -= 2;
                h2 -= 2;
                h3 -= 2;
            }
            richBookViewModel.RenderSection();
        }

        private void ZoomIn(object sender, RoutedEventArgs e)
        {
            if (h3 < 40)
            {
                p += 2;
                h1 += 2;
                h2 += 2;
                h3 += 2;
            }
            richBookViewModel.RenderSection();
        }

        private void ClearRichTextBoxIfNotEmpty()
        {
            if (richTextBox != null)
                richTextBox.Document.Blocks.Clear();
        }
    }
}

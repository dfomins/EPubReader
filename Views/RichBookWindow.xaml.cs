using EPubReader.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using VersOne.Epub;

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

        public RichTextBoxBookWindow(string BookPath, int seconds)
        {
            InitializeComponent();
            this.seconds = seconds;
            richBookViewModel = new RReaderBookViewModel(BookPath);
            DataContext = richBookViewModel;
            richBookWindow.Title = richBookViewModel.bookTitle;
            ClearRichTextBoxIfNotEmpty();
            richTextBox.Document = richBookViewModel.flowDocument;
            bookChaptersListBox.ItemsSource = richBookViewModel.bookChapters;
            richBookViewModel.ClearRichTextBox += ClearRichTextBoxIfNotEmpty;

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
                MessageBox.Show("Time ended!", "Timer");
                dispatcherTimer.Stop();
            }
            TimeSpan timer = TimeSpan.FromSeconds(seconds);
            TimeSpan timeSpan = TimeSpan.FromSeconds(counter);
            TimerText.Text = "Timer: " + timeSpan.ToString(@"hh\:mm") + "/" + timer.ToString(@"hh\:mm");
        }

        private void ClearRichTextBoxIfNotEmpty()
        {
            if (richTextBox != null)
            {
                richTextBox.Document.Blocks.Clear();
                richTextBoxScrollViewer.ScrollToVerticalOffset(0);
            }
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label label)
            {
                var chapter = label.DataContext as EpubNavigationItem;
                var anchor = chapter.Link.ContentFileUrl;
                richBookViewModel.RenderSectionByAnchor(anchor);
            }
        }
    }
}

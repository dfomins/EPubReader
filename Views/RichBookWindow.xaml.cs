using EPubReader.ViewModels;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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

        public RichTextBoxBookWindow(string BookPath, int seconds)
        {
            InitializeComponent();
            this.seconds = seconds;
            richBookViewModel = new RReaderBookViewModel(BookPath);
            this.DataContext = richBookViewModel;
            richBookWindow.Title = richBookViewModel.bookTitle;
            ClearRichTextBoxIfNotEmpty();
            richTextBox.Document = richBookViewModel.flowDocument;
            bookChaptersListBox.ItemsSource = richBookViewModel.bookChapters;
            richBookViewModel.ClearRichTextBox += ClearRichTextBoxIfNotEmpty;
            richBookViewModel.ChangeColorTheme += SetThemeColor;

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

        private void SetThemeColor(int themeColor)
        {
            if (themeColor == 0)
            {
                bookChaptersListBox.Background = Brushes.White;
                bookChaptersListBox.Foreground = Brushes.Black;
                buttonsPanel.Background = Brushes.White;
                richTextBox.Background = Brushes.White;
                richTextBox.Foreground = Brushes.Black;
            }
            else if (themeColor == 1)
            {
                bookChaptersListBox.Background = Brushes.Black;
                bookChaptersListBox.Foreground = Brushes.White;
                buttonsPanel.Background = Brushes.Black;
                richTextBox.Background = Brushes.Black;
                richTextBox.Foreground = Brushes.White;
            }
        }

        private void ClearRichTextBoxIfNotEmpty()
        {
            if (richTextBox != null)
            {
                richTextBox.Document.Blocks.Clear();
                richTextBoxScrollViewer.ScrollToVerticalOffset(0);
            }
        }

        //test
        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(sender.ToString());
        }
    }
}

using EPubReader.ViewModels;
using System.Diagnostics.Metrics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace EPubReader.Views
{
    /// <summary>
    /// Interaction logic for FlowBookWindow.xaml
    /// </summary>
    public partial class FlowBookWindow : Window
    {
        DispatcherTimer dispatcherTimer { get; }
        int counter { get; set; }
        int seconds { get; }

        public FlowBookWindow(string BookPath, int seconds)
        {
            InitializeComponent();
            this.seconds = seconds;
            FReaderBookViewModel flowBookViewModel = new FReaderBookViewModel(BookPath);
            flowBookWindow.Title = flowBookViewModel.BookTitle;
            flowDocumentReader.Document = flowBookViewModel.flowDocument;
            this.DataContext = flowBookViewModel;

            if (seconds > 0)
            {
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
                dispatcherTimer.Tick += timer_Tick;
                dispatcherTimer.Start();
            } else
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

        private void JumpToPara_Click(object sender, RoutedEventArgs e)
        {
            Paragraph p = new Paragraph(new Run("Uncle Justus was a very quiet, dignified man, with a Roman nose andgray side whiskers."));
            p.BringIntoView();
        }
    }
}

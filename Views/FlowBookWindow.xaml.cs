using EPubReader.ViewModels;
using System.Diagnostics.Metrics;
using System.Windows;
using System.Windows.Controls;
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
        private FReaderBookViewModel flowBookViewModel { get; }
        private DispatcherTimer dispatcherTimer { get; }
        private int counter { get; set; }
        private int seconds { get; }

        public FlowBookWindow(string BookPath, int seconds)
        {
            InitializeComponent();
            flowBookViewModel = new FReaderBookViewModel(BookPath);

            // Data context
            this.seconds = seconds;
            flowBookWindow.Title = flowBookViewModel.bookTitle;
            flowDocumentReader.Document = flowBookViewModel.flowDocument;
            this.DataContext = flowBookViewModel;

            // Invokes
            flowBookViewModel.ScrollToAnchor += ScrollToAnchor;

            // Timer
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

        void ScrollToAnchor(Paragraph p)
        {
            p.BringIntoView();
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
    }
}

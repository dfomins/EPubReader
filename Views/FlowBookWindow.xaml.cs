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
        private int timerMinutes { get; }

        public FlowBookWindow(string BookPath, int timerMinutes, bool showTimer)
        {
            InitializeComponent();
            flowBookViewModel = new FReaderBookViewModel(BookPath, timerMinutes, showTimer);

            // Data context
            this.timerMinutes = timerMinutes;
            flowBookWindow.Title = flowBookViewModel.bookTitle;
            flowDocumentReader.Document = flowBookViewModel.flowDocument;
            DataContext = flowBookViewModel;

            // Invokes
            flowBookViewModel.ScrollToAnchor += ScrollToAnchor;
        }

        void ScrollToAnchor(Paragraph p)
        {
            p.BringIntoView();
        }
    }
}

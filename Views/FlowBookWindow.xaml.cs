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
using System.Windows.Threading;

namespace EPubReader.Views
{
    /// <summary>
    /// Interaction logic for FlowBookWindow.xaml
    /// </summary>
    public partial class FlowBookWindow : Window
    {
        DispatcherTimer dispatcherTimer;
        int counter = 0;
        int seconds;

        public FlowBookWindow(string BookPath, int seconds)
        {
            InitializeComponent();
            this.seconds = seconds;
            BookViewModel bookViewModel = new BookViewModel(BookPath);
            flowBookWindow.Title = bookViewModel.Title;
            flowDocumentReader.Document = bookViewModel.flowDocument;
            if (seconds > 0)
            {
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
                dispatcherTimer.Tick += timer_Tick;
                dispatcherTimer.Start();
            } else
            {
                TextTimer.Text = "Timer: disabled";
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
            TextTimer.Text = "Timer: " + timeSpan.ToString(@"hh\:mm") + "/" + timer.ToString(@"hh\:mm");
        }
    }
}

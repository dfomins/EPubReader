using System.Windows;

namespace EPubReader.Views
{
    /// <summary>
    /// Interaction logic for Timer.xaml
    /// </summary>
    public partial class Timer : Window
    {
        public int timerMinutes { get; set; }
        public bool showTimer { get; set; }

        public Timer(int minutes, bool showTimer)
        {
            InitializeComponent();

            TimeSpan timeSpan = TimeSpan.FromMinutes(minutes);
            TimerHours.Text = timeSpan.Hours.ToString();
            TimerMinutes.Text = timeSpan.Minutes.ToString();
            showTimeOnScreenCheckbox.IsChecked = showTimer;
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            int hours = Convert.ToInt32(TimerHours.Text);
            int minutes = Convert.ToInt32(TimerMinutes.Text);

            timerMinutes = minutes + hours * 60;

            if (showTimeOnScreenCheckbox.IsChecked == true)
                showTimer = true;

            if (timerMinutes > 0)
                DialogResult = true;

            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

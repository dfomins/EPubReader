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
    /// Interaction logic for Timer.xaml
    /// </summary>
    public partial class Timer : Window
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public Timer(int hours, int minutes)
        {
            InitializeComponent();
            TimerHours.Text = hours.ToString();
            TimerMinutes.Text = minutes.ToString();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            Hours = Convert.ToInt32(TimerHours.Text);
            Minutes = Convert.ToInt32(TimerMinutes.Text);
            DialogResult = true;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

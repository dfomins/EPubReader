using EPubReader.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Media.Imaging;

//using System.Windows.Navigation;
//using System.Windows.Shapes;
using VersOne.Epub;
using static System.Reflection.Metadata.BlobBuilder;

namespace EPubReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
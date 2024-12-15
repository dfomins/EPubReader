using EPubReader.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public string selectedFontFamily { get; set; }

        public Options(string CurrentFont)
        {
            InitializeComponent();

            fontFamilyComboBox.SelectedItem = CurrentFont;

            var allSystemFonts = Fonts.SystemFontFamilies.ToArray();
            string[] fontsNameList = new string[allSystemFonts.Length];
            for (int i = 0; i < allSystemFonts.Length; i++)
            {
                fontsNameList[i] = allSystemFonts[i].Source;
            }

            fontFamilyComboBox.ItemsSource = fontsNameList;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (fontFamilyComboBox.SelectedItem != null)
            {
                selectedFontFamily = Convert.ToString(fontFamilyComboBox.SelectedItem);
                DialogResult = true;
            }
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

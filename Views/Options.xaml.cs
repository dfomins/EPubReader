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
        public FontFamily selectedFontFamily { get; set; }
        private string currentFont { get; }
        private string selectedFont { get; }
        private bool fontSizeChangable { get; }

        public Options(string currentFont, string[] fontsNameList, bool fontSizeChangable, int currentFontSize = 18)
        {
            InitializeComponent();

            this.fontSizeChangable = fontSizeChangable;

            if (!this.fontSizeChangable)
            {
                fontFamilySettingsBlock.Visibility = Visibility.Collapsed;
            } else
            {
                currentFontSizeLabel.Content = "Current font size: " + currentFontSize;
            }

            this.currentFont = currentFont;
            fontFamilySelect.SelectedItem = this.currentFont;
            fontPrieviewLabel.FontFamily = new FontFamily(this.currentFont);

            fontFamilySelect.ItemsSource = fontsNameList;
        }

        private void fontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedFontFamily = new FontFamily(Convert.ToString(fontFamilySelect.SelectedItem));
            fontPrieviewLabel.Content = selectedFontFamily.Source;
            fontPrieviewLabel.FontFamily = selectedFontFamily;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (fontFamilySelect.SelectedItem != null)
            {
                selectedFontFamily = selectedFontFamily;
                DialogResult = true;
            }

            if (fontSizeChangable && fontSizeSelect.SelectedItem != null)
            {
                MessageBox.Show(fontSizeSelect.SelectedItem.ToString());
            }

            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

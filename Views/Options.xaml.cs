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
        string currentFont { get; }
        string selectedFont { get; }

        public Options(string currentFont, string[] fontsNameList)
        {
            InitializeComponent();

            this.currentFont = currentFont;
            fontFamilyComboBox.SelectedItem = this.currentFont;
            fontPrieviewTextBlock.FontFamily = new FontFamily(this.currentFont);

            fontFamilyComboBox.ItemsSource = fontsNameList;
        }

        private void fontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedFontFamily = new FontFamily(Convert.ToString(fontFamilyComboBox.SelectedItem));
            fontPrieviewTextBlock.Content = selectedFontFamily.Source;
            fontPrieviewTextBlock.FontFamily = selectedFontFamily;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (fontFamilyComboBox.SelectedItem != null)
            {
                selectedFontFamily = selectedFontFamily;
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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EPubReader.Views
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        // Font family
        public FontFamily fontFamily { get; set; }
        // Is font size changable? (For RichTextBox reader - yes, FlowDocumentReader - no)
        private bool isFontSizeChangable { get; }
        // Font size
        public int fontSize { get; set; }
        // Color theme
        public int themeColor { get; set; }

        public Options(FontFamily currentFontFamily, bool isFontSizeChangable, int currentFontSize = 18, int currentThemeColor = 0)
        {
            InitializeComponent();

            // Generates system fonts list
            FontFamily[] allSystemFonts = System.Windows.Media.Fonts.SystemFontFamilies.ToArray();
            List<string> fontsNameList = new List<string>();
            fontsNameList.AddRange(allSystemFonts.Select(font => font.Source));

            // Font family
            fontFamilySelect.ItemsSource = fontsNameList;
            fontFamily = currentFontFamily;
            fontFamilySelect.SelectedItem = this.fontFamily.Source;
            fontPrieviewLabel.FontFamily = new FontFamily(this.fontFamily.Source);

            // Font size
            this.isFontSizeChangable = isFontSizeChangable;
            fontSize = currentFontSize;

            if (!this.isFontSizeChangable)
            {
                fontFamilySettingsBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                currentFontSizeLabel.Content = "Current font size: " + fontSize;
            }

            // Color theme
            colorThemeSelect.SelectedIndex = currentThemeColor;
        }

        private void fontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fontFamily = new FontFamily(Convert.ToString(fontFamilySelect.SelectedItem));
            fontPrieviewLabel.Content = fontFamily.Source;
            fontPrieviewLabel.FontFamily = fontFamily;
        }

        private void colorThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            themeColor = colorThemeSelect.SelectedIndex;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = fontSizeSelect.SelectedItem as ListBoxItem;

            if (isFontSizeChangable && selectedItem != null)
            {
                fontSize = Convert.ToInt32(selectedItem.Content);
            }

            DialogResult = true;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

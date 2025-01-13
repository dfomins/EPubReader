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
        // UI test
        private ComboBox fontFamilySelect;
        private Label fontPrieviewLabel;
        private ComboBox colorThemeSelect;
        private ListBox fontSizeSelect;
        private Label currentFontSizeLabel;

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

            Grid rootGrid = new Grid
            {
                Margin = new Thickness(10)
            };

            rootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.9, GridUnitType.Star) });
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
            rootGrid.ColumnDefinitions.Add(new ColumnDefinition());
            rootGrid.ColumnDefinitions.Add(new ColumnDefinition());

            Grid innerGrid = new Grid();
            innerGrid.RowDefinitions.Add(new RowDefinition());
            innerGrid.RowDefinitions.Add(new RowDefinition());

            Border fontFamilyBorder = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 5, 10),
                Padding = new Thickness(10)
            };

            StackPanel fontFamilyStack = new StackPanel();
            fontFamilyStack.Children.Add(new Label { Content = "Font family" });

            fontFamilySelect = new ComboBox
            {
                Cursor = System.Windows.Input.Cursors.Hand
            };
            fontFamilySelect.SelectionChanged += FontFamilyComboBox_SelectionChanged;
            fontFamilyStack.Children.Add(fontFamilySelect);

            fontFamilyStack.Children.Add(new Label { Content = "Preview:", Margin = new Thickness(0, 5, 0, 0) });

            fontPrieviewLabel = new Label
            {
                Name = "fontPrieviewLabel",
                FontSize = 18
            };
            fontFamilyStack.Children.Add(fontPrieviewLabel);

            fontFamilyBorder.Child = fontFamilyStack;

            Border colorThemeBorder = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 5, 10),
                Padding = new Thickness(10)
            };

            StackPanel colorThemeStack = new StackPanel();
            colorThemeStack.Children.Add(new Label { Content = "Color theme" });

            colorThemeSelect = new ComboBox
            {
                Name = "colorThemeSelect",
                Cursor = System.Windows.Input.Cursors.Hand
            };
            colorThemeSelect.SelectionChanged += ColorThemeComboBox_SelectionChanged;

            colorThemeSelect.Items.Add(new ComboBoxItem { Content = "White" });
            colorThemeSelect.Items.Add(new ComboBoxItem { Content = "Dark" });

            colorThemeStack.Children.Add(colorThemeSelect);
            colorThemeBorder.Child = colorThemeStack;

            innerGrid.Children.Add(fontFamilyBorder);
            innerGrid.Children.Add(colorThemeBorder);
            Grid.SetRow(colorThemeBorder, 1);

            Border fontSizeBorder = new Border
            {
                Name = "fontFamilySettingsBlock",
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5, 0, 0, 10),
                Padding = new Thickness(10)
            };

            StackPanel fontSizeStack = new StackPanel();
            fontSizeStack.Children.Add(new Label { Content = "Font size" });

            fontSizeSelect = new ListBox { Name = "fontSizeSelect" };

            foreach (int size in new[] { 10, 12, 14, 16, 18, 20, 24, 28, 32, 36 })
            {
                fontSizeSelect.Items.Add(new ListBoxItem { Content = size.ToString() });
            }

            fontSizeStack.Children.Add(fontSizeSelect);
            currentFontSizeLabel = new Label { Name = "currentFontSizeLabel" };
            fontSizeStack.Children.Add(currentFontSizeLabel);

            fontSizeBorder.Child = fontSizeStack;

            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            Button saveButton = new Button
            {
                Content = "Save",
                Width = 70,
                Margin = new Thickness(0, 0, 20, 0)
            };
            saveButton.Click += SaveButton_Click;

            Button closeButton = new Button
            {
                Content = "Close",
                Width = 70
            };
            closeButton.Click += CloseButton_Click;

            buttonPanel.Children.Add(saveButton);
            buttonPanel.Children.Add(closeButton);

            rootGrid.Children.Add(innerGrid);
            rootGrid.Children.Add(fontSizeBorder);
            rootGrid.Children.Add(buttonPanel);

            Grid.SetColumn(innerGrid, 0);
            Grid.SetColumn(fontSizeBorder, 1);
            Grid.SetRow(buttonPanel, 1);
            Grid.SetColumn(buttonPanel, 1);

            this.Content = rootGrid;

            List<string> fontsNameList = new List<string>();
            foreach (var font in Fonts.SystemFontFamilies)
            {
                fontsNameList.Add(font.Source);
            }

            // Font family
            fontFamilySelect.ItemsSource = fontsNameList;
            fontFamily = currentFontFamily;
            fontFamilySelect.SelectedItem = fontFamily.Source;
            fontPrieviewLabel.FontFamily = fontFamily;

            // Font size
            this.isFontSizeChangable = isFontSizeChangable;
            fontSize = currentFontSize;

            if (!this.isFontSizeChangable)
            {
                fontSizeBorder.Visibility = Visibility.Collapsed;
            }
            else
            {
                currentFontSizeLabel.Content = "Current font size: " + fontSize;
            }

            // Color theme
            colorThemeSelect.SelectedIndex = currentThemeColor;
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fontFamily = new FontFamily(Convert.ToString(fontFamilySelect.SelectedItem));
            fontPrieviewLabel.Content = fontFamily.Source;
            fontPrieviewLabel.FontFamily = fontFamily;
        }

        private void ColorThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

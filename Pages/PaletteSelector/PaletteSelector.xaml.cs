using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace SurvivalcraftTextureStudio
{
    /// <summary>
    /// Interaction logic for PaletteSelector.xaml
    /// </summary>
    public partial class PaletteSelector : UserControl
    {
        public PaletteSelector()
        {
            InitializeComponent();
            toggleButton.IsChecked = bool.Parse(MainWindow.Config("isDark"));
        }

        private void Palette_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Palette.DataContext = DataContext;
        }
    }

    public class PaletteNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = (value as string).ToLower();
            switch (input)
            {
                case "yellow": return "黄色";
                case "amber": return "黄褐";
                case "deeporange": return "橘红";
                case "lightblue": return "浅蓝";
                case "teal": return "鸭绿";
                case "cyan": return "青色";
                case "pink": return "粉红";
                case "green": return "绿色";
                case "deeppurple": return "深紫";
                case "indigo": return "靛蓝";
                case "lightgreen": return "浅绿";
                case "blue": return "蓝色";
                case "lime": return "柠青";
                case "red": return "红色";
                case "orange": return "橙色";
                case "purple": return "紫色";
                case "bluegrey": return "蓝灰";
                case "grey": return "灰色";
                case "brown": return "棕色";
                default: return input;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
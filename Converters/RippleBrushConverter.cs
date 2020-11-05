using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SurvivalcraftTextureStudio
{
    public class RippleBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush input = value as SolidColorBrush;
            if (input != null)
            {
                Color color = input.Color;
                Color newColor = Color.FromRgb(color.R, color.G, color.B);
                return new SolidColorBrush(newColor);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
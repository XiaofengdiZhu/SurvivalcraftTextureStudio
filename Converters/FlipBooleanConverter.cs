using System;

namespace SurvivalcraftTextureStudio
{
    public class FlipBooleanConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? input = value as bool?;
            if (input.HasValue)
            {
                return !input.Value;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Windows.Data.Binding.DoNothing;
        }
    }
}
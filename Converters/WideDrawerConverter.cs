using MaterialDesignThemes.Wpf;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SurvivalcraftTextureStudio
{
    public class WideDrawerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? input = value as bool?;
            if (input.HasValue && input.Value)
            {
                return ShadowDepth.Depth2;
            }
            return ShadowDepth.Depth0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
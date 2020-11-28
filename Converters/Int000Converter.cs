using System;
using System.Windows.Documents;

namespace SurvivalcraftTextureStudio
{
    public class Int000Converter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int? input = value as int?;
            if (input.HasValue)
            {
                string output="";
                int length = input.Value.ToString().Length;
                for (int i = 3; i>length ; i--)
                {
                    output += '0';
                }
                return output;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Windows.Data.Binding.DoNothing;
        }
    }
}
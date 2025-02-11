using System.Globalization;
using System.Windows.Data;

namespace ScreenSketcher.Converter
{
    public class ToolSelectedConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == parameter.ToString() ? "Selected" : null;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
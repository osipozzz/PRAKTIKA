using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace WpfApp.Converters
{
    /// <summary>
    /// Конвертер для отображения ошибок валидации
    /// </summary>
    public class HasErrorsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool hasErrors)
                return hasErrors ? "Red" : "Transparent";
            return "Transparent";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Конвертер для видимости элементов при ошибках
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                var invert = parameter?.ToString()?.ToLower() == "invert";
                if (invert)
                    boolValue = !boolValue;

                return boolValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

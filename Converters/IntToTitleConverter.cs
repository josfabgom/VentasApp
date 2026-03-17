using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace VentasApp.Converters
{
    public class IntToTitleConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int id && id > 0)
            {
                return $"Editar {parameter}";
            }
            return $"Nuevo {parameter}";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

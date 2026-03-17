using System;
using System.Globalization;
using System.IO;
using Microsoft.Maui.Controls;

namespace VentasApp.Converters
{
    public class ImagePathConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string path && !string.IsNullOrWhiteSpace(path))
            {
                if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase) ||
                   (!path.Contains("\\") && !path.Contains("/")))
                {
                    // URL o Recurso empaquetado (ej: dotnet_bot.png)
                    return path;
                }

                if (File.Exists(path))
                {
                    // Retorna un ImageSource limpio para MAUI cruzando plataformas
                    return ImageSource.FromFile(path);
                }

                return path;
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Globalization;
using Avalonia.Data.Converters;
using StarSimLib.Models;

namespace StarSimGui.Source.Converters
{
    /// <summary>
    /// Allows for conversion between a <see cref="Body"/> instances raw mass and the formatted mass suitable for display.
    /// </summary>
    public class BodyMassConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{value:E3}";
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
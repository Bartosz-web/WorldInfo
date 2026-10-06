using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CountryInfoApp.Wpf.Converters;

/// <summary>
/// Collapses an element when the bound value is null or an empty string.
/// </summary>
public sealed class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is null || (value is string text && string.IsNullOrWhiteSpace(text))
            ? Visibility.Collapsed
            : Visibility.Visible;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

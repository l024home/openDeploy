using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace OpenDeploy.Client.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var visibility = Visibility.Collapsed;
        if (value is bool flag && flag)
        {
            visibility = Visibility.Visible;
        }
        if (parameter != null && parameter.ToString() == "reverse")
        {
            visibility = visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
        return visibility;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace OpenDeploy.Client.Converters;

public class DataCountToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Visibility visibility = Visibility.Collapsed;
        if (value is int count && count > 0)
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
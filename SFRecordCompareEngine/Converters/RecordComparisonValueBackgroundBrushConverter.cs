using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using SFRecordCompareEngine.ViewModels;

namespace SFRecordCompareEngine.Converters;

public class RecordComparisonValueBackgroundBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value switch
        {
            RecordComparisonValueState.Identical => new SolidColorBrush(ColorHelper.FromArgb(80, 0, 128, 0)),
            RecordComparisonValueState.Conflict => new SolidColorBrush(ColorHelper.FromArgb(80, 192, 0, 0)),
            RecordComparisonValueState.WinningOverride => new SolidColorBrush(ColorHelper.FromArgb(80, 192, 160, 0)),
            _ => new SolidColorBrush(Colors.Transparent)
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotSupportedException();
    }
}
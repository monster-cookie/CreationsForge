using System.Windows;
using System.Windows.Controls;
using SFRecordCompareEngine.Core.Models.Records;
using SFRecordCompareEngine.ViewModels;

namespace SFRecordCompareEngine;

public class RecordComparisonCellTemplateSelector : DataTemplateSelector
{
    public DataTemplate? TextTemplate { get; set; }
    public DataTemplate? BooleanTemplate { get; set; }
    public DataTemplate? TreeTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        return item is RecordComparisonCellViewModel cell
            ? cell.DisplayKind switch
            {
                RecordComparisonFieldDisplayKind.Boolean => BooleanTemplate,
                RecordComparisonFieldDisplayKind.Tree => TreeTemplate,
                _ => TextTemplate
            }
            : TextTemplate;
    }
}

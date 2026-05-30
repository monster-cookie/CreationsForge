using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.ViewModels;

public class RecordComparisonColumnViewModel
{
    public RecordComparisonColumnViewModel(ModKey modKey, int loadOrderIndex, bool isActive, IReadOnlyList<string> values)
    {
        ModKey = modKey;
        LoadOrderIndex = loadOrderIndex;
        IsActive = isActive;
        Values = values.Select(value => new RecordComparisonValueViewModel(value)).ToList();
    }

    public ModKey ModKey { get; }
    public int LoadOrderIndex { get; }
    public bool IsActive { get; }
    public IReadOnlyList<RecordComparisonValueViewModel> Values { get; }
    public string HeaderText => $"[{LoadOrderIndex:D2}] {ModKey.FileName}";
}
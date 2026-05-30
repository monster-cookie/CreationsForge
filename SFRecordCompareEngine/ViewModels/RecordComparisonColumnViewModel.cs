using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.ViewModels;

public class RecordComparisonColumnViewModel
{
    public RecordComparisonColumnViewModel(ModKey modKey, int loadOrderIndex, bool isActive, IReadOnlyList<string> values, IReadOnlyList<RecordComparisonValueState> states, bool isWinningOverride)
    {
        ModKey = modKey;
        LoadOrderIndex = loadOrderIndex;
        IsActive = isActive;
        Values = values.Select((value, index) => new RecordComparisonValueViewModel(value, GetValueState(states[index], isWinningOverride))).ToList();
    }

    public ModKey ModKey { get; }
    public int LoadOrderIndex { get; }
    public bool IsActive { get; }
    public IReadOnlyList<RecordComparisonValueViewModel> Values { get; }
    public string HeaderText => $"[{LoadOrderIndex:D2}] {ModKey.FileName}";

    private static RecordComparisonValueState GetValueState(RecordComparisonValueState state, bool isWinningOverride)
    {
        return state == RecordComparisonValueState.Conflict && isWinningOverride
            ? RecordComparisonValueState.WinningOverride
            : state;
    }
}
using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.ViewModels;

public class RecordComparisonColumnViewModel
{
    public RecordComparisonColumnViewModel(ModKeyDTO modKey, string header, bool isActive)
    {
        ModKey = modKey;
        Header = header;
        IsActive = isActive;
    }

    public ModKeyDTO ModKey { get; }

    public string Header { get; }

    public bool IsActive { get; }
}

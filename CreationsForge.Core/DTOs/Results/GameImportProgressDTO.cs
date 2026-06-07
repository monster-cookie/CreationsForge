namespace CreationsForge.Core.DTOs.Results;

public class GameImportProgressDTO
{
    public required string StatusText { get; set; }

    public string DetailText { get; set; } = string.Empty;

    public double ProgressValue { get; set; }

    public double ProgressMaximum { get; set; } = 100;

    public bool IsIndeterminate { get; set; } = true;

    public string CurrentPluginName { get; set; } = string.Empty;

    public int PluginIndex { get; set; }

    public int PluginCount { get; set; }

    public string CurrentRecordType { get; set; } = string.Empty;

    public int RecordIndex { get; set; }

    public int RecordCount { get; set; }
}

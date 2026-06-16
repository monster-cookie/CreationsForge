namespace CreationsForge.Core.Helpers;

public class RecordTypeData
{
    public string TableName { get; set; } = string.Empty;

    public string RecordType { get; set; } = string.Empty;

    public string RecordID { get; set; } = string.Empty;

    public string FriendlyName { get; set; } = string.Empty;

    public string DisplayLabel => $"{FriendlyName} ({RecordID})";
}

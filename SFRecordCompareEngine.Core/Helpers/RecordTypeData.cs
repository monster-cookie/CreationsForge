namespace SFRecordCompareEngine.Core.Helpers;

public class RecordTypeData
{
    /// <summary>
    ///     The name of the table in the database.
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    ///     The name of the record type. For example, "GameSetting" for game setting records.
    /// </summary>
    public string RecordType { get; set; } = string.Empty;

    /// <summary>
    ///     The ID of the record type. For example, "GMST" for game setting records.
    /// </summary>
    public string RecordID { get; set; } = string.Empty;
}
namespace SFRecordCompareEngine.Core.Helpers;

public static class RecordTypeCatalog
{
    public static readonly RecordTypeData FormList = new()
    {
        TableName = "FormList",
        RecordType = "FormList",
        RecordID = "FLST"
    };
    
    public static readonly RecordTypeData GameSetting = new()
    {
        TableName = "GameSetting",
        RecordType = "GameSetting",
        RecordID = "GMST"
    };
}

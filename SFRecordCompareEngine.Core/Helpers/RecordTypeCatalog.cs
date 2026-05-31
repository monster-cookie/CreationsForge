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

    public static readonly RecordTypeData Global = new()
    {
        TableName = "Global",
        RecordType = "Global",
        RecordID = "GLOB"
    };

    public static readonly RecordTypeData MiscObject = new()
    {
        TableName = "MiscObject",
        RecordType = "MiscObject",
        RecordID = "MISC"
    };

    public static readonly RecordTypeData Keyword = new()
    {
        TableName = "Keyword",
        RecordType = "Keyword",
        RecordID = "KYWD"
    };

    public static readonly RecordTypeData NPC = new()
    {
        TableName = "NPC",
        RecordType = "NPC",
        RecordID = "NPC_"
    };

    public static readonly RecordTypeData ActorValueInformation = new()
    {
        TableName = "ActorValueInformation",
        RecordType = "ActorValueInformation",
        RecordID = "AVIF"
    };

    public static readonly RecordTypeData MagicEffect = new()
    {
        TableName = "MagicEffect",
        RecordType = "MagicEffect",
        RecordID = "MGEF"
    };

    public static readonly RecordTypeData Perk = new()
    {
        TableName = "Perk",
        RecordType = "Perk",
        RecordID = "PERK"
    };
}

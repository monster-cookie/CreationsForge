namespace CreationsForge.Core.Helpers;

public static class RecordTypeCatalog
{
    public static readonly RecordTypeData FormList = new()
    {
        TableName = "FormLists",
        RecordType = "FormList",
        RecordID = "FLST"
    };

    public static readonly RecordTypeData GameSetting = new()
    {
        TableName = "GameSettings",
        RecordType = "GameSetting",
        RecordID = "GMST"
    };

    public static readonly RecordTypeData Global = new()
    {
        TableName = "Globals",
        RecordType = "Global",
        RecordID = "GLOB"
    };

    public static readonly RecordTypeData MiscObject = new()
    {
        TableName = "MiscObjects",
        RecordType = "MiscObject",
        RecordID = "MISC"
    };

    public static readonly RecordTypeData Keyword = new()
    {
        TableName = "Keywords",
        RecordType = "Keyword",
        RecordID = "KYWD"
    };

    public static readonly RecordTypeData ActorValueInformation = new()
    {
        TableName = "ActorValueInformation",
        RecordType = "ActorValueInformation",
        RecordID = "AVIF"
    };

    public static readonly RecordTypeData NPC = new()
    {
        TableName = "NPCs",
        RecordType = "NPC",
        RecordID = "NPC_"
    };

    public static readonly RecordTypeData MagicEffect = new()
    {
        TableName = "MagicEffects",
        RecordType = "MagicEffect",
        RecordID = "MGEF"
    };

    public static readonly RecordTypeData Perk = new()
    {
        TableName = "Perks",
        RecordType = "Perk",
        RecordID = "PERK"
    };
}

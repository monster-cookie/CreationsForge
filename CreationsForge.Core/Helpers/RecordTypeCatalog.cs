namespace CreationsForge.Core.Helpers;

/// <summary>
/// Listing of all currently implemented major record types
/// 
/// NOTE: Please keep these alphabetized. 
/// </summary>
public static class RecordTypeCatalog
{
    public static readonly RecordTypeData ActorValueInformation = new()
    {
        TableName = "ActorValueInformation",
        RecordType = "ActorValueInformation",
        RecordID = "AVIF",
        FriendlyName = "Actor Value Information"
    };

    public static readonly RecordTypeData Book = new()
    {
        TableName = "Books",
        RecordType = "Book",
        RecordID = "BOOK",
        FriendlyName = "Book"
    };

    public static readonly RecordTypeData Container = new()
    {
        TableName = "Containers",
        RecordType = "Container",
        RecordID = "CONT",
        FriendlyName = "Container"
    };

    public static readonly RecordTypeData ConditionForm = new()
    {
        TableName = "ConditionForms",
        RecordType = "ConditionForm",
        RecordID = "CNDF",
        FriendlyName = "Condition Form"
    };

    public static readonly RecordTypeData ConstructibleObject = new()
    {
        TableName = "ConstructibleObjects",
        RecordType = "ConstructibleObject",
        RecordID = "COBJ",
        FriendlyName = "Constructible Object"
    };

    public static readonly RecordTypeData Door = new()
    {
        TableName = "Doors",
        RecordType = "Door",
        RecordID = "DOOR",
        FriendlyName = "Door"
    };

    public static readonly RecordTypeData FormList = new()
    {
        TableName = "FormLists",
        RecordType = "FormList",
        RecordID = "FLST",
        FriendlyName = "Form List"
    };

    public static readonly RecordTypeData GameSetting = new()
    {
        TableName = "GameSettings",
        RecordType = "GameSetting",
        RecordID = "GMST",
        FriendlyName = "Game Setting"
    };

    public static readonly RecordTypeData Global = new()
    {
        TableName = "Globals",
        RecordType = "Global",
        RecordID = "GLOB",
        FriendlyName = "Global"
    };

    public static readonly RecordTypeData Keyword = new()
    {
        TableName = "Keywords",
        RecordType = "Keyword",
        RecordID = "KYWD",
        FriendlyName = "Keyword"
    };

    public static readonly RecordTypeData MagicEffect = new()
    {
        TableName = "MagicEffects",
        RecordType = "MagicEffect",
        RecordID = "MGEF",
        FriendlyName = "Magic Effect"
    };

    public static readonly RecordTypeData MiscObject = new()
    {
        TableName = "MiscItems",
        RecordType = "MiscItems",
        RecordID = "MISC",
        FriendlyName = "Misc Item"
    };

    public static readonly RecordTypeData NPC = new()
    {
        TableName = "NPCs",
        RecordType = "NPC",
        RecordID = "NPC_",
        FriendlyName = "NPC"
    };

    public static readonly RecordTypeData Perk = new()
    {
        TableName = "Perks",
        RecordType = "Perk",
        RecordID = "PERK",
        FriendlyName = "Perk"
    };

    public static readonly RecordTypeData Static = new()
    {
        TableName = "Statics",
        RecordType = "Static",
        RecordID = "STAT",
        FriendlyName = "Static"
    };

    public static readonly RecordTypeData Terminal = new()
    {
        TableName = "Terminals",
        RecordType = "Terminal",
        RecordID = "TERM",
        FriendlyName = "Terminal"
    };

    public static readonly IReadOnlyList<RecordTypeData> All =
    [
        ActorValueInformation,
        Book,
        Container,
        ConditionForm,
        ConstructibleObject,
        Door,
        FormList,
        GameSetting,
        Global,
        Keyword,
        MagicEffect,
        MiscObject,
        NPC,
        Perk,
        Static,
        Terminal
    ];

    public static RecordTypeData? FindByRecordID(string recordID)
    {
        return All.FirstOrDefault(recordType =>
            string.Equals(recordType.RecordID, recordID, StringComparison.OrdinalIgnoreCase));
    }

    public static string GetDisplayLabel(string recordID)
    {
        return FindByRecordID(recordID)?.DisplayLabel ?? recordID;
    }
}

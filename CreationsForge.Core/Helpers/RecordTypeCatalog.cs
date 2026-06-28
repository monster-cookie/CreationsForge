using CreationsForge.Specification.Records;

namespace CreationsForge.Core.Helpers;

/// <summary>
/// Adapts production record specifications into the Core record-type metadata shape used by legacy call sites.
/// </summary>
public static class RecordTypeCatalog
{
    public static readonly RecordTypeData ActorValueInformation = RecordTypeData.FromSpecification(SupportedRecordSpecifications.ActorValueInformation);
    public static readonly RecordTypeData Book = RecordTypeData.FromSpecification(SupportedRecordSpecifications.Book);
    public static readonly RecordTypeData Class = RecordTypeData.FromSpecification(SupportedRecordSpecifications.Class);
    public static readonly RecordTypeData Container = RecordTypeData.FromSpecification(SupportedRecordSpecifications.Container);
    public static readonly RecordTypeData ConditionForm = RecordTypeData.FromSpecification(SupportedRecordSpecifications.ConditionForm);
    public static readonly RecordTypeData ConstructibleObject = RecordTypeData.FromSpecification(SupportedRecordSpecifications.ConstructibleObject);
    public static readonly RecordTypeData Door = RecordTypeData.FromSpecification(SupportedRecordSpecifications.Door);
    public static readonly RecordTypeData Faction = RecordTypeData.FromSpecification(SupportedRecordSpecifications.Faction);
    public static readonly RecordTypeData FormList = RecordTypeData.FromSpecification(SupportedRecordSpecifications.FormList);
    public static readonly RecordTypeData GameSetting = RecordTypeData.FromSpecification(SupportedRecordSpecifications.GameSetting);
    public static readonly RecordTypeData Global = RecordTypeData.FromSpecification(SupportedRecordSpecifications.Global);
    public static readonly RecordTypeData Keyword = RecordTypeData.FromSpecification(SupportedRecordSpecifications.Keyword);
    public static readonly RecordTypeData MagicEffect = RecordTypeData.FromSpecification(SupportedRecordSpecifications.MagicEffect);
    public static readonly RecordTypeData MiscItem = RecordTypeData.FromSpecification(SupportedRecordSpecifications.MiscItem);
    public static readonly RecordTypeData NPC = RecordTypeData.FromSpecification(SupportedRecordSpecifications.NPC);
    public static readonly RecordTypeData Perk = RecordTypeData.FromSpecification(SupportedRecordSpecifications.Perk);
    public static readonly RecordTypeData Static = RecordTypeData.FromSpecification(SupportedRecordSpecifications.Static);
    public static readonly RecordTypeData Terminal = RecordTypeData.FromSpecification(SupportedRecordSpecifications.Terminal);

    public static readonly IReadOnlyList<RecordTypeData> All =
    [
        ActorValueInformation,
        Book,
        Class,
        Container,
        ConditionForm,
        ConstructibleObject,
        Door,
        Faction,
        FormList,
        GameSetting,
        Global,
        Keyword,
        MagicEffect,
        MiscItem,
        NPC,
        Perk,
        Static,
        Terminal
    ];

    /// <summary>
    /// Finds a Core record-type shape by Bethesda record identifier.
    /// </summary>
    /// <param name="recordID">The record identifier to locate.</param>
    /// <returns>The adapted record type, or <c>null</c> when no specification is registered for the identifier.</returns>
    public static RecordTypeData? FindByRecordID(string recordID)
    {
        return All.FirstOrDefault(recordType =>
            string.Equals(recordType.RecordID, recordID, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the display label for a record identifier.
    /// </summary>
    /// <param name="recordID">The record identifier to display.</param>
    /// <returns>The known display label, or the original identifier when it is unknown.</returns>
    public static string GetDisplayLabel(string recordID)
    {
        return FindByRecordID(recordID)?.DisplayLabel ?? recordID;
    }
}

namespace CreationsForge.Specification.Records;

/// <summary>
/// Exposes the static production record specification catalog.
/// </summary>
public static class RecordSpecificationCatalog
{
    /// <summary>
    /// Gets every record specification included in the current foundation slice.
    /// </summary>
    public static IReadOnlyList<RecordSpecification> All => SupportedRecordSpecifications.All;

    /// <summary>
    /// Finds a record specification by Bethesda record identifier using an ordinal, case-insensitive match.
    /// </summary>
    /// <param name="recordID">The four-character record identifier to search for.</param>
    /// <returns>The matching specification, or <c>null</c> when the identifier is empty or unknown.</returns>
    public static RecordSpecification? FindByRecordID(string recordID)
    {
        if (string.IsNullOrWhiteSpace(recordID))
        {
            return null;
        }

        return All.FirstOrDefault(specification =>
            string.Equals(specification.RecordID, recordID, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the record specifications that currently advertise support for the requested game.
    /// </summary>
    /// <param name="game">The game adapter to filter specifications by.</param>
    /// <returns>The specifications with at least one support entry for <paramref name="game"/>.</returns>
    public static IReadOnlyList<RecordSpecification> GetSupportedByGame(SpecificationGame game)
    {
        return All
            .Where(specification => specification.GameSupport.Any(support => support.Game == game))
            .ToList();
    }
}

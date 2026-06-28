namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides production record specifications to Core services without requiring callers to know the static catalog
/// shape.
/// </summary>
public interface IRecordSpecificationProvider
{
    /// <summary>
    /// Gets all record specifications known to the current production specification catalog.
    /// </summary>
    /// <returns>The immutable list of known record specifications.</returns>
    IReadOnlyList<RecordSpecification> GetAll();

    /// <summary>
    /// Finds a record specification by Bethesda record identifier.
    /// </summary>
    /// <param name="recordID">The four-character record identifier to search for.</param>
    /// <returns>The matching specification, or <c>null</c> when the identifier is unknown.</returns>
    RecordSpecification? FindByRecordID(string recordID);

    /// <summary>
    /// Gets the record specifications that currently advertise support for the requested game.
    /// </summary>
    /// <param name="game">The game adapter to filter specifications by.</param>
    /// <returns>The specifications with at least one support entry for <paramref name="game"/>.</returns>
    IReadOnlyList<RecordSpecification> GetSupportedByGame(SpecificationGame game);
}

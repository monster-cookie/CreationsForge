namespace CreationsForge.Specification.Records;

/// <summary>
/// Default implementation that exposes the static record specification catalog through an injectable service
/// boundary.
/// </summary>
public sealed class RecordSpecificationProvider : IRecordSpecificationProvider
{
    /// <inheritdoc />
    public IReadOnlyList<RecordSpecification> GetAll()
    {
        return RecordSpecificationCatalog.All;
    }

    /// <inheritdoc />
    public RecordSpecification? FindByRecordID(string recordID)
    {
        return RecordSpecificationCatalog.FindByRecordID(recordID);
    }

    /// <inheritdoc />
    public IReadOnlyList<RecordSpecification> GetSupportedByGame(SpecificationGame game)
    {
        return RecordSpecificationCatalog.GetSupportedByGame(game);
    }
}

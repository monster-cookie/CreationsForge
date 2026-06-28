using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Specification.Records;

namespace CreationsForge.Core.Services;

/// <summary>
/// Builds plugin record sets by applying specification reader metadata to mapped record-family collections.
/// </summary>
public sealed class RecordSetSpecificationBuilder : IRecordSetSpecificationBuilder
{
    private readonly IRecordSpecificationProvider RecordSpecificationProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordSetSpecificationBuilder"/> class.
    /// </summary>
    /// <param name="recordSpecificationProvider">The provider that exposes production record specifications.</param>
    public RecordSetSpecificationBuilder(IRecordSpecificationProvider recordSpecificationProvider)
    {
        RecordSpecificationProvider = recordSpecificationProvider;
    }

    /// <summary>
    /// Creates a builder that uses the production static record specification provider.
    /// </summary>
    /// <returns>A builder configured with the production specification catalog.</returns>
    public static RecordSetSpecificationBuilder CreateDefault()
    {
        return new RecordSetSpecificationBuilder(new RecordSpecificationProvider());
    }

    /// <inheritdoc />
    public PluginRecordSetDTO Build(
        SupportedGame game,
        IReadOnlyDictionary<string, object> recordsByRecordID)
    {
        ArgumentNullException.ThrowIfNull(recordsByRecordID);

        var specificationGame = MapSpecificationGame(game);
        var recordsByRecordIDLookup = CreateCaseInsensitiveLookup(recordsByRecordID);
        var recordSet = new PluginRecordSetDTO();
        var specifications = RecordSpecificationProvider
            .GetSupportedByGame(specificationGame)
            .OrderBy(specification => specification.Import.ImportOrder);

        foreach (var specification in specifications)
        {
            if (!recordsByRecordIDLookup.TryGetValue(specification.RecordID, out var records))
            {
                throw new InvalidOperationException(
                    $"No mapped record collection was supplied for {game} {specification.RecordID}.");
            }

            SetRecordSetProperty(recordSet, specification, records);
        }

        return recordSet;
    }

    /// <summary>
    /// Converts Core game identifiers to specification-layer game identifiers.
    /// </summary>
    /// <param name="game">The Core game identifier.</param>
    /// <returns>The matching specification-layer game identifier.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the game is not known to the specification layer.</exception>
    private static SpecificationGame MapSpecificationGame(SupportedGame game)
    {
        return game switch
        {
            SupportedGame.Starfield => SpecificationGame.Starfield,
            SupportedGame.Fallout4 => SpecificationGame.Fallout4,
            SupportedGame.Skyrim => SpecificationGame.Skyrim,
            _ => throw new ArgumentOutOfRangeException(nameof(game), game, "The game is not supported by record specifications.")
        };
    }

    /// <summary>
    /// Creates a case-insensitive record-ID lookup while preserving duplicate detection under the target comparer.
    /// </summary>
    /// <param name="recordsByRecordID">The source record-family collections keyed by record identifier.</param>
    /// <returns>A case-insensitive record-ID lookup.</returns>
    private static Dictionary<string, object> CreateCaseInsensitiveLookup(
        IReadOnlyDictionary<string, object> recordsByRecordID)
    {
        var lookup = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        foreach (var (recordID, records) in recordsByRecordID)
        {
            lookup.Add(recordID, records);
        }

        return lookup;
    }

    /// <summary>
    /// Assigns a mapped record-family collection to the record-set property named by the specification.
    /// </summary>
    /// <param name="recordSet">The record set being populated.</param>
    /// <param name="specification">The record specification that names the target collection property.</param>
    /// <param name="records">The mapped collection value to assign.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the target property is missing, read-only, or incompatible with the supplied collection value.
    /// </exception>
    private static void SetRecordSetProperty(
        PluginRecordSetDTO recordSet,
        RecordSpecification specification,
        object records)
    {
        var property = typeof(PluginRecordSetDTO).GetProperty(specification.Reader.PluginRecordSetPropertyName);
        if (property == null)
        {
            throw new InvalidOperationException(
                $"Record specification '{specification.RecordID}' references unknown PluginRecordSetDTO property " +
                $"'{specification.Reader.PluginRecordSetPropertyName}'.");
        }

        if (!property.CanWrite)
        {
            throw new InvalidOperationException(
                $"PluginRecordSetDTO property '{specification.Reader.PluginRecordSetPropertyName}' for record " +
                $"specification '{specification.RecordID}' is read-only.");
        }

        if (records == null)
        {
            throw new InvalidOperationException(
                $"Mapped records for {specification.RecordID} cannot be null.");
        }

        if (!property.PropertyType.IsAssignableFrom(records.GetType()))
        {
            throw new InvalidOperationException(
                $"Mapped records for {specification.RecordID} are '{records.GetType().Name}', which cannot be " +
                $"assigned to PluginRecordSetDTO property '{property.Name}' of type '{property.PropertyType.Name}'.");
        }

        property.SetValue(recordSet, records);
    }
}

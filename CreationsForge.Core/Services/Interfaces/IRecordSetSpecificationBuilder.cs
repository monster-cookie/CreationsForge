using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Services.Interfaces;

/// <summary>
/// Builds plugin record sets from record-family collections using production record specification metadata.
/// </summary>
public interface IRecordSetSpecificationBuilder
{
    /// <summary>
    /// Builds a <see cref="PluginRecordSetDTO"/> for the selected game by assigning each supplied record-family list
    /// to the destination collection named by the active record specification.
    /// </summary>
    /// <param name="game">The game whose supported record specifications should be used.</param>
    /// <param name="recordsByRecordID">The mapped record-family collections keyed by Bethesda record identifier.</param>
    /// <returns>A populated plugin record set containing the supplied supported record-family collections.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="recordsByRecordID"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="game"/> is not supported by specifications.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a supported specification is missing a mapped collection, points at an invalid record-set property,
    /// or receives a collection value that is not assignable to the target property.
    /// </exception>
    PluginRecordSetDTO Build(
        SupportedGame game,
        IReadOnlyDictionary<string, object> recordsByRecordID);
}

using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Core.Engine.RecordReading;

/// <summary>Builds detached plugin summaries while counting physical and load-order-unique major records.</summary>
public static class PluginSummaryBuilder
{
    /// <summary>Enumerates complete Mutagen plugin views and records counts without retaining a record index.</summary>
    /// <param name="sourceMods">The complete source plugin views in admitted load-order order.</param>
    /// <param name="sourceInputs">The aligned source identities, paths, positions, and roles.</param>
    /// <param name="outputMod">The optional complete mutable output view appended after the sources.</param>
    /// <param name="outputAssociation">The output identity and path, or <see langword="null"/> when no output is selected.</param>
    /// <param name="cancellationToken">A token observed before every plugin and major record.</param>
    /// <returns>Plugin summaries whose record count includes overrides and whose unique contribution counts each FormKey only at its first load-order appearance.</returns>
    /// <exception cref="ArgumentException">Thrown when source collections are unaligned or only one output argument is supplied.</exception>
    /// <exception cref="ArgumentNullException">Thrown when a required source collection is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public static EngineResult<IReadOnlyList<PluginSummary>> Build(
        IReadOnlyList<IModGetter> sourceMods,
        IReadOnlyList<PluginSourcePluginInput> sourceInputs,
        IModGetter? outputMod = null,
        OutputAssociation? outputAssociation = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sourceMods);
        ArgumentNullException.ThrowIfNull(sourceInputs);
        if (sourceMods.Count != sourceInputs.Count)
        {
            throw new ArgumentException("Every plugin summary source must have aligned Mutagen and input metadata.", nameof(sourceMods));
        }

        if ((outputMod is null) != (outputAssociation is null))
        {
            throw new ArgumentException("Output plugin summary counting requires both a Mutagen view and its association.", nameof(outputMod));
        }

        var summaries = new List<PluginSummary>(sourceInputs.Count + (outputMod is null ? 0 : 1));
        var observedFormKeys = new HashSet<FormKey>();
        var warnings = new List<EngineWarning>();
        var uniqueCountsAvailable = true;
        for (var index = 0; index < sourceInputs.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var input = sourceInputs[index];
            summaries.Add(CreateSummary(
                sourceMods[index],
                input.ModKey,
                input.Path,
                input.LoadOrderIndex,
                input.Role,
                observedFormKeys,
                warnings,
                ref uniqueCountsAvailable,
                cancellationToken));
        }

        if (outputMod is not null && outputAssociation is not null)
        {
            summaries.Add(CreateSummary(
                outputMod,
                outputAssociation.ModKey,
                outputAssociation.PluginPath,
                summaries.Count,
                PluginRole.Output,
                observedFormKeys,
                warnings,
                ref uniqueCountsAvailable,
                cancellationToken));
        }

        return EngineResult<IReadOnlyList<PluginSummary>>.Success(
            Array.AsReadOnly(summaries.ToArray()),
            warnings: Array.AsReadOnly(warnings.ToArray()));
    }

    /// <summary>Counts one complete plugin and adds its successfully enumerated identities to the load-order union.</summary>
    /// <param name="mod">The complete Mutagen plugin view.</param>
    /// <param name="modKey">The expected plugin identity.</param>
    /// <param name="path">The canonical plugin path.</param>
    /// <param name="loadOrderIndex">The plugin's zero-based load-order position.</param>
    /// <param name="role">The plugin's workspace role.</param>
    /// <param name="observedFormKeys">The FormKeys successfully observed in earlier plugins.</param>
    /// <param name="warnings">The warning sink for optional count failures.</param>
    /// <param name="uniqueCountsAvailable">Whether every preceding plugin was enumerated completely.</param>
    /// <param name="cancellationToken">A token observed for every major record.</param>
    /// <returns>A detached plugin summary with available counts or explicit unavailable values.</returns>
    private static PluginSummary CreateSummary(
        IModGetter mod,
        ModKey modKey,
        string path,
        int loadOrderIndex,
        PluginRole role,
        HashSet<FormKey> observedFormKeys,
        List<EngineWarning> warnings,
        ref bool uniqueCountsAvailable,
        CancellationToken cancellationToken)
    {
        long recordCount = 0;
        long uniqueRecordContributionCount = 0;
        try
        {
            foreach (var record in mod.EnumerateMajorRecords())
            {
                cancellationToken.ThrowIfCancellationRequested();
                recordCount++;
                if (uniqueCountsAvailable && observedFormKeys.Add(record.FormKey))
                {
                    uniqueRecordContributionCount++;
                }
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            uniqueCountsAvailable = false;
            warnings.Add(new EngineWarning(
                "plugin-record-count-unavailable",
                $"Record counts for plugin '{modKey.FileName}' are unavailable: {exception.Message}"));
            return new PluginSummary(modKey, path, loadOrderIndex, role);
        }

        return new PluginSummary(
            modKey,
            path,
            loadOrderIndex,
            role,
            recordCount,
            uniqueCountsAvailable ? uniqueRecordContributionCount : null);
    }
}

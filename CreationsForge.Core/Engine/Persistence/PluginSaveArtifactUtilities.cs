using System.Security.Cryptography;
using System.Text;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Engine.PluginOutputs;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Records;
using System.IO.Abstractions;

namespace CreationsForge.Core.Engine.Persistence;

/// <summary>Provides bounded artifact capture, comparison, token, and owned-copy operations for guarded saves.</summary>
internal static class PluginSaveArtifactUtilities
{
    /// <summary>Compares canonical paths using platform file-name semantics.</summary>
    internal static StringComparer PathComparer { get; } = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;

    /// <summary>Captures the complete plugin and all release-defined loose-string paths for an output association.</summary>
    /// <param name="release">The exact plugin release.</param>
    /// <param name="output">The complete output association.</param>
    /// <param name="cancellationToken">The token checked while discovering and hashing artifacts.</param>
    /// <returns>A fresh deterministic output baseline.</returns>
    internal static async Task<OutputArtifactSetBaseline> CaptureOutputAsync(
        GameRelease release,
        OutputAssociation output,
        CancellationToken cancellationToken)
    {
        var plugin = new PluginOutputPluginInput(
            output.PluginPath,
            output.ModKey,
            File.Exists(output.PluginPath),
            MapMasterStyle(output.MasterStyle),
            output.LocalizedOutputMode == LocalizedOutputMode.SeparateStringFiles);
        var collector = new PluginOutputArtifactCollector(release, plugin);
        var artifacts = await collector.CaptureAsync(cancellationToken).ConfigureAwait(false);
        return new OutputArtifactSetBaseline(
            PluginArtifactSetUtilities.CreateBaselineId("CreationsForge.OutputArtifactSetBaseline/v1", artifacts),
            artifacts);
    }

    /// <summary>Recaptures the exact paths and roles recorded in a trusted bounded artifact set.</summary>
    /// <param name="expected">The artifact shape whose paths, roles, and languages are recaptured.</param>
    /// <param name="cancellationToken">The token checked while hashing artifacts.</param>
    /// <returns>The fresh observations in the same order.</returns>
    internal static async Task<IReadOnlyList<PluginArtifactAssociation>> RecaptureAsync(
        IReadOnlyList<PluginArtifactAssociation> expected,
        CancellationToken cancellationToken)
    {
        var observations = new PluginArtifactAssociation[expected.Count];
        for (var index = 0; index < expected.Count; index++)
        {
            var artifact = expected[index];
            observations[index] = await PluginFileInspector.InspectAsync(
                artifact.Path,
                artifact.Role,
                artifact.Language,
                mustExist: false,
                cancellationToken).ConfigureAwait(false);
        }

        return Array.AsReadOnly(observations);
    }

    /// <summary>Recaptures a source artifact set with the same localized-entry archive fingerprint used during workspace admission.</summary>
    /// <param name="release">The exact game release used to read applicable strings archives.</param>
    /// <param name="expected">The admitted source artifact shape whose paths, roles, and languages are recaptured.</param>
    /// <param name="cancellationToken">The token checked while reading and hashing artifacts.</param>
    /// <returns>The fresh observations in the same order and representation as the admitted source baseline.</returns>
    internal static async Task<IReadOnlyList<PluginArtifactAssociation>> RecaptureSourceAsync(
        GameRelease release,
        IReadOnlyList<PluginArtifactAssociation> expected,
        CancellationToken cancellationToken)
    {
        var targetFileNames = expected
            .Where(artifact => artifact.Role is
                PluginArtifactRole.Strings or
                PluginArtifactRole.DlStrings or
                PluginArtifactRole.IlStrings)
            .Select(artifact => Path.GetFileName(artifact.Path))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var fileSystem = new FileSystem();
        var observations = new PluginArtifactAssociation[expected.Count];
        for (var index = 0; index < expected.Count; index++)
        {
            var artifact = expected[index];
            observations[index] = artifact.Role == PluginArtifactRole.StringsArchive
                ? await PluginFileInspector.InspectArchiveStringsAsync(
                    artifact.Path,
                    release,
                    targetFileNames,
                    fileSystem,
                    cancellationToken).ConfigureAwait(false)
                : await PluginFileInspector.InspectAsync(
                    artifact.Path,
                    artifact.Role,
                    artifact.Language,
                    mustExist: false,
                    cancellationToken).ConfigureAwait(false);
        }

        return Array.AsReadOnly(observations);
    }

    /// <summary>Creates a fresh output baseline from already recaptured destination artifacts.</summary>
    /// <param name="artifacts">The complete canonical output artifacts.</param>
    /// <returns>The deterministic fresh baseline.</returns>
    internal static OutputArtifactSetBaseline CreateOutputBaseline(IReadOnlyList<PluginArtifactAssociation> artifacts)
    {
        var ordered = artifacts
            .OrderBy(artifact => artifact.Path, PathComparer)
            .ThenBy(artifact => artifact.Role)
            .ThenBy(artifact => artifact.Language, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        return new OutputArtifactSetBaseline(
            PluginArtifactSetUtilities.CreateBaselineId("CreationsForge.OutputArtifactSetBaseline/v1", ordered),
            ordered);
    }

    /// <summary>Determines whether two artifact sets match in path, role, language, content, and physical identity.</summary>
    /// <param name="expected">The trusted expected artifacts.</param>
    /// <param name="actual">The fresh actual artifacts.</param>
    /// <returns><see langword="true"/> when the observations are exact.</returns>
    internal static bool MatchExact(
        IReadOnlyList<PluginArtifactAssociation> expected,
        IReadOnlyList<PluginArtifactAssociation> actual)
    {
        return PluginArtifactSetUtilities.Match(expected, actual);
    }

    /// <summary>Determines whether output associations identify the same canonical output and plugin options.</summary>
    /// <param name="left">The first association.</param>
    /// <param name="right">The second association.</param>
    /// <returns><see langword="true"/> when all fields match.</returns>
    internal static bool MatchOutput(OutputAssociation left, OutputAssociation right)
    {
        return PathComparer.Equals(Path.GetFullPath(left.PluginPath), Path.GetFullPath(right.PluginPath))
            && left.ModKey == right.ModKey
            && left.LocalizedOutputMode == right.LocalizedOutputMode
            && left.MasterStyle == right.MasterStyle;
    }

    /// <summary>Determines whether two output baselines match exactly, including deterministic identity.</summary>
    /// <param name="left">The first baseline.</param>
    /// <param name="right">The second baseline.</param>
    /// <returns><see langword="true"/> when the baselines are equal.</returns>
    internal static bool MatchBaseline(OutputArtifactSetBaseline left, OutputArtifactSetBaseline right)
    {
        return left.BaselineId == right.BaselineId && MatchExact(left.Artifacts, right.Artifacts);
    }

    /// <summary>Determines whether two source baselines match exactly.</summary>
    /// <param name="left">The first source baseline.</param>
    /// <param name="right">The second source baseline.</param>
    /// <returns><see langword="true"/> when every identity and artifact matches.</returns>
    internal static bool MatchSourceBaseline(
        PluginSourceInputBaseline left,
        PluginSourceInputBaseline right)
    {
        return left.BaselineId == right.BaselineId && MatchExact(left.Artifacts, right.Artifacts);
    }

    /// <summary>Copies one staged or backup source into a new transaction-owned file and flushes its bytes to disk.</summary>
    /// <param name="sourcePath">The existing source file.</param>
    /// <param name="destinationPath">The absent transaction-owned destination file.</param>
    /// <param name="cancellationToken">The token honored before the destination mutation boundary.</param>
    /// <returns>A task that completes after the owned copy is flushed.</returns>
    internal static async Task CopyAndFlushAsync(
        string sourcePath,
        string destinationPath,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var source = new FileStream(
            sourcePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read | FileShare.Delete,
            131072,
            FileOptions.Asynchronous | FileOptions.SequentialScan);
        await using var destination = new FileStream(
            destinationPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            131072,
            FileOptions.Asynchronous | FileOptions.WriteThrough);
        await source.CopyToAsync(destination, cancellationToken).ConfigureAwait(false);
        destination.Flush(flushToDisk: true);
    }

    /// <summary>Creates an evidence token bound to a canonical journal and all currently observed supporting artifacts.</summary>
    /// <param name="journal">The complete journal snapshot.</param>
    /// <param name="observations">The current destination, staged, publication, backup, and source observations.</param>
    /// <returns>An opaque uppercase SHA-256 token.</returns>
    internal static RecoveryEvidenceToken CreateEvidenceToken(
        SaveTransactionJournal journal,
        IEnumerable<PluginArtifactAssociation> observations)
    {
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        AppendBytes(hash, SaveTransactionJournalCodec.Write(journal));
        foreach (var observation in observations)
        {
            AppendString(hash, observation.Path);
            AppendString(hash, ((int)observation.Role).ToString(System.Globalization.CultureInfo.InvariantCulture));
            AppendString(hash, observation.Language ?? string.Empty);
            AppendString(hash, observation.Fingerprint.Exists ? "1" : "0");
            AppendString(hash, observation.Fingerprint.Length.ToString(System.Globalization.CultureInfo.InvariantCulture));
            AppendString(hash, observation.Fingerprint.Sha256 ?? string.Empty);
            AppendString(hash, observation.FileIdentity?.Provider ?? string.Empty);
            AppendString(hash, observation.FileIdentity?.VolumeId ?? string.Empty);
            AppendString(hash, observation.FileIdentity?.FileId ?? string.Empty);
            AppendString(hash, observation.FileIdentity?.LinkCount?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty);
        }

        return new RecoveryEvidenceToken(Convert.ToHexString(hash.GetHashAndReset()));
    }

    /// <summary>Ensures a path is a canonical descendant of a fixed root.</summary>
    /// <param name="path">The candidate canonical absolute path.</param>
    /// <param name="root">The canonical absolute directory root.</param>
    /// <param name="description">The path description used in failures.</param>
    /// <returns>The canonical full path.</returns>
    /// <exception cref="InvalidDataException">Thrown when the candidate escapes the root or is not canonical.</exception>
    internal static string RequireDescendant(string path, string root, string description)
    {
        var fullPath = Path.GetFullPath(path);
        var fullRoot = Path.TrimEndingDirectorySeparator(Path.GetFullPath(root));
        if (!PathComparer.Equals(path, fullPath)
            || PathComparer.Equals(fullPath, fullRoot)
            || !fullPath.StartsWith(fullRoot + Path.DirectorySeparatorChar, PathComparison))
        {
            throw new InvalidDataException($"The {description} path escapes its bounded directory: '{path}'.");
        }

        return fullPath;
    }

    /// <summary>Appends length-delimited bytes to a canonical digest.</summary>
    /// <param name="hash">The digest receiving the bytes.</param>
    /// <param name="bytes">The bytes to append.</param>
    private static void AppendBytes(IncrementalHash hash, ReadOnlySpan<byte> bytes)
    {
        hash.AppendData(BitConverter.GetBytes(bytes.Length));
        hash.AppendData(bytes);
    }

    /// <summary>Appends one length-delimited UTF-8 string to a canonical digest.</summary>
    /// <param name="hash">The digest receiving the value.</param>
    /// <param name="value">The value to append.</param>
    private static void AppendString(IncrementalHash hash, string value)
    {
        AppendBytes(hash, Encoding.UTF8.GetBytes(value));
    }

    /// <summary>Maps the shared output master style to Mutagen's record value.</summary>
    /// <param name="style">The shared output style.</param>
    /// <returns>The plugin master style.</returns>
    private static MasterStyle MapMasterStyle(OutputMasterStyle style)
    {
        return style switch
        {
            OutputMasterStyle.Full => MasterStyle.Full,
            OutputMasterStyle.Small => MasterStyle.Small,
            OutputMasterStyle.Medium => MasterStyle.Medium,
            _ => throw new ArgumentOutOfRangeException(nameof(style))
        };
    }

    /// <summary>Gets platform path prefix comparison semantics.</summary>
    private static StringComparison PathComparison => OperatingSystem.IsWindows()
        ? StringComparison.OrdinalIgnoreCase
        : StringComparison.Ordinal;
}

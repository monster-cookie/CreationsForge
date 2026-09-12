using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Core.Engine.NativeOutputs;

/// <summary>Captures the complete plugin and loose strings inventory for one explicit native output.</summary>
internal sealed class NativeOutputArtifactCollector
{
    /// <summary>The selected native game release.</summary>
    private readonly GameRelease Release;

    /// <summary>The verified native output descriptor.</summary>
    private readonly NativeOutputPluginInput Output;

    /// <summary>Initializes a complete output artifact collector.</summary>
    /// <param name="release">The selected native game release.</param>
    /// <param name="output">The verified output descriptor.</param>
    internal NativeOutputArtifactCollector(GameRelease release, NativeOutputPluginInput output)
    {
        Release = release;
        Output = output;
    }

    /// <summary>Captures the output plugin plus every known sibling loose strings sidecar state.</summary>
    /// <param name="cancellationToken">The token checked during inventory and hashing.</param>
    /// <returns>The complete immutable artifact set in canonical path order.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    /// <exception cref="NativeSourceInputException">Thrown when an artifact cannot be observed safely.</exception>
    internal async Task<IReadOnlyList<NativeArtifactAssociation>> CaptureAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        NativeFileInspector.VerifyDirectory(Path.GetDirectoryName(Output.Path)!, "output plugin directory");
        var artifacts = new List<NativeArtifactAssociation>
        {
            await NativeFileInspector.InspectAsync(
                Output.Path,
                NativeArtifactRole.Plugin,
                null,
                mustExist: false,
                cancellationToken).ConfigureAwait(false)
        };

        var constants = GameConstants.Get(Release);
        if (constants.StringsLanguageFormat is { } languageFormat)
        {
            var outputDirectory = Path.GetDirectoryName(Output.Path)!;
            var stringsDirectory = Path.Combine(outputDirectory, "Strings");
            if (File.Exists(stringsDirectory))
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.InvalidRequest,
                    $"The output localized-string path identifies a file instead of a directory: '{stringsDirectory}'.");
            }

            if (Directory.Exists(stringsDirectory))
            {
                NativeFileInspector.VerifyDirectory(stringsDirectory, "output localized-string directory");
            }

            foreach (var source in OrderedStringsSources)
            {
                foreach (var language in constants.Languages.OrderBy(language => language))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var fileName = StringsUtility.GetFileName(languageFormat, Output.ModKey, language, source);
                    var path = Path.GetFullPath(Path.Combine(stringsDirectory, fileName));
                    artifacts.Add(await NativeFileInspector.InspectAsync(
                        path,
                        MapRole(source),
                        language.ToString(),
                        mustExist: false,
                        cancellationToken).ConfigureAwait(false));
                }
            }
        }

        var pathComparer = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        return Array.AsReadOnly(artifacts
            .OrderBy(artifact => artifact.Path, pathComparer)
            .ThenBy(artifact => artifact.Role)
            .ThenBy(artifact => artifact.Language, StringComparer.OrdinalIgnoreCase)
            .ToArray());
    }

    /// <summary>Maps a native strings source to its physical sidecar role.</summary>
    /// <param name="source">The native strings source.</param>
    /// <returns>The corresponding artifact role.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown for an unknown native strings source.</exception>
    private static NativeArtifactRole MapRole(StringsSource source)
    {
        return source switch
        {
            StringsSource.Normal => NativeArtifactRole.Strings,
            StringsSource.DL => NativeArtifactRole.DlStrings,
            StringsSource.IL => NativeArtifactRole.IlStrings,
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };
    }

    /// <summary>Gets native strings sources in stable artifact order.</summary>
    private static IReadOnlyList<StringsSource> OrderedStringsSources { get; } = Array.AsReadOnly(
        new[] { StringsSource.Normal, StringsSource.DL, StringsSource.IL });
}

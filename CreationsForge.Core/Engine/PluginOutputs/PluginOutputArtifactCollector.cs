using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Core.Engine.PluginOutputs;

/// <summary>Captures the complete plugin and loose strings inventory for one explicit plugin output.</summary>
internal sealed class PluginOutputArtifactCollector
{
    /// <summary>The selected plugin game release.</summary>
    private readonly GameRelease Release;

    /// <summary>The verified plugin output descriptor.</summary>
    private readonly PluginOutputPluginInput Output;

    /// <summary>Initializes a complete output artifact collector.</summary>
    /// <param name="release">The selected plugin game release.</param>
    /// <param name="output">The verified output descriptor.</param>
    internal PluginOutputArtifactCollector(GameRelease release, PluginOutputPluginInput output)
    {
        Release = release;
        Output = output;
    }

    /// <summary>Captures the output plugin plus every known sibling loose strings sidecar state.</summary>
    /// <param name="cancellationToken">The token checked during inventory and hashing.</param>
    /// <returns>The complete immutable artifact set in canonical path order.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    /// <exception cref="PluginSourceInputException">Thrown when an artifact cannot be observed safely.</exception>
    internal async Task<IReadOnlyList<PluginArtifactAssociation>> CaptureAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        PluginFileInspector.VerifyDirectory(Path.GetDirectoryName(Output.Path)!, "output plugin directory");
        var artifacts = new List<PluginArtifactAssociation>
        {
            await PluginFileInspector.InspectAsync(
                Output.Path,
                PluginArtifactRole.Plugin,
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
                throw new PluginSourceInputException(
                    EngineErrorCode.InvalidRequest,
                    $"The output localized-string path identifies a file instead of a directory: '{stringsDirectory}'.");
            }

            if (Directory.Exists(stringsDirectory))
            {
                PluginFileInspector.VerifyDirectory(stringsDirectory, "output localized-string directory");
            }

            foreach (var source in OrderedStringsSources)
            {
                foreach (var language in constants.Languages.OrderBy(language => language))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var fileName = StringsUtility.GetFileName(languageFormat, Output.ModKey, language, source);
                    var path = Path.GetFullPath(Path.Combine(stringsDirectory, fileName));
                    artifacts.Add(await PluginFileInspector.InspectAsync(
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

    /// <summary>Maps a record strings source to its physical sidecar role.</summary>
    /// <param name="source">The record strings source.</param>
    /// <returns>The corresponding artifact role.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown for an unknown record strings source.</exception>
    private static PluginArtifactRole MapRole(StringsSource source)
    {
        return source switch
        {
            StringsSource.Normal => PluginArtifactRole.Strings,
            StringsSource.DL => PluginArtifactRole.DlStrings,
            StringsSource.IL => PluginArtifactRole.IlStrings,
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };
    }

    /// <summary>Gets record strings sources in stable artifact order.</summary>
    private static IReadOnlyList<StringsSource> OrderedStringsSources { get; } = Array.AsReadOnly(
        new[] { StringsSource.Normal, StringsSource.DL, StringsSource.IL });
}

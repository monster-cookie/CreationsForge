using CreationsForge.Core.Engine.PluginInputs;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Core.Engine.PluginOutputs;

/// <summary>Completes a localized private staging set when plugin serialization emits no translated-string entries.</summary>
public static class StagedPluginStringTables
{
    /// <summary>
    /// Creates an empty selected-language strings table only when the private staging directory contains no plugin table for the output.
    /// A localized plugin with no translated fields still needs an explicit loose table for later archive-free plugin admission.
    /// Existing tables and their entries are never rewritten.
    /// </summary>
    /// <param name="release">The exact plugin release determining table names and supported languages.</param>
    /// <param name="modKey">The identity of the privately staged output plugin.</param>
    /// <param name="stringsDirectoryPath">The existing, caller-owned private staging strings directory after the plugin writer has been disposed.</param>
    /// <param name="recordTextLanguage">The explicit plugin language used for the empty table name.</param>
    /// <param name="cancellationToken">A token checked before discovering or creating an empty table.</param>
    /// <exception cref="ArgumentException">Thrown when a required path or record strings naming format is unavailable.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="recordTextLanguage"/> is undefined.</exception>
    /// <exception cref="IOException">Thrown when the private table cannot be created or flushed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested before table creation.</exception>
    public static void EnsureExplicitTable(
        GameRelease release,
        ModKey modKey,
        string stringsDirectoryPath,
        Language recordTextLanguage,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(stringsDirectoryPath);
        if (modKey.IsNull)
        {
            throw new ArgumentException("An empty strings table requires an explicit output plugin identity.", nameof(modKey));
        }

        if (!Enum.IsDefined(recordTextLanguage))
        {
            throw new ArgumentOutOfRangeException(nameof(recordTextLanguage));
        }

        cancellationToken.ThrowIfCancellationRequested();
        PluginFileInspector.VerifyDirectory(stringsDirectoryPath, "private staged strings directory");
        var constants = GameConstants.Get(release);
        var languageFormat = constants.StringsLanguageFormat
            ?? throw new ArgumentException("The plugin release does not define localized strings table names.", nameof(release));

        foreach (var source in new[] { StringsSource.Normal, StringsSource.DL, StringsSource.IL })
        {
            foreach (var language in constants.Languages)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var fileName = StringsUtility.GetFileName(languageFormat, modKey, language, source);
                if (File.Exists(Path.Combine(stringsDirectoryPath, fileName)))
                {
                    return;
                }
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        var emptyTablePath = Path.Combine(
            stringsDirectoryPath,
            StringsUtility.GetFileName(languageFormat, modKey, recordTextLanguage, StringsSource.Normal));
        using var stream = new FileStream(emptyTablePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        writer.Write(0U); // Entry count in the plugin little-endian strings-table header.
        writer.Write(0U); // Byte length of the record string-data section.
        writer.Flush();
        stream.Flush(flushToDisk: true);
    }
}

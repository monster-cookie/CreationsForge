using CreationsForge.Core.Engine.NativeInputs;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Core.Engine.NativeOutputs;

/// <summary>Completes a localized private staging set when native serialization emits no translated-string entries.</summary>
public static class NativeStagedStringTables
{
    /// <summary>
    /// Creates an empty English strings table only when the private staging directory contains no native table for the output.
    /// A localized plugin with no translated fields still needs an explicit loose table for later archive-free native admission.
    /// Existing tables and their entries are never rewritten.
    /// </summary>
    /// <param name="release">The exact native release determining table names and supported languages.</param>
    /// <param name="modKey">The identity of the privately staged output plugin.</param>
    /// <param name="stringsDirectoryPath">The existing, caller-owned private staging strings directory after the native writer has been disposed.</param>
    /// <param name="cancellationToken">A token checked before discovering or creating an empty table.</param>
    /// <exception cref="ArgumentException">Thrown when a required path or native strings naming format is unavailable.</exception>
    /// <exception cref="IOException">Thrown when the private table cannot be created or flushed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested before table creation.</exception>
    public static void EnsureExplicitTable(
        GameRelease release,
        ModKey modKey,
        string stringsDirectoryPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(stringsDirectoryPath);
        if (modKey.IsNull)
        {
            throw new ArgumentException("An empty strings table requires an explicit output plugin identity.", nameof(modKey));
        }

        cancellationToken.ThrowIfCancellationRequested();
        NativeFileInspector.VerifyDirectory(stringsDirectoryPath, "private staged strings directory");
        var constants = GameConstants.Get(release);
        var languageFormat = constants.StringsLanguageFormat
            ?? throw new ArgumentException("The native release does not define localized strings table names.", nameof(release));

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
            StringsUtility.GetFileName(languageFormat, modKey, Language.English, StringsSource.Normal));
        using var stream = new FileStream(emptyTablePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        writer.Write(0U); // Entry count in the native little-endian strings-table header.
        writer.Write(0U); // Byte length of the native string-data section.
        writer.Flush();
        stream.Flush(flushToDisk: true);
    }
}

using System.IO.Abstractions;
using CreationsForge.Core.Engine.NativeOutputs;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.NativeOutputs;

/// <summary>Verifies native parsing of empty localized staging tables and preservation of existing translations.</summary>
public sealed class NativeStagedStringTablesTests
{
    /// <summary>Verifies each supported game's native parser reads the created table as exactly zero entries.</summary>
    /// <param name="release">The native release whose table names and encoding are exercised.</param>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public void EnsureExplicitTable_WithoutTranslations_WritesNativelyReadableEmptyTable(GameRelease release)
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var modKey = ModKey.FromNameAndExtension("Empty.esm");
            NativeStagedStringTables.EnsureExplicitTable(release, modKey, directory.FullName, Language.French);
            var file = directory.EnumerateFiles().ShouldHaveSingleItem();
            var expectedName = StringsUtility.GetFileName(
                GameConstants.Get(release).StringsLanguageFormat!.Value,
                modKey,
                Language.French,
                StringsSource.Normal);
            file.Name.ShouldBe(expectedName);
            file.Length.ShouldBe(8L);
            var lookup = new StringsLookupOverlay(
                file.FullName,
                StringsSource.Normal,
                MutagenEncoding.GetEncoding(release, Language.French),
                new FileSystem());
            lookup.Count.ShouldBe(0);
            lookup.ShouldBeEmpty();
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }

    /// <summary>Verifies the completion helper never changes populated native tables or adds an empty table beside them.</summary>
    /// <param name="release">The native release whose populated English and French tables are exercised.</param>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public void EnsureExplicitTable_WithTranslations_PreservesEveryExistingByte(GameRelease release)
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var modKey = ModKey.FromNameAndExtension("Populated.esm");
            using (var writer = new StringsWriter(release, modKey, directory.FullName, new NativeFixtureEncodingProvider(), new FileSystem()))
            {
                writer.Register("English value", Language.English, StringsSource.Normal);
                writer.Register("French value", Language.French, StringsSource.Normal);
            }

            var before = directory.EnumerateFiles().ToDictionary(file => file.FullName, file => File.ReadAllBytes(file.FullName));
            before.Count.ShouldBe(6);
            NativeStagedStringTables.EnsureExplicitTable(release, modKey, directory.FullName, Language.English);
            directory.EnumerateFiles().Select(file => file.FullName).Order().ShouldBe(before.Keys.Order());
            foreach (var artifact in before)
            {
                File.ReadAllBytes(artifact.Key).ShouldBe(artifact.Value);
            }
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }

    /// <summary>Supplies the exact native encodings used by each supported game's controlled table fixtures.</summary>
    private sealed class NativeFixtureEncodingProvider : IMutagenEncodingProvider
    {
        /// <summary>Returns Mutagen's native encoding for the requested release and table language.</summary>
        /// <param name="release">The fixture's native game release.</param>
        /// <param name="language">The native strings-table language.</param>
        /// <returns>The encoding used by the corresponding native writer and reader.</returns>
        public IMutagenEncoding GetEncoding(GameRelease release, Language language)
        {
            return MutagenEncoding.GetEncoding(release, language);
        }
    }
}

using System.Reflection;
using Moq;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Cache;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Services;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class CacheServiceTests : IDisposable
{
    private readonly string CacheFilePath = Path.Combine(
        Path.GetTempPath(),
        "SFRecordCompareEngineTests",
        $"{Guid.NewGuid():N}.bin.gz");

    public void Dispose()
    {
        if (File.Exists(CacheFilePath))
        {
            File.Delete(CacheFilePath);
        }
    }

    [Fact]
    public void SaveToDiskAndLoadFromDisk_WhenCacheHasEntries_RestoresLookup()
    {
        var gameConfigurationStore = new Mock<IGameConfigurationStore>();
        var sut = new CacheService(gameConfigurationStore.Object, CacheFilePath);
        SetCache(sut, CreateCache("02E7C8:Starfield.esm", "ChargenPreset"));
        SetIsDirty(sut, true);

        sut.SaveToDisk();

        var loaded = new CacheService(gameConfigurationStore.Object, CacheFilePath);
        loaded.LoadFromDisk();

        loaded.ResolveReferenceDisplayValue("02E7C8:Starfield.esm").ShouldBe("ChargenPreset");
    }

    [Theory]
    [InlineData("02E7C8:Starfield.esm", "2E7C8:Starfield.esm")]
    [InlineData("2E7C8:Starfield.esm", "02E7C8:Starfield.esm")]
    [InlineData("0002E7C8:Starfield.esm", "2E7C8:Starfield.esm")]
    [InlineData("2E7C8:Starfield.esm", "0002E7C8:Starfield.esm")]
    [InlineData("2E7C8:Starfield.esm", "2E7C8:Starfield.esm<Starfield.IStarfieldMajorRecordGetter>")]
    [InlineData("2E7C8:Starfield.esm", "2E7C8:Starfield.esm <Starfield.IStarfieldMajorRecordGetter>")]
    public void ResolveReferenceDisplayValue_WhenLookupUsesEquivalentFormKeyVariant_ReturnsEditorId(
        string cachedFormKey,
        string lookupFormKey)
    {
        var gameConfigurationStore = new Mock<IGameConfigurationStore>();
        var sut = new CacheService(gameConfigurationStore.Object, CacheFilePath);
        SetCache(sut, CreateCache(cachedFormKey, "ChargenPreset"));

        var result = sut.ResolveReferenceDisplayValue(lookupFormKey);

        result.ShouldBe("ChargenPreset");
    }

    [Fact]
    public void IsPluginCacheValid_WhenFileMetadataMatches_ReturnsTrue()
    {
        var pluginFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.esm");
        File.WriteAllText(pluginFilePath, "plugin");
        try
        {
            var pluginFileInfo = new FileInfo(pluginFilePath);
            var pluginCache = new PersistedPluginCacheDTO
            {
                PluginName = "Example.esm",
                FilePath = pluginFileInfo.FullName,
                FileSize = pluginFileInfo.Length,
                LastWriteTimeUtc = pluginFileInfo.LastWriteTimeUtc
            };

            IsPluginCacheValid(pluginCache, pluginFileInfo).ShouldBeTrue();
        }
        finally
        {
            File.Delete(pluginFilePath);
        }
    }

    [Fact]
    public void IsPluginCacheValid_WhenFileMetadataDiffers_ReturnsFalse()
    {
        var pluginFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.esm");
        File.WriteAllText(pluginFilePath, "plugin");
        try
        {
            var pluginFileInfo = new FileInfo(pluginFilePath);
            var pluginCache = new PersistedPluginCacheDTO
            {
                PluginName = "Example.esm",
                FilePath = pluginFileInfo.FullName,
                FileSize = pluginFileInfo.Length + 1,
                LastWriteTimeUtc = pluginFileInfo.LastWriteTimeUtc
            };

            IsPluginCacheValid(pluginCache, pluginFileInfo).ShouldBeFalse();
        }
        finally
        {
            File.Delete(pluginFilePath);
        }
    }

    [Fact]
    public void AddRecordCacheEntry_WhenRecordHasFormKey_AddsReferenceEntry()
    {
        var pluginCache = new PersistedPluginCacheDTO
        {
            PluginName = "Starfield.esm"
        };
        var record = new TestCacheRecord
        {
            FormKey = "2E7C8:Starfield.esm<Starfield.IStarfieldMajorRecordGetter>",
            EditorID = "ChargenPreset"
        };

        AddRecordCacheEntry(pluginCache, "Npc", record);

        var entry = pluginCache.Entries.Single();
        entry.FormKey.ShouldBe("2E7C8:Starfield.esm");
        entry.PluginName.ShouldBe("Starfield.esm");
        entry.RecordType.ShouldBe("Npc");
        entry.EditorID.ShouldBe("ChargenPreset");
    }

    private static PersistedReferenceCacheDTO CreateCache(string formKey, string editorId)
    {
        return new PersistedReferenceCacheDTO
        {
            CacheVersion = 1,
            Game = "Starfield",
            Plugins =
            {
                new PersistedPluginCacheDTO
                {
                    PluginName = "Starfield.esm",
                    FilePath = "Starfield.esm",
                    Entries =
                    {
                        new RecordReferenceCacheEntryDTO
                        {
                            FormKey = formKey,
                            PluginName = "Starfield.esm",
                            RecordType = "Npc",
                            EditorID = editorId
                        }
                    }
                }
            }
        };
    }

    private static void SetCache(CacheService cacheService, PersistedReferenceCacheDTO cache)
    {
        var field = typeof(CacheService).GetField("Cache", BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();
        field.SetValue(cacheService, cache);

        var method = typeof(CacheService).GetMethod("RebuildLookup", BindingFlags.Instance | BindingFlags.NonPublic);
        method.ShouldNotBeNull();
        method.Invoke(cacheService, []);
    }

    private static void SetIsDirty(CacheService cacheService, bool isDirty)
    {
        var field = typeof(CacheService).GetField("IsDirty", BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();
        field.SetValue(cacheService, isDirty);
    }

    private static bool IsPluginCacheValid(PersistedPluginCacheDTO pluginCache, FileInfo pluginFileInfo)
    {
        var method = typeof(CacheService).GetMethod("IsPluginCacheValid", BindingFlags.Static | BindingFlags.NonPublic);
        method.ShouldNotBeNull();
        return (bool)method.Invoke(null, [pluginCache, pluginFileInfo])!;
    }

    private static void AddRecordCacheEntry(PersistedPluginCacheDTO pluginCache, string recordType, object record)
    {
        var method = typeof(CacheService).GetMethod("AddRecordCacheEntry", BindingFlags.Static | BindingFlags.NonPublic);
        method.ShouldNotBeNull();
        method.Invoke(null, [pluginCache, recordType, record]);
    }

    private class TestCacheRecord
    {
        public string? FormKey { get; set; }
        public string? EditorID { get; set; }
    }
}

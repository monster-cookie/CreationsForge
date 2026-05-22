using System.Collections;
using System.Reflection;
using Moq;
using Mutagen.Bethesda.Environments;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Records;
using SFRecordCompareEngine.Core.Services;
using SFRecordCompareEngine.Core.Services.Interfaces;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class PluginServiceTests
{
    [Fact]
    public void GetDatabases_WhenGameIsNotConfigured_ReturnsEmptyList()
    {
        var gameConfigurationStore = new Mock<IGameConfigurationStore>();
        gameConfigurationStore.SetupGet(store => store.Game).Returns(null as IGameEnvironment);
        var recordService = new Mock<IRecordService>();
        var sut = new PluginService(gameConfigurationStore.Object, recordService.Object);

        var result = sut.GetPlugins();

        result.ShouldBeEmpty();
    }

    [Fact]
    public void GetDatabases_WhenConfigurationStoreThrows_ReturnsEmptyList()
    {
        var gameConfigurationStore = new Mock<IGameConfigurationStore>();
        gameConfigurationStore.SetupGet(store => store.Game).Throws(new InvalidOperationException("Config failed."));
        var recordService = new Mock<IRecordService>();
        var sut = new PluginService(gameConfigurationStore.Object, recordService.Object);

        var result = sut.GetPlugins();

        result.ShouldBeEmpty();
    }

    [Fact]
    public void GetPluginHeader_WhenGameIsNotConfigured_ReturnsNull()
    {
        var gameConfigurationStore = new Mock<IGameConfigurationStore>();
        gameConfigurationStore.SetupGet(store => store.Game).Returns(null as IGameEnvironment);
        var recordService = new Mock<IRecordService>();
        var sut = new PluginService(gameConfigurationStore.Object, recordService.Object);

        var result = sut.GetPluginHeader("Example.esm");

        result.ShouldBeNull();
    }

    [Fact]
    public void GetRecordComparison_WhenFormKeyIsEmpty_ReturnsEmptyComparison()
    {
        var gameConfigurationStore = new Mock<IGameConfigurationStore>();
        var recordService = new Mock<IRecordService>();
        var sut = new PluginService(gameConfigurationStore.Object, recordService.Object);

        var result = sut.GetRecordComparison("Example.esm", "Npc", string.Empty);

        result.Plugins.ShouldBeEmpty();
        result.Fields.ShouldBeEmpty();
    }

    [Fact]
    public void GetRecordComparison_WhenGameIsNotConfigured_ReturnsEmptyComparison()
    {
        var gameConfigurationStore = new Mock<IGameConfigurationStore>();
        gameConfigurationStore.SetupGet(store => store.Game).Returns(null as IGameEnvironment);
        var recordService = new Mock<IRecordService>();
        var sut = new PluginService(gameConfigurationStore.Object, recordService.Object);

        var result = sut.GetRecordComparison("Example.esm", "Npc", "Example.esm|800");

        result.Plugins.ShouldBeEmpty();
        result.Fields.ShouldBeEmpty();
    }

    [Fact]
    public void FlattenRecordFields_WhenRecordTypeIsFormList_HidesConfiguredFields()
    {
        var record = CreateFormListRecord();

        var result = FlattenRecordFields(record, RecordComparisonRecordTypeOptions.For("FormList"));

        result.Contains("FormKey").ShouldBeFalse();
        result.Contains("FormVersion").ShouldBeFalse();
        result.Contains("StarfieldMajorRecordFlags").ShouldBeFalse();
        result.Contains("Version2").ShouldBeFalse();
        result.Contains("VersionControl").ShouldBeFalse();
        result.Contains("EditorID").ShouldBeTrue();
    }

    [Fact]
    public void FlattenRecordFields_WhenRecordTypeIsFormList_MarksItemsAsTree()
    {
        var record = CreateFormListRecord(25);

        var result = FlattenRecordFields(record, RecordComparisonRecordTypeOptions.For("FormList"));

        var items = result["Items"];
        items.ShouldNotBeNull();

        GetDisplayKind(items).ShouldBe(RecordComparisonFieldDisplayKind.Tree);
        var treeNodes = GetTreeNodes(items);
        treeNodes.Count.ShouldBe(25);
        treeNodes.ShouldNotContain(node => node.Name == "..." && node.Value == "Additional items not shown");
    }

    [Fact]
    public void FlattenRecordFields_WhenFieldIsBoolean_MarksFieldAsBoolean()
    {
        var record = CreateFormListRecord();

        var result = FlattenRecordFields(record, RecordComparisonRecordTypeOptions.For("FormList"));

        var enabled = result["Enabled"];
        enabled.ShouldNotBeNull();

        GetDisplayKind(enabled).ShouldBe(RecordComparisonFieldDisplayKind.Boolean);
        GetBooleanValue(enabled).ShouldBe(true);
    }

    [Fact(Skip = "Will need to be updated after the full conversion to SQLite is complete.")]
    public void FlattenRecordFields_WhenRecordTypeHasNoOptions_HidesDefaultFieldsAndKeepsFormKeyAsText()
    {
        var record = CreateFormListRecord();

        var result = FlattenRecordFields(record, RecordComparisonRecordTypeOptions.For("Keyword"));

        result.Contains("FormVersion").ShouldBeFalse();
        result.Contains("StarfieldMajorRecordFlags").ShouldBeFalse();
        result.Contains("Version2").ShouldBeFalse();
        result.Contains("VersionControl").ShouldBeFalse();
        result.Contains("FormKey").ShouldBeTrue();
        var formKey = result["FormKey"];
        formKey.ShouldNotBeNull();
        GetDisplayKind(formKey).ShouldBe(RecordComparisonFieldDisplayKind.Text);
    }

    [Fact(Skip = "Will need to be updated after the full conversion to SQLite is complete.")]
    public void FlattenRecordFields_WhenRecordTypeIsGameSetting_HidesDefaultFieldsAndXalg()
    {
        var record = new TestGameSettingRecord
        {
            FormKey = "Example.esm|900",
            FormVersion = 1,
            StarfieldMajorRecordFlags = "None",
            Version2 = 2,
            VersionControl = "Default",
            XALG = "Ignored",
            EditorID = "ExampleSetting"
        };

        var result = FlattenRecordFields(record, RecordComparisonRecordTypeOptions.For("GameSetting"));

        result.Contains("FormVersion").ShouldBeFalse();
        result.Contains("StarfieldMajorRecordFlags").ShouldBeFalse();
        result.Contains("Version2").ShouldBeFalse();
        result.Contains("VersionControl").ShouldBeFalse();
        result.Contains("XALG").ShouldBeFalse();
        result.Contains("FormKey").ShouldBeTrue();
        result.Contains("EditorID").ShouldBeTrue();
    }

    [Fact]
    public void FlattenRecordFields_WhenTextValueReferencesRecord_DisplaysEditorId()
    {
        var record = new TestReferencedRecord
        {
            ReferencedRecord = "Example.esm|801"
        };

        var result = FlattenRecordFields(
            record,
            RecordComparisonRecordTypeOptions.For("Keyword"),
            ResolveReferenceDisplayValue);

        var referencedRecord = result["ReferencedRecord"];
        referencedRecord.ShouldNotBeNull();
        GetTextValue(referencedRecord).ShouldBe("ResolvedItem");
    }

    [Fact]
    public void FlattenRecordFields_WhenTreeValueReferencesRecord_DisplaysEditorId()
    {
        var record = CreateFormListRecord();

        var result = FlattenRecordFields(
            record,
            RecordComparisonRecordTypeOptions.For("FormList"),
            ResolveReferenceDisplayValue);

        var items = result["Items"];
        items.ShouldNotBeNull();

        var formKeyNode = GetTreeNodes(items)[0].Children.Single(node => node.Name == "FormKey");
        formKeyNode.Value.ShouldBe("ResolvedItem");
    }

    [Theory]
    [InlineData("formid:Example.esm|801", "Example.esm|801")]
    [InlineData("2F7C8:Starfield.esm <Starfield.IStarfieldMajorRecordGetter>", "2F7C8:Starfield.esm")]
    [InlineData("2F7C8:Starfield.esm<Starfield.IStarfieldMajorRecordGetter>", "2F7C8:Starfield.esm")]
    [InlineData(" formid:2F7C8:Starfield.esm <Starfield.IStarfieldMajorRecordGetter> ", "2F7C8:Starfield.esm")]
    [InlineData(" formid:2F7C8:Starfield.esm<Starfield.IStarfieldMajorRecordGetter> ", "2F7C8:Starfield.esm")]
    public void NormalizeReferenceValue_WhenReferenceHasPrefixOrMutagenTypeSuffix_ReturnsLookupKey(
        string referenceValue,
        string expectedValue)
    {
        var result = FormKeyTextNormalizer.NormalizeReferenceValue(referenceValue);

        result.ShouldBe(expectedValue);
    }

    [Fact]
    public void ReferenceDisplayResolver_WhenRecordServiceResolvesReference_ReturnsResolvedDisplayValue()
    {
        var resolver = CreateReferenceDisplayResolver(
            referenceValue => referenceValue == "2F7C8:Starfield.esm" ? "ResolvedEditorId" : null);

        var result = ResolveReferenceDisplayValue(
            resolver,
            "2F7C8:Starfield.esm<Starfield.IStarfieldMajorRecordGetter>");

        result.ShouldBe("ResolvedEditorId");
    }

    [Fact]
    public void ReferenceDisplayResolver_WhenRecordServiceMisses_ReturnsNormalizedReference()
    {
        var resolver = CreateReferenceDisplayResolver(_ => null);

        var result = ResolveReferenceDisplayValue(
            resolver,
            "2F7C8:Starfield.esm<Starfield.IStarfieldMajorRecordGetter>");

        result.ShouldBe("2F7C8:Starfield.esm");
    }

    private static IDictionary FlattenRecordFields(
        object record,
        RecordComparisonRecordTypeOptions recordTypeOptions,
        Func<object?, string?>? displayValueResolver = null)
    {
        var method = typeof(PluginService).GetMethod(
            "FlattenRecordFields",
            BindingFlags.NonPublic | BindingFlags.Static);

        method.ShouldNotBeNull();
        return (IDictionary)method.Invoke(null, [record, recordTypeOptions, displayValueResolver])!;
    }

    private static object CreateReferenceDisplayResolver(Func<string, string?> directResolver)
    {
        var resolverType = typeof(PluginService).GetNestedType(
            "RecordReferenceDisplayResolver",
            BindingFlags.NonPublic);

        resolverType.ShouldNotBeNull();
        return Activator.CreateInstance(
            resolverType,
            [
                directResolver
            ])!;
    }

    private static string? ResolveReferenceDisplayValue(object resolver, object? value)
    {
        var method = resolver.GetType().GetMethod(
            "GetDisplayValue",
            BindingFlags.Instance | BindingFlags.Public);

        method.ShouldNotBeNull();
        return (string?)method.Invoke(resolver, [value]);
    }

    private static RecordComparisonFieldDisplayKind GetDisplayKind(object fieldValue)
    {
        return (RecordComparisonFieldDisplayKind)fieldValue.GetType()
            .GetProperty("DisplayKind")!
            .GetValue(fieldValue)!;
    }

    private static bool? GetBooleanValue(object fieldValue)
    {
        return (bool?)fieldValue.GetType()
            .GetProperty("BooleanValue")!
            .GetValue(fieldValue);
    }

    private static string? GetTextValue(object fieldValue)
    {
        return (string?)fieldValue.GetType()
            .GetProperty("TextValue")!
            .GetValue(fieldValue);
    }

    private static IList<RecordComparisonFieldNodeDTO> GetTreeNodes(object fieldValue)
    {
        return (IList<RecordComparisonFieldNodeDTO>)fieldValue.GetType()
            .GetProperty("TreeNodes")!
            .GetValue(fieldValue)!;
    }

    private static string? ResolveReferenceDisplayValue(object? value)
    {
        var rawValue = value?.ToString();
        return rawValue == "Example.esm|801"
            ? "ResolvedItem"
            : rawValue;
    }

    private static TestFormListRecord CreateFormListRecord(int itemCount = 2)
    {
        return new TestFormListRecord
        {
            FormKey = "Example.esm|800",
            FormVersion = 1,
            StarfieldMajorRecordFlags = "None",
            Version2 = 2,
            VersionControl = "Default",
            EditorID = "ExampleList",
            Enabled = true,
            Items = Enumerable.Range(1, itemCount)
                .Select(index => new TestFormListItem
                {
                    FormKey = $"Example.esm|{800 + index}",
                    Count = index
                })
                .ToList()
        };
    }

    private class TestFormListRecord
    {
        public string? FormKey { get; set; }
        public int FormVersion { get; set; }
        public string? StarfieldMajorRecordFlags { get; set; }
        public int Version2 { get; set; }
        public string? VersionControl { get; set; }
        public string? EditorID { get; set; }
        public bool Enabled { get; set; }
        public IList<TestFormListItem> Items { get; set; } = new List<TestFormListItem>();
    }

    private class TestFormListItem
    {
        public string? FormKey { get; set; }
        public int Count { get; set; }
    }

    private class TestReferencedRecord
    {
        public string? ReferencedRecord { get; set; }
    }

    private class TestGameSettingRecord
    {
        public string? FormKey { get; set; }
        public int FormVersion { get; set; }
        public string? StarfieldMajorRecordFlags { get; set; }
        public int Version2 { get; set; }
        public string? VersionControl { get; set; }
        public string? XALG { get; set; }
        public string? EditorID { get; set; }
    }
}

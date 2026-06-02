using Moq;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Services;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class ScriptingAdapterHydrationServiceTests
{
    [Fact]
    public void Hydrate_AttachesPropertiesAndListItemsToMatchingRecord()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var formKey = new FormKey(modKey, 123);
        var listObjectFormKey = new FormKey(new ModKey("Items", ModType.Plugin), 456);
        var record = new GlobalDTO
        {
            ModKey = modKey,
            FormKey = formKey,
            EditorID = "Editor",
            FormVersion = 1,
            StarfieldMajorRecordFlags = 0,
            Version2 = 0,
            VersionControl = 0,
            ImportedAtUTC = DateTime.UtcNow
        };
        var scriptingAdapterRepository = new Mock<IScriptingAdapterRepository>();
        scriptingAdapterRepository.Setup(x => x.GetByRecord(modKey, "Global", formKey)).Returns(new List<ScriptingAdapterDTO>
        {
            new()
            {
                ModKey = modKey,
                RecordType = "Global",
                FormKey = formKey,
                Name = "ExampleScript",
                ScriptIndex = 0,
                ImportedAtUTC = DateTime.UtcNow
            }
        });
        var scriptingAdapterPropertyRepository = new Mock<IScriptingAdapterPropertyRepository>();
        scriptingAdapterPropertyRepository.Setup(x => x.GetByRecord(modKey, "Global", formKey)).Returns(new List<ScriptingAdapterPropertyDTO>
        {
            new()
            {
                ModKey = modKey,
                RecordType = "Global",
                FormKey = formKey,
                ScriptingAdapterName = "ExampleScript",
                PropertyIndex = 0,
                Name = "Targets",
                MutagenObjectType = "ScriptObjectListProperty",
                ImportedAtUTC = DateTime.UtcNow
            }
        });
        var scriptingAdapterPropertyListItemRepository = new Mock<IScriptingAdapterPropertyListItemRepository>();
        scriptingAdapterPropertyListItemRepository.Setup(x => x.GetByRecord(modKey, "Global", formKey)).Returns(new List<ScriptingAdapterPropertyListItemDTO>
        {
            new()
            {
                ModKey = modKey,
                RecordType = "Global",
                FormKey = formKey,
                ScriptingAdapterName = "ExampleScript",
                PropertyIndex = 0,
                ListItemIndex = 0,
                MutagenObjectType = "ScriptObjectProperty",
                ObjectFormKey = listObjectFormKey,
                ImportedAtUTC = DateTime.UtcNow
            }
        });
        var sut = new ScriptingAdapterHydrationService(
            scriptingAdapterRepository.Object,
            scriptingAdapterPropertyRepository.Object,
            scriptingAdapterPropertyListItemRepository.Object);

        var result = sut.Hydrate(new List<GlobalDTO> { record }, "Global");

        result.Count.ShouldBe(1);
        result[0].ScriptingAdapters.Count.ShouldBe(1);
        result[0].ScriptingAdapters[0].Properties.Count.ShouldBe(1);
        result[0].ScriptingAdapters[0].Properties[0].ListItems.Count.ShouldBe(1);
        result[0].ScriptingAdapters[0].Properties[0].ListItems[0].ObjectFormKey.ShouldBe(listObjectFormKey);
    }
}

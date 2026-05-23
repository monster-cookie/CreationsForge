using Moq;
using Serilog;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Records;
using SFRecordCompareEngine.Core.Services.Interfaces;
using SFRecordCompareEngine.ViewModels;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.ViewModels;

public class MainWindowViewModelTests
{
    [Fact]
    public void Constructor_SetsInitialEmptyState()
    {
        var sut = new MainWindowViewModel(Mock.Of<IPluginService>(), Mock.Of<ILogger>());

        sut.LoadedGameText.ShouldBe("None");
        sut.LoadedPluginText.ShouldBe("None");
        sut.StatusText.ShouldBe("Use File > Open to choose a game and plugin.");
        sut.RecordTypeNodes.ShouldBeEmpty();
        sut.RecordsGridItems.ShouldBeNull();
        sut.IsComparisonMode.ShouldBeFalse();
    }

    [Fact]
    public void LoadPlugin_LoadsRecordTypeNodesAndUpdatesStatus()
    {
        var pluginService = new Mock<IPluginService>();
        pluginService.Setup(service => service.GetRecordTypes()).Returns(["FormList", "Keyword"]);
        pluginService.Setup(service => service.GetRecords("Example.esm", "FormList")).Returns(
            [
                new RecordSummaryDTO
                {
                    RecordType = "FormList",
                    FormID = "Example.esm|800",
                    EditorID = "ExampleList"
                }
            ]);
        pluginService.Setup(service => service.GetRecords("Example.esm", "Keyword")).Returns([]);
        var sut = new MainWindowViewModel(pluginService.Object, Mock.Of<ILogger>());

        sut.LoadPlugin("Starfield", "Example.esm");

        sut.LoadedGameText.ShouldBe("Starfield");
        sut.LoadedPluginText.ShouldBe("Example.esm");
        sut.RecordTypeNodes.Count.ShouldBe(1);
        sut.RecordTypeNodes[0].Name.ShouldBe("FormList");
        sut.StatusText.ShouldBe("Loaded 1 record type.");
        sut.IsComparisonMode.ShouldBeFalse();
    }

    [Fact]
    public void SelectRecordTreeItem_WhenRecordTypeSelected_ShowsSummaries()
    {
        var records = new List<RecordSummaryDTO>
        {
            new()
            {
                RecordType = "FormList",
                FormID = "Example.esm|800",
                EditorID = "ExampleList"
            }
        };
        var sut = new MainWindowViewModel(Mock.Of<IPluginService>(), Mock.Of<ILogger>());

        sut.SelectRecordTreeItem(new RecordTypeTreeNode
        {
            Name = "FormList",
            Records = records
        });

        sut.RecordsGridItems.ShouldBe(records);
        sut.StatusText.ShouldBe("Loaded 1 FormList records.");
        sut.IsComparisonMode.ShouldBeFalse();
    }

    [Fact]
    public void SelectRecordTreeItem_WhenRecordSelected_BuildsComparisonRows()
    {
        var pluginService = new Mock<IPluginService>();
        pluginService.Setup(service => service.GetRecordTypes()).Returns(["FormList"]);
        pluginService.Setup(service => service.GetRecords("Example.esm", "FormList")).Returns(
            [
                new RecordSummaryDTO
                {
                    RecordType = "FormList",
                    FormID = "Example.esm|800",
                    EditorID = "ExampleList"
                }
            ]);
        pluginService.Setup(service => service.GetRecordComparison("Example.esm", "FormList", "Example.esm|800"))
            .Returns(new RecordComparisonDTO
            {
                Plugins =
                [
                    new RecordComparisonPluginDTO
                    {
                        PluginName = "Starfield.esm"
                    },
                    new RecordComparisonPluginDTO
                    {
                        PluginName = "Example.esm"
                    }
                ],
                Fields =
                [
                    new RecordComparisonFieldDTO
                    {
                        FieldName = "EditorID",
                        DisplayKind = RecordComparisonFieldDisplayKind.Text,
                        ValuesByPlugin = new Dictionary<string, string?>
                        {
                            ["Starfield.esm"] = "BaseList",
                            ["Example.esm"] = "ExampleList"
                        }
                    }
                ]
            });
        var sut = new MainWindowViewModel(pluginService.Object, Mock.Of<ILogger>());
        sut.LoadPlugin("Starfield", "Example.esm");

        sut.SelectRecordTreeItem(new RecordSummaryDTO
        {
            RecordType = "FormList",
            FormID = "Example.esm|800",
            EditorID = "ExampleList"
        });

        sut.IsComparisonMode.ShouldBeTrue();
        sut.ComparisonPluginNames.ShouldBe(["Starfield.esm", "Example.esm"]);
        sut.RecordsGridItems.ShouldNotBeNull();
        var rows = sut.RecordsGridItems.ShouldBeAssignableTo<IList<RecordComparisonRowViewModel>>();
        rows.Count.ShouldBe(1);
        rows[0].FieldName.ShouldBe("EditorID");
        rows[0].Cells["Starfield.esm"].TextValue.ShouldBe("BaseList");
        rows[0].Cells["Example.esm"].TextValue.ShouldBe("ExampleList");
        sut.StatusText.ShouldBe("Loaded comparison for ExampleList.");
    }

    [Fact]
    public void SelectRecordTreeItem_WhenRecordCannotBeLoaded_ReportsStatus()
    {
        var sut = new MainWindowViewModel(Mock.Of<IPluginService>(), Mock.Of<ILogger>());

        sut.SelectRecordTreeItem(new RecordSummaryDTO
        {
            RecordType = "FormList",
            FormID = null,
            EditorID = "ExampleList"
        });

        sut.StatusText.ShouldBe("Unable to load comparison for the selected record.");
        sut.IsComparisonMode.ShouldBeFalse();
    }
}

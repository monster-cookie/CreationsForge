using System.Text.Json;
using CreationsForge.Services;
using CreationsForge.ViewModels;
using Shouldly;

namespace CreationsForge.PresentationTests.Services;

/// <summary>
/// Verifies native JSON projection retains every structural and scalar distinction required by the browser.
/// </summary>
public sealed class NativeJsonTreeProjectionServiceTests
{
    /// <summary>Verifies recursive projection retains property order, duplicate names, array positions, polymorphic tags, localized objects, nulls, and empty containers.</summary>
    [Fact]
    public void Project_CompleteNativeShape_PreservesExactOrderedHierarchy()
    {
        using var document = JsonDocument.Parse(
            """
            {
              "$type": "NativeComponent",
              "localized": { "English": "Alpha", "French": "Bêta" },
              "links": [
                { "link": "Master.esm:000123" },
                { "link": "Master.esm:000123" },
                { "link": null }
              ],
              "emptyArray": [],
              "emptyObject": {},
              "duplicate": 1,
              "duplicate": 2.50,
              "": "empty-name",
              "enabled": true,
              "missing": null
            }
            """);
        var service = new NativeJsonTreeProjectionService();

        var roots = service.Project(document.RootElement);

        var root = roots.ShouldHaveSingleItem();
        root.Name.ShouldBe("$");
        root.ValueKind.ShouldBe(JsonValueKind.Object);
        root.Children.Select(child => child.Name).ShouldBe(
        [
            "$type",
            "localized",
            "links",
            "emptyArray",
            "emptyObject",
            "duplicate",
            "duplicate",
            "",
            "enabled",
            "missing"
        ]);
        root.Children[0].ValueText.ShouldBe("\"NativeComponent\"");
        root.Children[1].Children.Select(child => child.Name).ShouldBe(["English", "French"]);
        root.Children[1].Children.Select(child => child.ValueText).ShouldBe(["\"Alpha\"", "\"Bêta\""]);
        root.Children[2].Children.Select(child => child.Name).ShouldBe(["[0]", "[1]", "[2]"]);
        root.Children[2].Children.Select(child => child.Children[0].ValueText).ShouldBe(
            ["\"Master.esm:000123\"", "\"Master.esm:000123\"", "null"]);
        root.Children[3].ValueText.ShouldBe("[]");
        root.Children[3].Children.ShouldBeEmpty();
        root.Children[4].ValueText.ShouldBe("{}");
        root.Children[4].Children.ShouldBeEmpty();
        root.Children[5].ValueText.ShouldBe("1");
        root.Children[6].ValueText.ShouldBe("2.50");
        root.Children[7].ValueText.ShouldBe("\"empty-name\"");
        root.Children[8].ValueText.ShouldBe("true");
        root.Children[9].ValueText.ShouldBe("null");
    }

    /// <summary>Verifies an unavailable native context projects to no synthetic field.</summary>
    [Fact]
    public void Project_MissingRecord_ReturnsEmptyTree()
    {
        new NativeJsonTreeProjectionService()
            .Project(record: null)
            .ShouldBeEmpty();
    }

    /// <summary>Verifies cancellation is observed during traversal of a record large enough to require repeated node checks.</summary>
    [Fact]
    public void Project_LargeRecord_CancellationStopsRecursiveProjection()
    {
        var json = "[" + string.Join(',', Enumerable.Range(0, 250_000)) + "]";
        using var document = JsonDocument.Parse(json);
        using var cancellation = new CancellationTokenSource();
        cancellation.CancelAfter(TimeSpan.FromMilliseconds(1));

        Should.Throw<OperationCanceledException>(() =>
            new NativeJsonTreeProjectionService().Project(document.RootElement, cancellation.Token));
    }
}

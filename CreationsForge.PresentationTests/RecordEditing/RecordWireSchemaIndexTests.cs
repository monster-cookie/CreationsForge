using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.RecordEditing.Schema;
using Shouldly;

namespace CreationsForge.PresentationTests.RecordEditing;

/// <summary>Verifies the real generated schema surface resolves into bounded typed presentation descriptors.</summary>
public sealed class RecordWireSchemaIndexTests
{
    /// <summary>Verifies all 722 generated concrete schemas resolve and retain the 719-to-3 complete-default split.</summary>
    [Fact]
    public void Resolve_AllGeneratedConcreteTypes_PreservesDefaultAvailability()
    {
        var context = FormListCatalogTests.CreateContexts()[0];
        var concrete = context.SchemaCatalog.Nodes
            .Where(key => key.Kind == RecordWireSchemaNodeKind.Type)
            .Select(key => context.SchemaCatalog.ReadNode(key).Value!)
            .Where(node => node.Schema.TryGetProperty("x-record-construction", out _))
            .ToArray();

        concrete.Length.ShouldBe(722);
        concrete.Count(node => node.DefaultTemplate.HasValue).ShouldBe(719);
        concrete.Where(node => !node.DefaultTemplate.HasValue).Select(node => node.Key.Name).ShouldBe(
        [
            "Mutagen.Bethesda.Starfield.ConditionFloat",
            "Mutagen.Bethesda.Starfield.ConditionGlobal",
            "Mutagen.Bethesda.Starfield.VolumesComponentItem",
        ],
        ignoreOrder: true);

        var index = new RecordWireSchemaIndex(context);
        foreach (var node in concrete)
        {
            var result = index.Resolve(node.Key, RecordWireReadLimits.Default);
            result.Succeeded.ShouldBeTrue(result.Error?.Message);
        }
    }

    /// <summary>Verifies large generated unions remain lightweight and do not materialize alternative graphs during indexing.</summary>
    [Fact]
    public void Resolve_LargeGeneratedUnions_IndexesExpectedAlternativesLazily()
    {
        var context = FormListCatalogTests.CreateContexts()[0];
        var index = new RecordWireSchemaIndex(context);
        var componentKey = context.SchemaCatalog.Nodes.Single(key => key.Kind == RecordWireSchemaNodeKind.Type && key.Name == "record.component");
        var conditionDataKey = context.SchemaCatalog.Nodes.Single(key => key.Kind == RecordWireSchemaNodeKind.Type && key.Name == "record.condition-data");

        var components = index.Resolve(componentKey, RecordWireReadLimits.Default).Value!;
        var conditionData = index.Resolve(conditionDataKey, RecordWireReadLimits.Default).Value!;

        components.Kind.ShouldBe(RecordWireSchemaValueKind.Union);
        components.UnionOptions.Count.ShouldBe(66);
        conditionData.Kind.ShouldBe(RecordWireSchemaValueKind.Union);
        conditionData.UnionOptions.Count.ShouldBe(608);
        conditionData.UnionOptions[0].DisplayName.ShouldNotBeNullOrWhiteSpace();
    }

    /// <summary>Verifies cancellation is observed before schema materialization.</summary>
    [Fact]
    public void Resolve_Canceled_ThrowsOperationCanceledException()
    {
        var context = FormListCatalogTests.CreateContexts()[0];
        var key = context.SchemaCatalog.Nodes[0];
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        Should.Throw<OperationCanceledException>(() => new RecordWireSchemaIndex(context).Resolve(key, RecordWireReadLimits.Default, cancellation.Token));
    }
}

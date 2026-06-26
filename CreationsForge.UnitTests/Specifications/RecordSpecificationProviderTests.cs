using CreationsForge.Specification.Records;
using Shouldly;

namespace CreationsForge.UnitTests.Specifications;

/// <summary>
/// Tests the injectable record specification provider registered by Core composition.
/// </summary>
public class RecordSpecificationProviderTests
{
    /// <summary>
    /// Verifies that the provider exposes the static catalog through the injectable service contract.
    /// </summary>
    [Fact]
    public void GetAll_ReturnsCatalogSpecifications()
    {
        var provider = new RecordSpecificationProvider();

        provider.GetAll().ShouldBeSameAs(RecordSpecificationCatalog.All);
    }

    /// <summary>
    /// Verifies that provider lookup delegates to the catalog's case-insensitive record identifier matching.
    /// </summary>
    [Fact]
    public void FindByRecordID_ReturnsMatchingSpecification()
    {
        var provider = new RecordSpecificationProvider();

        var specification = provider.FindByRecordID("gmst");

        specification.ShouldNotBeNull();
        specification.RecordID.ShouldBe("GMST");
    }

    /// <summary>
    /// Verifies that the provider can filter records by game support without callers using the static catalog.
    /// </summary>
    [Fact]
    public void GetSupportedByGame_ReturnsGameSupportedSpecifications()
    {
        var provider = new RecordSpecificationProvider();

        var specifications = provider.GetSupportedByGame(SpecificationGame.Fallout4);

        specifications.ShouldAllBe(specification =>
            specification.GameSupport.Any(support => support.Game == SpecificationGame.Fallout4));
    }
}

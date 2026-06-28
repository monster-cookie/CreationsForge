using CreationsForge.Specification.Games;
using CreationsForge.Specification.Records;
using Shouldly;

namespace CreationsForge.UnitTests.Specifications;

/// <summary>
/// Tests the specification-owned game metadata catalog.
/// </summary>
public class GameSpecificationCatalogTests
{
    /// <summary>
    /// Verifies that each supported specification game has exactly one catalog entry.
    /// </summary>
    [Fact]
    public void All_ContainsEachSpecificationGameOnce()
    {
        var games = GameSpecificationCatalog.All
            .Select(specification => specification.Game)
            .Order()
            .ToList();

        games.ShouldBe(Enum.GetValues<SpecificationGame>().Order().ToList());
    }

    /// <summary>
    /// Verifies that game display metadata is populated for diagnostics and boundary adapters.
    /// </summary>
    [Fact]
    public void All_ExposesNamesAndDisplayNames()
    {
        GameSpecificationCatalog.All.ShouldAllBe(specification => !string.IsNullOrWhiteSpace(specification.Name));
        GameSpecificationCatalog.All.ShouldAllBe(specification => !string.IsNullOrWhiteSpace(specification.DisplayName));
    }

    /// <summary>
    /// Verifies that lookup returns the matching game specification.
    /// </summary>
    [Fact]
    public void Find_ReturnsMatchingGameSpecification()
    {
        var specification = GameSpecificationCatalog.Find(SpecificationGame.Fallout4);

        specification.ShouldNotBeNull();
        specification.Game.ShouldBe(SpecificationGame.Fallout4);
    }
}

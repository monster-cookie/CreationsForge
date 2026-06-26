using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services;
using CreationsForge.Specification.Records;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

/// <summary>
/// Tests record-set assembly from specification reader metadata.
/// </summary>
public class RecordSetSpecificationBuilderTests
{
    /// <summary>
    /// Verifies that mapped record-family collections are assigned to the properties named by reader metadata.
    /// </summary>
    [Fact]
    public void Build_AssignsMappedCollectionsToSpecificationRecordSetProperties()
    {
        var formLists = new List<FormListDTO>();
        var globals = new List<GlobalDTO>();
        var builder = new RecordSetSpecificationBuilder(new TestRecordSpecificationProvider(
            CreateSpecification("FLST", "FormLists", SpecificationGame.Starfield),
            CreateSpecification("GLOB", "Globals", SpecificationGame.Starfield)));

        var recordSet = builder.Build(
            SupportedGame.Starfield,
            new Dictionary<string, object>
            {
                ["FLST"] = formLists,
                ["GLOB"] = globals
            });

        recordSet.FormLists.ShouldBeSameAs(formLists);
        recordSet.Globals.ShouldBeSameAs(globals);
    }

    /// <summary>
    /// Verifies that specifications for other games are not required when building a selected game's record set.
    /// </summary>
    [Fact]
    public void Build_LeavesUnsupportedGameFamiliesUnset()
    {
        var formLists = new List<FormListDTO>();
        var builder = new RecordSetSpecificationBuilder(new TestRecordSpecificationProvider(
            CreateSpecification("FLST", "FormLists", SpecificationGame.Starfield),
            CreateSpecification("CNDF", "ConditionForms", SpecificationGame.Fallout4)));

        var recordSet = builder.Build(
            SupportedGame.Starfield,
            new Dictionary<string, object>
            {
                ["FLST"] = formLists
            });

        recordSet.FormLists.ShouldBeSameAs(formLists);
        recordSet.ConditionForms.ShouldBeEmpty();
    }

    /// <summary>
    /// Verifies that missing mapped collections fail loudly for specifications supported by the selected game.
    /// </summary>
    [Fact]
    public void Build_WhenSupportedMappingIsMissing_Throws()
    {
        var builder = new RecordSetSpecificationBuilder(new TestRecordSpecificationProvider(
            CreateSpecification("FLST", "FormLists", SpecificationGame.Starfield)));

        var exception = Should.Throw<InvalidOperationException>(() =>
            builder.Build(SupportedGame.Starfield, new Dictionary<string, object>()));

        exception.Message.ShouldContain("FLST");
    }

    /// <summary>
    /// Verifies that mapped collection values must be assignable to the specification target property type.
    /// </summary>
    [Fact]
    public void Build_WhenMappedCollectionTypeIsIncompatible_Throws()
    {
        var builder = new RecordSetSpecificationBuilder(new TestRecordSpecificationProvider(
            CreateSpecification("FLST", "FormLists", SpecificationGame.Starfield)));

        var exception = Should.Throw<InvalidOperationException>(() =>
            builder.Build(
                SupportedGame.Starfield,
                new Dictionary<string, object>
                {
                    ["FLST"] = new List<GlobalDTO>()
                }));

        exception.Message.ShouldContain("FLST");
        exception.Message.ShouldContain("FormLists");
    }

    /// <summary>
    /// Creates a minimal record specification for record-set builder tests.
    /// </summary>
    /// <param name="recordID">The Bethesda record identifier used as the source mapping key.</param>
    /// <param name="pluginRecordSetPropertyName">The target <see cref="PluginRecordSetDTO"/> property name.</param>
    /// <param name="game">The game that supports the specification.</param>
    /// <returns>The test record specification.</returns>
    private static RecordSpecification CreateSpecification(
        string recordID,
        string pluginRecordSetPropertyName,
        SpecificationGame game)
    {
        return new RecordSpecification
        {
            RecordID = recordID,
            RecordType = recordID,
            TableName = pluginRecordSetPropertyName,
            FriendlyName = recordID,
            GameSupport =
            [
                new RecordGameSupportSpecification
                {
                    Game = game,
                    MutagenCollectionName = pluginRecordSetPropertyName,
                    SpriggitRecordDirectoryName = pluginRecordSetPropertyName
                }
            ],
            Import = new RecordImportSpecification
            {
                PluginRecordSetPropertyName = pluginRecordSetPropertyName
            },
            Reader = new RecordReaderSpecification
            {
                PluginRecordSetPropertyName = pluginRecordSetPropertyName,
                DefaultMutagenCollectionName = pluginRecordSetPropertyName
            }
        };
    }

    /// <summary>
    /// Provides isolated record specifications for builder tests.
    /// </summary>
    private sealed class TestRecordSpecificationProvider : IRecordSpecificationProvider
    {
        private readonly IReadOnlyList<RecordSpecification> Specifications;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRecordSpecificationProvider"/> class.
        /// </summary>
        /// <param name="specifications">The specifications exposed by the provider.</param>
        public TestRecordSpecificationProvider(params RecordSpecification[] specifications)
        {
            Specifications = specifications;
        }

        /// <inheritdoc />
        public IReadOnlyList<RecordSpecification> GetAll()
        {
            return Specifications;
        }

        /// <inheritdoc />
        public RecordSpecification? FindByRecordID(string recordID)
        {
            return Specifications.FirstOrDefault(specification =>
                string.Equals(specification.RecordID, recordID, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc />
        public IReadOnlyList<RecordSpecification> GetSupportedByGame(SpecificationGame game)
        {
            return Specifications
                .Where(specification => specification.GameSupport.Any(support => support.Game == game))
                .ToList();
        }
    }
}

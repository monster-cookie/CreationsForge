using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Specification.Records;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

/// <summary>
/// Contains record comparison scenarios for NPC records.
/// </summary>
public partial class RecordComparisonServiceTests
{
    /// <summary>
    /// Verifies that attributed NPC numeric fields use reduced precision for comparison display and state.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForNPC_UsesNumericDisplayPrecisionAttributes()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x1010);
        var npcRepository = new TestNPCRepository
        {
            Records =
            [
                CreateNPC("Base.esm", formKey, 1.2344, 1.2345),
                CreateNPC("Patch.esp", formKey, 1.23449, 1.2355)
            ]
        };
        var service = CreateService(npcRepository: npcRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.NPC.RecordID, formKey);

        var heightMin = comparison.Fields.Single(field => field.FieldName == "HeightMin");
        heightMin.Values.Select(value => value.DisplayValue).ShouldBe(["1.234", "1.234"]);
        heightMin.State.ShouldBe(RecordComparisonValueState.Identical);
        heightMin.Values.Select(value => value.State).ShouldBe([RecordComparisonValueState.Identical, RecordComparisonValueState.Identical]);

        var heightMax = comparison.Fields.Single(field => field.FieldName == "HeightMax");
        heightMax.Values.Select(value => value.DisplayValue).ShouldBe(["1.235", "1.236"]);
        heightMax.State.ShouldBe(RecordComparisonValueState.Conflict);
        heightMax.Values.Select(value => value.State).ShouldBe([RecordComparisonValueState.Conflict, RecordComparisonValueState.WinningOverride]);
    }

    /// <summary>
    /// Verifies that NPC top-level scalar parent rows and child rows are selected from the injected comparison
    /// specification.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForNPC_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x1011);
        var baseNpc = CreateNPC("Base.esm", formKey, 1, 1);
        var patchNpc = CreateNPC("Patch.esp", formKey, 1, 1);
        patchNpc.Aggression = "Aggressive";
        patchNpc.HeadParts.Add(CreateFormKey("Starfield.esm", 0x3E2B2));
        var npcRepository = new TestNPCRepository
        {
            Records =
            [
                baseNpc,
                patchNpc
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.NPC.RecordID,
                RecordType = SupportedRecordSpecifications.NPC.RecordType,
                TableName = SupportedRecordSpecifications.NPC.TableName,
                FriendlyName = SupportedRecordSpecifications.NPC.FriendlyName,
                GameSupport = SupportedRecordSpecifications.NPC.GameSupport,
                Fields = SupportedRecordSpecifications.NPC.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "Aggression",
                            SourcePath = "Aggression",
                            ValueKind = RecordFieldValueKind.Text
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(npcRepository: npcRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.NPC.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Aggression").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Unaggressive", "Aggressive"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "HeightMin");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "HeadParts");
    }

    /// <summary>
    /// Verifies that NPC localized scalar parent rows resolve through specification metadata and the selected record
    /// text language.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForNPC_UsesSpecificationLocalizedDisplay()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x1012);
        var baseNpc = CreateNPC("Base.esm", formKey, 1, 1);
        var patchNpc = CreateNPC("Patch.esp", formKey, 1, 1);
        baseNpc.Name = Text("Base NPC");
        baseNpc.ShortName = Text("Base Short");
        baseNpc.LongName = Text("Base Long");
        patchNpc.Name = Text("Patch NPC");
        patchNpc.ShortName = Text("Patch Short");
        patchNpc.LongName = Text("Patch Long");
        var npcRepository = new TestNPCRepository
        {
            Records =
            [
                baseNpc,
                patchNpc
            ]
        };
        var localizedStringRepository = new TestRecordLocalizedStringRepository
        {
            Records =
            [
                CreateLocalizedString("Base.esm", formKey, "Name", "German", "Basis NSC"),
                CreateLocalizedString("Patch.esp", formKey, "Name", "German", "Patch NSC"),
                CreateLocalizedString("Base.esm", formKey, "ShortName", "German", "Basis Kurz"),
                CreateLocalizedString("Patch.esp", formKey, "ShortName", "German", "Patch Kurz"),
                CreateLocalizedString("Base.esm", formKey, "LongName", "German", "Basis Lang"),
                CreateLocalizedString("Patch.esp", formKey, "LongName", "German", "Patch Lang")
            ]
        };
        var gameSelectionService = new TestGameSelectionService { RecordTextLanguage = Language.German };
        var service = CreateService(
            npcRepository: npcRepository,
            recordLocalizedStringRepository: localizedStringRepository,
            gameSelectionService: gameSelectionService);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.NPC.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis NSC", "Patch NSC"]);
        comparison.Fields.Single(field => field.FieldName == "ShortName").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Kurz", "Patch Kurz"]);
        comparison.Fields.Single(field => field.FieldName == "LongName").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Lang", "Patch Lang"]);
    }

    /// <summary>
    /// Verifies that NPC comparison output renders first-class persisted child rows instead of only scalar actor data.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForNPC_RendersPersistedChildRows()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x831);
        var baseNpc = CreateNPC("Base.esm", formKey, 1, 1);
        var patchNpc = CreateNPC("Patch.esp", formKey, 1, 1);
        patchNpc.Class = CreateFormKey("Starfield.esm", 0x20F487);
        patchNpc.DefaultOutfit = CreateFormKey("Starfield.esm", 0x102EC);
        patchNpc.Weight = new NPCWeightDTO { Thin = 0.54, Muscular = 0, Fat = 0 };
        patchNpc.HeadParts.Add(CreateFormKey("Starfield.esm", 0x3E2B2));
        patchNpc.FaceDialPositions.Add(new NPCFaceDialPositionDTO
        {
            FaceDialPositionIndex = 0,
            Index = 24,
            Position = -0.512
        });
        patchNpc.FaceMorphGroups.Add(new NPCFaceMorphGroupSetDTO
        {
            FaceMorphIndex = 0,
            Index = 12,
            MorphGroups =
            {
                new NPCFaceMorphGroupDTO
                {
                    FaceMorphIndex = 0,
                    MorphGroupIndex = 0,
                    MorphGroup = "Cheeks",
                    BlendIntensity = 1
                }
            }
        });
        patchNpc.MorphBlends.Add(new NPCMorphBlendDTO
        {
            MorphBlendIndex = 0,
            BlendName = "male_eu_md2_Cheeks",
            Intensity = 1
        });
        patchNpc.Tints.Add(new NPCTintDTO
        {
            TintIndex = 0,
            TintType = "Simple Group",
            TintGroup = "Dermaesthetic",
            TintName = "European_Male_Md2_Sk3",
            TintTexture = "textures/actors/human/faces/chargen/postblenddetails/dermaesthetic/male_eu_md2_sk3_derm_color.dds",
            TintColor = "Black",
            TintIntensity = 64
        });
        var npcRepository = new TestNPCRepository
        {
            Records =
            [
                baseNpc,
                patchNpc
            ]
        };
        var service = CreateService(npcRepository: npcRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.NPC.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Class").Values.Select(value => value.DisplayValue).ShouldBe(["", "Starfield.esm:0020F487"]);
        comparison.Fields.Single(field => field.FieldName == "DefaultOutfit").Values.Select(value => value.DisplayValue).ShouldBe(["", "Starfield.esm:000102EC"]);
        comparison.Fields.Single(field => field.FieldName == "Weight").Children.Single(field => field.FieldName == "Thin").Values.Select(value => value.DisplayValue).ShouldBe(["", "0.54"]);
        comparison.Fields.Single(field => field.FieldName == "HeadParts").Children.Single(field => field.FieldName == "HeadPart [0]").Values.Select(value => value.DisplayValue).ShouldBe(["", "Starfield.esm:0003E2B2"]);
        comparison.Fields.Single(field => field.FieldName == "FaceDialPositions").Children.Single(field => field.FieldName == "FaceDialPosition [0]").Children.Single(field => field.FieldName == "Position").Values.Select(value => value.DisplayValue).ShouldBe(["", "-0.512"]);
        comparison.Fields.Single(field => field.FieldName == "FaceMorphGroups").Children.Single(field => field.FieldName == "FaceMorph [0]").Children.Single(field => field.FieldName == "MorphGroup [0]").Children.Single(field => field.FieldName == "MorphGroup").Values.Select(value => value.DisplayValue).ShouldBe(["", "Cheeks"]);
        comparison.Fields.Single(field => field.FieldName == "MorphBlends").Children.Single(field => field.FieldName == "MorphBlend [0]").Children.Single(field => field.FieldName == "BlendName").Values.Select(value => value.DisplayValue).ShouldBe(["", "male_eu_md2_Cheeks"]);
        comparison.Fields.Single(field => field.FieldName == "Tints").Children.Single(field => field.FieldName == "Tint [0]").Children.Single(field => field.FieldName == "TintName").Values.Select(value => value.DisplayValue).ShouldBe(["", "European_Male_Md2_Sk3"]);
    }
}

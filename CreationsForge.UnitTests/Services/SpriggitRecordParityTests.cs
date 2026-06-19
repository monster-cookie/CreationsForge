using System.Globalization;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

[Collection("Spriggit Record Parity")]
[Trait("Category", "RequiresStarfield")]
public class SpriggitRecordParityTests
{
    private static readonly string[] KnownSoundSlots =
    [
        "Charge",
        "CloseSound",
        "CraftingSound",
        "DropdownSound",
        "OpenSound",
        "PickupSound",
        "PutdownSound",
        "Release"
    ];

    private readonly SpriggitRecordParityFixture fixture;

    public SpriggitRecordParityTests(SpriggitRecordParityFixture fixture)
    {
        this.fixture = fixture;
    }

    public static IEnumerable<object[]> SupportedRecordTypeCases()
    {
        yield return [SupportedGame.Fallout4, RecordTypeCatalog.ActorValueInformation.RecordID];
        yield return [SupportedGame.Fallout4, RecordTypeCatalog.Container.RecordID];
        yield return [SupportedGame.Fallout4, RecordTypeCatalog.ConstructibleObject.RecordID];
        yield return [SupportedGame.Fallout4, RecordTypeCatalog.FormList.RecordID];
        yield return [SupportedGame.Fallout4, RecordTypeCatalog.GameSetting.RecordID];
        yield return [SupportedGame.Fallout4, RecordTypeCatalog.Global.RecordID];
        yield return [SupportedGame.Fallout4, RecordTypeCatalog.Keyword.RecordID];
        yield return [SupportedGame.Fallout4, RecordTypeCatalog.MagicEffect.RecordID];
        yield return [SupportedGame.Fallout4, RecordTypeCatalog.MiscObject.RecordID];
        yield return [SupportedGame.Fallout4, RecordTypeCatalog.NPC.RecordID];
        yield return [SupportedGame.Fallout4, RecordTypeCatalog.Perk.RecordID];
        yield return [SupportedGame.Fallout4, RecordTypeCatalog.Static.RecordID];
        yield return [SupportedGame.Skyrim, RecordTypeCatalog.ActorValueInformation.RecordID];
        yield return [SupportedGame.Skyrim, RecordTypeCatalog.Container.RecordID];
        yield return [SupportedGame.Skyrim, RecordTypeCatalog.ConstructibleObject.RecordID];
        yield return [SupportedGame.Skyrim, RecordTypeCatalog.FormList.RecordID];
        yield return [SupportedGame.Skyrim, RecordTypeCatalog.GameSetting.RecordID];
        yield return [SupportedGame.Skyrim, RecordTypeCatalog.Global.RecordID];
        yield return [SupportedGame.Skyrim, RecordTypeCatalog.Keyword.RecordID];
        yield return [SupportedGame.Skyrim, RecordTypeCatalog.MagicEffect.RecordID];
        yield return [SupportedGame.Skyrim, RecordTypeCatalog.MiscObject.RecordID];
        yield return [SupportedGame.Skyrim, RecordTypeCatalog.NPC.RecordID];
        yield return [SupportedGame.Skyrim, RecordTypeCatalog.Perk.RecordID];
        yield return [SupportedGame.Skyrim, RecordTypeCatalog.Static.RecordID];
        yield return [SupportedGame.Starfield, RecordTypeCatalog.ActorValueInformation.RecordID];
        yield return [SupportedGame.Starfield, RecordTypeCatalog.Book.RecordID];
        yield return [SupportedGame.Starfield, RecordTypeCatalog.Container.RecordID];
        yield return [SupportedGame.Starfield, RecordTypeCatalog.ConditionForm.RecordID];
        yield return [SupportedGame.Starfield, RecordTypeCatalog.ConstructibleObject.RecordID];
        yield return [SupportedGame.Starfield, RecordTypeCatalog.Door.RecordID];
        yield return [SupportedGame.Starfield, RecordTypeCatalog.FormList.RecordID];
        yield return [SupportedGame.Starfield, RecordTypeCatalog.GameSetting.RecordID];
        yield return [SupportedGame.Starfield, RecordTypeCatalog.Global.RecordID];
        yield return [SupportedGame.Starfield, RecordTypeCatalog.Keyword.RecordID];
        yield return [SupportedGame.Starfield, RecordTypeCatalog.MagicEffect.RecordID];
        yield return [SupportedGame.Starfield, RecordTypeCatalog.MiscObject.RecordID];
        yield return [SupportedGame.Starfield, RecordTypeCatalog.NPC.RecordID];
        yield return [SupportedGame.Starfield, RecordTypeCatalog.Perk.RecordID];
        yield return [SupportedGame.Starfield, RecordTypeCatalog.Static.RecordID];
        yield return [SupportedGame.Starfield, RecordTypeCatalog.Terminal.RecordID];
    }

    [Theory]
    [MemberData(nameof(SupportedRecordTypeCases))]
    public void SupportedMajorRecordTypeSample_ShouldMapExpectedFieldsAndChildren(SupportedGame game, string recordType)
    {
        var sample = fixture.GetSample(game, GetFolderName(recordType), GetRequiredPaths(recordType));
        sample.TryGetFormKey(out var rawFormKey).ShouldBeTrue($"Spriggit sample '{sample.FilePath}' should contain a FormKey.");
        rawFormKey.ShouldNotBeNullOrWhiteSpace();

        var record = fixture.GetRecord(game, recordType, rawFormKey!);

        AssertCommonRecordFields(record, sample);
        AssertCommonChildMappings(record, sample);
        AssertRecordSpecificFields(recordType, record, sample);
    }

    private static void AssertCommonRecordFields(RecordDTO record, SpriggitYamlDocument sample)
    {
        if (sample.TryGetScalar("EditorID", out var editorId))
        {
            record.EditorID.ShouldBe(editorId, $"EditorID mismatch for sample '{sample.FilePath}'.");
        }

        if (sample.TryGetInt32("FormVersion", out var formVersion))
        {
            record.FormVersion.ShouldBe(formVersion, $"FormVersion mismatch for sample '{sample.FilePath}'.");
        }

        AssertNullableIntProperty(record, sample, "Version2", "Version2");
        AssertNullableStringProperty(record, sample, "ObjectBoundsFirst", "ObjectBounds.First");
        AssertNullableStringProperty(record, sample, "ObjectBoundsSecond", "ObjectBounds.Second");
        AssertLocalizedStringProperty(record, sample, "Name", "Name");
    }

    private static void AssertCommonChildMappings(RecordDTO record, SpriggitYamlDocument sample)
    {
        if (sample.TryGetScalar("Model.File", out var modelFile))
        {
            (record is IHasModelsRecordDTO).ShouldBeTrue($"Record '{record.EditorID}' should expose models.");
            var modelRecord = (IHasModelsRecordDTO)record;
            var normalizedExpectedModelFile = NormalizeSpriggitModelFilePath(modelFile);
            modelRecord.Models.Any(model => string.Equals(NormalizeMutagenModelFilePath(model.File), normalizedExpectedModelFile, StringComparison.OrdinalIgnoreCase))
                .ShouldBeTrue($"Record '{record.EditorID}' should contain model '{modelFile}'.");
        }

        var keywordCount = sample.GetListItemCount("Keywords");
        if (keywordCount > 0)
        {
            (record is IHasKeywordsRecordDTO).ShouldBeTrue($"Record '{record.EditorID}' should expose keywords.");
            var keywordRecord = (IHasKeywordsRecordDTO)record;
            keywordRecord.Keywords.Count.ShouldBeGreaterThanOrEqualTo(keywordCount, $"Record '{record.EditorID}' should preserve Spriggit keywords.");
        }

        var soundSlots = KnownSoundSlots
            .Where(soundSlot => sample.HasPath(soundSlot + ".Start"))
            .ToList();
        if (soundSlots.Count > 0)
        {
            (record is IHasSoundsRecordDTO).ShouldBeTrue($"Record '{record.EditorID}' should expose sounds.");
            var soundRecord = (IHasSoundsRecordDTO)record;
            foreach (var soundSlot in soundSlots)
            {
                soundRecord.Sounds.Any(sound => string.Equals(sound.SoundSlot, soundSlot, StringComparison.OrdinalIgnoreCase))
                    .ShouldBeTrue($"Record '{record.EditorID}' should preserve sound slot '{soundSlot}'.");
            }
        }

        if ((sample.HasPath("Model.Data") ||
             sample.HasPath("Components[].ANAM") ||
             sample.HasPath("Components[].BNAM") ||
             sample.HasPath("Components[].CNAM") ||
             sample.HasPath("Components[].REFL")) &&
            record is IHasRawRecordPayloadsRecordDTO)
        {
            var rawPayloadRecord = (IHasRawRecordPayloadsRecordDTO)record;
            rawPayloadRecord.RawPayloads.Count.ShouldBeGreaterThan(0, $"Record '{record.EditorID}' should preserve raw payloads.");
        }
    }

    private static void AssertRecordSpecificFields(string recordType, RecordDTO record, SpriggitYamlDocument sample)
    {
        switch (recordType)
        {
            case "AVIF":
                AssertNullableDoubleProperty(record, sample, "DefaultValue", "DefaultValue");
                AssertNullableDoubleProperty(record, sample, "Min", "Min");
                AssertNullableDoubleProperty(record, sample, "Max", "Max");
                AssertNullableStringProperty(record, sample, "Type", "Type");
                break;
            case "BOOK":
                AssertNullableIntProperty(record, sample, "Value", "Value");
                AssertNullableFloatProperty(record, sample, "Weight", "Weight");
                AssertNullableStringProperty(record, sample, "DataSlateType", "DataSlateType");
                AssertLocalizedStringPresence(record, sample, "Text", "Text");
                break;
            case "CONT":
                var containerRecord = record.ShouldBeOfType<ContainerDTO>();
                if (sample.GetListItemCount("Items") > 0)
                {
                    containerRecord.Items.Count.ShouldBe(sample.GetListItemCount("Items"), $"Container '{record.EditorID}' should preserve item counts.");
                }

                if (sample.HasPath("NativeTerminal"))
                {
                    containerRecord.NativeTerminalFormKey.ShouldNotBeNull($"Container '{record.EditorID}' should preserve NativeTerminal.");
                }

                break;
            case "CNDF":
                var conditionFormRecord = record.ShouldBeOfType<ConditionFormDTO>();
                if (sample.HasPath("Version2"))
                {
                    AssertNullableIntProperty(record, sample, "Version2", "Version2");
                }

                if (sample.HasPath("Conditions"))
                {
                    conditionFormRecord.Conditions.Count.ShouldBeGreaterThan(0, $"ConditionForm '{record.EditorID}' should preserve structured Conditions.");
                }

                break;
            case "COBJ":
                var constructibleObjectRecord = record.ShouldBeOfType<ConstructibleObjectDTO>();
                if (sample.HasPath("CreatedObject"))
                {
                    constructibleObjectRecord.CreatedObjectFormKey.ShouldNotBeNull($"ConstructibleObject '{record.EditorID}' should preserve CreatedObject.");
                }

                if (sample.HasPath("WorkbenchKeyword"))
                {
                    constructibleObjectRecord.WorkbenchKeywordFormKey.ShouldNotBeNull($"ConstructibleObject '{record.EditorID}' should preserve WorkbenchKeyword.");
                }

                var expectedComponentCount = sample.GetListItemCount("ConstructableComponents");
                if (expectedComponentCount == 0)
                {
                    expectedComponentCount = sample.GetListItemCount("Components");
                }

                if (expectedComponentCount == 0)
                {
                    expectedComponentCount = sample.GetListItemCount("Items");
                }

                if (expectedComponentCount > 0)
                {
                    constructibleObjectRecord.Components.Count.ShouldBe(expectedComponentCount, $"ConstructibleObject '{record.EditorID}' should preserve component counts.");
                }

                if (sample.GetListItemCount("RecipeFilters") > 0)
                {
                    constructibleObjectRecord.RecipeFilters.Count.ShouldBe(sample.GetListItemCount("RecipeFilters"), $"ConstructibleObject '{record.EditorID}' should preserve RecipeFilters.");
                }

                if (sample.GetListItemCount("Categories") > 0)
                {
                    constructibleObjectRecord.Categories.Count.ShouldBe(sample.GetListItemCount("Categories"), $"ConstructibleObject '{record.EditorID}' should preserve Categories.");
                }

                if (sample.HasPath("AmountProduced"))
                {
                    AssertNullableIntProperty(record, sample, "AmountProduced", "AmountProduced");
                }

                if (sample.HasPath("CreatedObjectCount"))
                {
                    AssertNullableIntProperty(record, sample, "CreatedObjectCount", "CreatedObjectCount");
                }

                if (sample.HasPath("Conditions"))
                {
                    constructibleObjectRecord.Conditions.Count.ShouldBeGreaterThan(0, $"ConstructibleObject '{record.EditorID}' should preserve structured Conditions.");
                }

                break;
            case "DOOR":
                AssertNullableStringProperty(record, sample, "SoundLevel", "SoundLevel");
                AssertNullableStringProperty(record, sample, "FacingAxisOverride", "FacingAxisOverride");
                if (sample.HasPath("NativeTerminal"))
                {
                    var doorRecord = record.ShouldBeOfType<DoorDTO>();
                    doorRecord.NativeTerminalFormKey.ShouldNotBeNull($"Door '{record.EditorID}' should preserve NativeTerminal.");
                }

                break;
            case "FLST":
                var formListRecord = record.ShouldBeOfType<FormListDTO>();
                if (sample.GetListItemCount("Items") > 0)
                {
                    formListRecord.Items.Count.ShouldBe(sample.GetListItemCount("Items"), $"FormList '{record.EditorID}' should preserve item counts.");
                }

                if (sample.TryGetScalar("AddToList", out var addToListValue) &&
                    !string.Equals(addToListValue?.Trim(), "Null", StringComparison.OrdinalIgnoreCase))
                {
                    formListRecord.AddToListFormKey.ShouldNotBeNull($"FormList '{record.EditorID}' should preserve AddToList.");
                }

                break;
            case "GMST":
                AssertGameSettingData(record.ShouldBeOfType<GameSettingDTO>(), sample);
                break;
            case "GLOB":
                AssertNullableDoubleProperty(record, sample, "Data", "Data");
                break;
            case "KYWD":
                AssertNullableStringProperty(record, sample, "Type", "Type");
                if (sample.HasPath("Color"))
                {
                    GetStringProperty(record, "Color").ShouldNotBeNullOrWhiteSpace($"Keyword '{record.EditorID}' should preserve Color.");
                }

                break;
            case "MGEF":
                AssertNullableStringProperty(record, sample, "CastType", "CastType");
                AssertNullableStringProperty(record, sample, "TargetType", "TargetType");
                if (sample.GetListItemCount("Flags") > 0)
                {
                    GetStringProperty(record, "Flags").ShouldNotBeNullOrWhiteSpace($"MagicEffect '{record.EditorID}' should preserve Flags.");
                }

                break;
            case "MISC":
                AssertNullableIntProperty(record, sample, "Value", "Value");
                AssertNullableFloatProperty(record, sample, "Weight", "Weight");
                break;
            case "NPC_":
                AssertNullableStringProperty(record, sample, "Aggression", "Aggression");
                AssertNullableStringProperty(record, sample, "Confidence", "Confidence");
                if (sample.HasPath("Race"))
                {
                    var npcRecord = record.ShouldBeOfType<NPCDTO>();
                    npcRecord.RaceFormKey.ShouldNotBeNull($"NPC '{record.EditorID}' should preserve Race.");
                }

                break;
            case "PERK":
                if (sample.GetListItemCount("Flags") > 0)
                {
                    GetStringProperty(record, "Flags").ShouldNotBeNullOrWhiteSpace($"Perk '{record.EditorID}' should preserve Flags.");
                }

                var perkRecord = record.ShouldBeOfType<PerkDTO>();
                if (sample.GetListItemCount("Ranks") > 0)
                {
                    perkRecord.Ranks.Count.ShouldBe(sample.GetListItemCount("Ranks"), $"Perk '{record.EditorID}' should preserve rank counts.");
                }

                break;
            case "STAT":
                AssertNullableDoubleProperty(record, sample, "MaxAngle", "MaxAngle");
                break;
            case "TERM":
                var terminalRecord = record.ShouldBeOfType<TerminalDTO>();
                if (sample.HasPath("Menu"))
                {
                    terminalRecord.MenuFormKey.ShouldNotBeNull($"Terminal '{record.EditorID}' should preserve Menu.");
                }

                if (sample.GetListItemCount("MarkerParameters") > 0)
                {
                    terminalRecord.MarkerParameters.Count.ShouldBe(
                        sample.GetListItemCount("MarkerParameters"),
                        $"Terminal '{record.EditorID}' should preserve marker parameter counts.");
                }

                break;
        }
    }

    private static void AssertGameSettingData(GameSettingDTO record, SpriggitYamlDocument sample)
    {
        sample.TryGetScalar("Data", out var expectedData).ShouldBeTrue($"GameSetting sample '{sample.FilePath}' should contain Data.");
        expectedData.ShouldNotBeNullOrWhiteSpace();

        if (bool.TryParse(expectedData, out var expectedBoolean))
        {
            record.BooleanData.ShouldBe(expectedBoolean, $"GameSetting '{record.EditorID}' should preserve boolean Data.");
            return;
        }

        if (int.TryParse(expectedData, NumberStyles.Integer, CultureInfo.InvariantCulture, out var expectedInteger))
        {
            record.IntegerData.ShouldBe(expectedInteger, $"GameSetting '{record.EditorID}' should preserve integer Data.");
            return;
        }

        if (double.TryParse(expectedData, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var expectedNumeric))
        {
            record.NumericData.ShouldNotBeNull($"GameSetting '{record.EditorID}' should preserve numeric Data.");
            Math.Abs(record.NumericData!.Value - expectedNumeric).ShouldBeLessThanOrEqualTo(
                0.0001,
                $"GameSetting '{record.EditorID}' should preserve numeric Data.");
            return;
        }

        record.Data.ShouldBe(expectedData, $"GameSetting '{record.EditorID}' should preserve string Data.");
    }

    private static void AssertLocalizedStringProperty(RecordDTO record, SpriggitYamlDocument sample, string propertyName, string path)
    {
        if (!sample.TryGetLocalizedEnglishString(path, out var expectedValue) || string.IsNullOrWhiteSpace(expectedValue))
        {
            return;
        }

        GetStringProperty(record, propertyName).ShouldBe(expectedValue, $"Record '{record.EditorID}' should preserve localized '{path}'.");
    }

    private static void AssertLocalizedStringPresence(RecordDTO record, SpriggitYamlDocument sample, string propertyName, string path)
    {
        if (!sample.TryGetLocalizedEnglishString(path, out var expectedValue) || string.IsNullOrWhiteSpace(expectedValue))
        {
            return;
        }

        GetStringProperty(record, propertyName).ShouldNotBeNullOrWhiteSpace($"Record '{record.EditorID}' should preserve localized '{path}'.");
    }

    private static void AssertNullableStringProperty(RecordDTO record, SpriggitYamlDocument sample, string propertyName, string path)
    {
        if (!sample.TryGetScalar(path, out var expectedValue))
        {
            return;
        }

        var actualValue = GetStringProperty(record, propertyName);
        if (actualValue == null && !HasProperty(record, propertyName))
        {
            return;
        }

        actualValue.ShouldBe(expectedValue, $"Record '{record.EditorID}' should preserve '{path}'.");
    }

    private static void AssertNullableIntProperty(RecordDTO record, SpriggitYamlDocument sample, string propertyName, string path)
    {
        if (!sample.TryGetInt32(path, out var expectedValue))
        {
            return;
        }

        var actualValue = GetNullableIntProperty(record, propertyName);
        if (actualValue == null && !HasProperty(record, propertyName))
        {
            return;
        }

        actualValue.ShouldBe(expectedValue, $"Record '{record.EditorID}' should preserve '{path}'.");
    }

    private static void AssertNullableFloatProperty(RecordDTO record, SpriggitYamlDocument sample, string propertyName, string path)
    {
        if (!sample.TryGetDouble(path, out var expectedValue))
        {
            return;
        }

        var actualValue = GetNullableFloatProperty(record, propertyName);
        if (actualValue == null && !HasProperty(record, propertyName))
        {
            return;
        }

        actualValue.ShouldNotBeNull($"Record '{record.EditorID}' should preserve '{path}'.");
        Math.Abs(actualValue!.Value - (float)expectedValue).ShouldBeLessThanOrEqualTo(
            0.0001f,
            $"Record '{record.EditorID}' should preserve '{path}'.");
    }

    private static void AssertNullableDoubleProperty(RecordDTO record, SpriggitYamlDocument sample, string propertyName, string path)
    {
        if (!sample.TryGetDouble(path, out var expectedValue))
        {
            return;
        }

        var actualValue = GetNullableDoubleProperty(record, propertyName);
        if (actualValue == null && !HasProperty(record, propertyName))
        {
            return;
        }

        actualValue.ShouldNotBeNull($"Record '{record.EditorID}' should preserve '{path}'.");
        Math.Abs(actualValue!.Value - expectedValue).ShouldBeLessThanOrEqualTo(
            0.0001,
            $"Record '{record.EditorID}' should preserve '{path}'.");
    }

    private static string GetFolderName(string recordType)
    {
        return recordType switch
        {
            "AVIF" => "ActorValueInformation",
            "BOOK" => "Books",
            "CONT" => "Containers",
            "CNDF" => "ConditionRecords",
            "COBJ" => "ConstructibleObjects",
            "DOOR" => "Doors",
            "FLST" => "FormLists",
            "GMST" => "GameSettings",
            "GLOB" => "Globals",
            "KYWD" => "Keywords",
            "MGEF" => "MagicEffects",
            "MISC" => "MiscItems",
            "NPC_" => "Npcs",
            "PERK" => "Perks",
            "STAT" => "Statics",
            "TERM" => "Terminals",
            _ => throw new InvalidOperationException($"Unsupported record type '{recordType}'.")
        };
    }

    private static IReadOnlyList<string> GetRequiredPaths(string recordType)
    {
        return recordType switch
        {
            "AVIF" => ["DefaultValue"],
            "BOOK" => ["Model.File", "Keywords"],
            "CONT" => ["Model.File", "Items"],
            "CNDF" => ["Conditions"],
            "COBJ" => ["CreatedObject"],
            "DOOR" => ["Model.File"],
            "FLST" => ["Items"],
            "GMST" => ["Data"],
            "GLOB" => ["Data"],
            "KYWD" => ["Name"],
            "MGEF" => ["Flags"],
            "MISC" => ["Model.File", "Value"],
            "NPC_" => ["Aggression"],
            "PERK" => ["Ranks"],
            "STAT" => ["Model.File"],
            "TERM" => ["Model.File"],
            _ => throw new InvalidOperationException($"Unsupported record type '{recordType}'.")
        };
    }

    private static string? GetStringProperty(object instance, string propertyName)
    {
        return (string?)instance.GetType().GetProperty(propertyName)?.GetValue(instance);
    }

    private static bool HasProperty(object instance, string propertyName)
    {
        return instance.GetType().GetProperty(propertyName) != null;
    }

    private static int? GetNullableIntProperty(object instance, string propertyName)
    {
        return (int?)instance.GetType().GetProperty(propertyName)?.GetValue(instance);
    }

    private static float? GetNullableFloatProperty(object instance, string propertyName)
    {
        return (float?)instance.GetType().GetProperty(propertyName)?.GetValue(instance);
    }

    private static double? GetNullableDoubleProperty(object instance, string propertyName)
    {
        return (double?)instance.GetType().GetProperty(propertyName)?.GetValue(instance);
    }

    private static string? NormalizeMutagenModelFilePath(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return filePath;
        }

        return filePath.StartsWith(@"Meshes\", StringComparison.OrdinalIgnoreCase)
            ? filePath["Meshes\\".Length..]
            : filePath.StartsWith("Meshes/", StringComparison.OrdinalIgnoreCase)
                ? filePath["Meshes/".Length..]
                : filePath
                    .Replace('/', '\\');
    }

    private static string? NormalizeSpriggitModelFilePath(string? filePath)
    {
        return string.IsNullOrWhiteSpace(filePath)
            ? filePath
            : filePath.Replace('/', '\\');
    }
}

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
/// Contains shared service setup and primitive DTO factory helpers for record comparison tests.
/// </summary>
public partial class RecordComparisonServiceTests
{
    private static RecordComparisonService CreateService(
        TestFormListRepository? formListRepository = null,
        TestGameSettingRepository? gameSettingRepository = null,
        TestGlobalRepository? globalRepository = null,
        TestClassRepository? classRepository = null,
        TestFactionRepository? factionRepository = null,
        TestMiscItemRepository? miscItemRepository = null,
        TestKeywordRepository? keywordRepository = null,
        TestActorValueInformationRepository? actorValueInformationRepository = null,
        TestNPCRepository? npcRepository = null,
        TestMagicEffectRepository? magicEffectRepository = null,
        TestPerkRepository? perkRepository = null,
        TestStaticRepository? staticRepository = null,
        TestBookRepository? bookRepository = null,
        TestDoorRepository? doorRepository = null,
        TestContainerRepository? containerRepository = null,
        TestConstructibleObjectRepository? constructibleObjectRepository = null,
        TestConditionFormRepository? conditionFormRepository = null,
        TestTerminalRepository? terminalRepository = null,
        TestModelRepository? modelRepository = null,
        TestKeywordMappingRepository? keywordMappingRepository = null,
        TestSoundMappingRepository? soundMappingRepository = null,
        TestScriptingAdapterRepository? scriptingAdapterRepository = null,
        TestReflectionRepository? reflectionRepository = null,
        TestRecordLocalizedStringRepository? recordLocalizedStringRepository = null,
        TestGameSelectionService? gameSelectionService = null,
        IRecordSpecificationProvider? recordSpecificationProvider = null)
    {
        return new RecordComparisonService(
            formListRepository ?? new TestFormListRepository(),
            gameSettingRepository ?? new TestGameSettingRepository(),
            globalRepository ?? new TestGlobalRepository(),
            classRepository ?? new TestClassRepository(),
            factionRepository ?? new TestFactionRepository(),
            miscItemRepository ?? new TestMiscItemRepository(),
            keywordRepository ?? new TestKeywordRepository(),
            actorValueInformationRepository ?? new TestActorValueInformationRepository(),
            npcRepository ?? new TestNPCRepository(),
            magicEffectRepository ?? new TestMagicEffectRepository(),
            perkRepository ?? new TestPerkRepository(),
            staticRepository ?? new TestStaticRepository(),
            bookRepository ?? new TestBookRepository(),
            doorRepository ?? new TestDoorRepository(),
            containerRepository ?? new TestContainerRepository(),
            constructibleObjectRepository ?? new TestConstructibleObjectRepository(),
            conditionFormRepository ?? new TestConditionFormRepository(),
            terminalRepository ?? new TestTerminalRepository(),
            modelRepository ?? new TestModelRepository(),
            keywordMappingRepository ?? new TestKeywordMappingRepository(),
            soundMappingRepository ?? new TestSoundMappingRepository(),
            scriptingAdapterRepository ?? new TestScriptingAdapterRepository(),
            reflectionRepository ?? new TestReflectionRepository(),
            recordLocalizedStringRepository ?? new TestRecordLocalizedStringRepository(),
            gameSelectionService ?? new TestGameSelectionService(),
            recordSpecificationProvider ?? new RecordSpecificationProvider());
    }

    private static FormKeyDTO CreateFormKey(string fileName, uint id)
    {
        return new FormKeyDTO
        {
            ModKey = CreateModKey(fileName),
            Id = id
        };
    }

    private static ModKeyDTO CreateModKey(string fileName)
    {
        return new ModKeyDTO
        {
            Name = Path.GetFileNameWithoutExtension(fileName),
            Type = 1,
            FileName = fileName
        };
    }

    private static TranslatedStringDTO Text(string value)
    {
        return new TranslatedStringDTO
        {
            Strings =
            [
                new TranslatedStringValueDTO
                {
                    Language = "English",
                    String = value
                }
            ]
        };
    }

    /// <summary>
    /// Creates a minimal NPC record for comparison-service tests that exercise height display precision.
    /// </summary>
    /// <param name="fileName">The plugin file name that contributed the test record.</param>
    /// <param name="formKey">The origin form key shared by compared records.</param>
    /// <param name="heightMin">The minimum height value to place on the DTO.</param>
    /// <param name="heightMax">The maximum height value to place on the DTO.</param>
    /// <returns>The populated NPC DTO.</returns>
}

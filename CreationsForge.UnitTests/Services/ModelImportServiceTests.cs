using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class ModelImportServiceTests
{
    [Fact]
    public void ReplaceRecordModels_DeletesExistingModelsAndSavesModelsAndMaterialSwapsWithRecordIdentity()
    {
        var modKey = CreateModKey("Test", "Test.esm");
        var formKey = new FormKeyDTO { ModKey = modKey, Id = 10 };
        var materialSwapFormKey = new FormKeyDTO { ModKey = CreateModKey("Materials", "Materials.esm"), Id = 20 };
        var record = new MiscItemDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = modKey,
            FormKey = formKey,
            EditorID = "MiscItem",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = new DateTime(2026, 6, 7, 12, 0, 0, DateTimeKind.Utc),
            Models =
            {
                new ModelDTO
                {
                    Game = SupportedGame.Fallout4,
                    ModKey = CreateModKey("Wrong", "Wrong.esm"),
                    RecordType = "Wrong",
                    FormKey = new FormKeyDTO { ModKey = modKey, Id = 99 },
                    ModelSlot = "Model",
                    ModelGender = string.Empty,
                    File = "Meshes/Test.nif",
                    ImportedAtUTC = default,
                    MaterialSwaps =
                    {
                        new ModelMaterialSwapDTO
                        {
                            Game = SupportedGame.Fallout4,
                            ModKey = CreateModKey("Wrong", "Wrong.esm"),
                            RecordType = "Wrong",
                            FormKey = new FormKeyDTO { ModKey = modKey, Id = 99 },
                            ModelSlot = "Wrong",
                            ModelGender = "Wrong",
                            MaterialSwapFormKey = materialSwapFormKey,
                            MaterialSwapIndex = 0,
                            ImportedAtUTC = default
                        }
                    }
                }
            }
        };
        var modelRepository = new TestModelRepository();
        var materialSwapRepository = new TestModelMaterialSwapRepository();
        var service = new ModelImportService(modelRepository, materialSwapRepository);

        service.ReplaceRecordModels(record, "MISC");

        modelRepository.DeleteRequests.ShouldBe([(SupportedGame.Starfield, modKey, "MISC", formKey)]);
        var savedModel = modelRepository.Saved.Single();
        savedModel.Game.ShouldBe(record.Game);
        savedModel.ModKey.ShouldBe(record.ModKey);
        savedModel.RecordType.ShouldBe("MISC");
        savedModel.FormKey.ShouldBe(record.FormKey);
        savedModel.ImportedAtUTC.ShouldBe(record.ImportedAtUTC);

        var savedMaterialSwap = materialSwapRepository.Saved.Single();
        savedMaterialSwap.Game.ShouldBe(record.Game);
        savedMaterialSwap.ModKey.ShouldBe(record.ModKey);
        savedMaterialSwap.RecordType.ShouldBe("MISC");
        savedMaterialSwap.FormKey.ShouldBe(record.FormKey);
        savedMaterialSwap.ModelSlot.ShouldBe(savedModel.ModelSlot);
        savedMaterialSwap.ModelGender.ShouldBe(savedModel.ModelGender);
        savedMaterialSwap.ImportedAtUTC.ShouldBe(record.ImportedAtUTC);
    }

    private static ModKeyDTO CreateModKey(string name, string fileName)
    {
        return new ModKeyDTO
        {
            Name = name,
            Type = 0,
            FileName = fileName
        };
    }

    private sealed class TestModelRepository : IModelRepository
    {
        public IList<(SupportedGame Game, ModKeyDTO ModKey, string RecordType, FormKeyDTO FormKey)> DeleteRequests { get; } = new List<(SupportedGame Game, ModKeyDTO ModKey, string RecordType, FormKeyDTO FormKey)>();

        public IList<ModelDTO> Saved { get; } = new List<ModelDTO>();

        public void Save(ModelDTO dto)
        {
            Saved.Add(dto);
        }

        public IReadOnlyList<ModelDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return [];
        }

        public void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey)
        {
            DeleteRequests.Add((game, modKey, recordType, formKey));
        }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestModelMaterialSwapRepository : IModelMaterialSwapRepository
    {
        public IList<ModelMaterialSwapDTO> Saved { get; } = new List<ModelMaterialSwapDTO>();

        public void Save(ModelMaterialSwapDTO dto)
        {
            Saved.Add(dto);
        }
    }
}

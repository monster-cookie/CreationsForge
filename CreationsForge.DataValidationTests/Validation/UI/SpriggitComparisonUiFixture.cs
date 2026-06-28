using Autofac;
using CreationsForge.Bootstrap.Composition;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.DataValidationTests.Validation.Services;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation;
using CreationsForge.DataValidationTests.Validation.Tests;

namespace CreationsForge.DataValidationTests.Validation.UI;

/// <summary>
/// Resolves imported records and production comparison services for Spriggit-backed headless UI validation.
/// </summary>
public class SpriggitComparisonUiFixture
{
    private static readonly Lazy<IContainer> Container = new(() => AutofacConfigurator.Configure());
    private readonly GameRecordSetProvider recordSetProvider = new();

    /// <summary>
    /// Creates a UI validation sample from the same spec data used by DTO validation tests.
    /// </summary>
    /// <param name="spec">The validation spec whose sample should be rendered.</param>
    /// <returns>A comparison UI sample backed by imported DTO readback and Spriggit sample data.</returns>
    public SpriggitComparisonUiSample CreateSample(ValidationSpec spec)
    {
        var record = recordSetProvider.GetRecord(spec.Game, spec.RecordType.RecordID, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);
        var game = SpecificationGameAdapter.ToSupportedGame(spec.Game);
        var plugin = CreatePlugin(game, record.ModKey.FileName);
        plugin.ModKey = record.ModKey;
        plugin.RecordCount = 1;

        return new SpriggitComparisonUiSample(
            game,
            spec.RecordType.RecordID,
            record,
            plugin,
            spriggit,
            Container.Value.Resolve<IRecordComparisonService>());
    }

    /// <summary>
    /// Creates a minimal imported-plugin DTO for configuring the headless main view.
    /// </summary>
    /// <param name="game">The game that owns the imported plugin.</param>
    /// <param name="fileName">The plugin filename to expose as the active comparison column.</param>
    /// <returns>A plugin DTO with the current import state expected by the view model.</returns>
    private static PluginDTO CreatePlugin(SupportedGame game, string fileName)
    {
        return new PluginDTO
        {
            Game = game,
            ModKey = new ModKeyDTO
            {
                Name = Path.GetFileNameWithoutExtension(fileName),
                FileName = fileName,
                Type = 0
            },
            LoadOrderIndex = 0,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current,
            HeaderFlags = 0,
            FormVersion = 0,
            RecordCount = 0,
            SourceLastWriteUTCTicks = 0,
            SourceFileSizeBytes = 0,
            LastCheckedUTC = DateTime.UtcNow
        };
    }
}

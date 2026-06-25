using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.DataValidationTests.Validation.Tests;

namespace CreationsForge.DataValidationTests.Validation.UI;

/// <summary>
/// Carries one imported DTO, its Spriggit sample, and the comparison service used for headless UI validation.
/// </summary>
public class SpriggitComparisonUiSample
{
    /// <summary>
    /// Initializes a UI validation sample for one Spriggit validation spec.
    /// </summary>
    /// <param name="game">The game that owns the sample.</param>
    /// <param name="recordType">The CreationsForge record type identifier rendered by the comparison UI.</param>
    /// <param name="record">The imported DTO read back from the configured validation database.</param>
    /// <param name="plugin">The active plugin represented in the headless main view.</param>
    /// <param name="spriggit">The flattened Spriggit sample data for diagnostic context.</param>
    /// <param name="comparisonService">The comparison service under test.</param>
    public SpriggitComparisonUiSample(
        SupportedGame game,
        string recordType,
        RecordDTO record,
        PluginDTO plugin,
        SpriggitRecordDTO spriggit,
        IRecordComparisonService comparisonService)
    {
        Game = game;
        RecordType = recordType;
        Record = record;
        Plugin = plugin;
        Spriggit = spriggit;
        ComparisonService = comparisonService;
    }

    /// <summary>
    /// Gets the game that owns the sample.
    /// </summary>
    public SupportedGame Game { get; }

    /// <summary>
    /// Gets the CreationsForge record type identifier rendered by the comparison UI.
    /// </summary>
    public string RecordType { get; }

    /// <summary>
    /// Gets the imported DTO read back from the configured validation database.
    /// </summary>
    public RecordDTO Record { get; }

    /// <summary>
    /// Gets the active plugin represented in the headless main view.
    /// </summary>
    public PluginDTO Plugin { get; }

    /// <summary>
    /// Gets the flattened Spriggit sample data used for diagnostic context.
    /// </summary>
    public SpriggitRecordDTO Spriggit { get; }

    /// <summary>
    /// Gets the comparison service used by the rendered main view.
    /// </summary>
    public IRecordComparisonService ComparisonService { get; }
}

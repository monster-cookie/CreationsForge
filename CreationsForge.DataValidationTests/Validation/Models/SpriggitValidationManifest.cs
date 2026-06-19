namespace CreationsForge.DataValidationTests.Validation.Models;

public class SpriggitValidationManifest
{
    public int SchemaVersion { get; set; } = 1;

    public string? GeneratedBy { get; set; }

    public DateTime? GeneratedAtUtc { get; set; }

    public IList<SpriggitValidationSample> Samples { get; set; } = new List<SpriggitValidationSample>();
}

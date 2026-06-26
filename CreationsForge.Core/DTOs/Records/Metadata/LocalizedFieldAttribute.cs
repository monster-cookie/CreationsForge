using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records.Metadata;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class LocalizedFieldAttribute : Attribute
{
    public LocalizedFieldAttribute(string sourceField)
    {
        SourceField = sourceField;
    }

    public LocalizedFieldAttribute(SupportedGame game, string sourceField)
    {
        Game = game;
        SourceField = sourceField;
    }

    public SupportedGame? Game { get; }

    public string SourceField { get; }

    public bool AppliesTo(SupportedGame? game)
    {
        return Game == null || game == null || Game == game;
    }
}

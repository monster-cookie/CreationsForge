using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records.Metadata;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class SpriggitPathAttribute : Attribute
{
    public SpriggitPathAttribute(string path)
    {
        Path = path;
    }

    public SpriggitPathAttribute(SupportedGame game, string path)
    {
        Game = game;
        Path = path;
    }

    public SupportedGame? Game { get; }

    public string Path { get; }

    public bool AppliesTo(SupportedGame? game)
    {
        return Game == null || game == null || Game == game;
    }
}

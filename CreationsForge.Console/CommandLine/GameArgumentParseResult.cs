using CreationsForge.Core.Enums;

namespace CreationsForge.Console.CommandLine;

public class GameArgumentParseResult
{
    private GameArgumentParseResult(bool isSuccess, SupportedGame? game, bool forceFullReimport, bool resetAll, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Game = game;
        ForceFullReimport = forceFullReimport;
        ResetAll = resetAll;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }

    public SupportedGame? Game { get; }

    public bool ForceFullReimport { get; }

    public bool ResetAll { get; }

    public string? ErrorMessage { get; }

    public static GameArgumentParseResult Success(SupportedGame game, bool forceFullReimport)
    {
        return new GameArgumentParseResult(true, game, forceFullReimport, false, null);
    }

    public static GameArgumentParseResult ResetAllSuccess()
    {
        return new GameArgumentParseResult(true, null, true, true, null);
    }

    public static GameArgumentParseResult Failure(string errorMessage)
    {
        return new GameArgumentParseResult(false, null, false, false, errorMessage);
    }
}

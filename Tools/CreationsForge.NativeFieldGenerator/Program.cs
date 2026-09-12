namespace CreationsForge.NativeFieldGenerator;

/// <summary>
/// Runs the guarded, explicitly invoked Starfield native-field source generator.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Generates the complete checked-in codec and manifest for one repository root.
    /// </summary>
    /// <param name="args">The <c>generate</c> command followed by the repository root path.</param>
    /// <returns>Zero after successful generation; otherwise one after writing the complete failure to standard error.</returns>
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 2 || !string.Equals(args[0], "generate", StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    "Usage: dotnet run --project Tools/CreationsForge.NativeFieldGenerator/CreationsForge.NativeFieldGenerator.csproj -- generate <repository-root>",
                    nameof(args));
            }

            new CodecEmitter().Generate(Path.GetFullPath(args[1]));
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            return 1;
        }
    }
}

namespace CreationsForge.RecordFieldGenerator;

/// <summary>
/// Runs the guarded, explicitly invoked Starfield Mutagen-field source generator.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Generates the checked-in codec or surveys major-record field coverage without writing files.
    /// </summary>
    /// <param name="args">The <c>generate</c> command and repository root, or the read-only <c>scan-major</c> command.</param>
    /// <returns>Zero after successful generation or survey; otherwise one after writing the failure to standard error.</returns>
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length == 1 && string.Equals(args[0], "scan-major", StringComparison.Ordinal))
            {
                new CodecEmitter().ScanMajorRecords(typeof(Mutagen.Bethesda.Starfield.StarfieldMajorRecord), validateModels: true);
                new CodecEmitter(typeof(Mutagen.Bethesda.Fallout4.Fallout4MajorRecord))
                    .ScanMajorRecords(typeof(Mutagen.Bethesda.Fallout4.Fallout4MajorRecord), validateModels: false);
                new CodecEmitter(typeof(Mutagen.Bethesda.Skyrim.SkyrimMajorRecord))
                    .ScanMajorRecords(typeof(Mutagen.Bethesda.Skyrim.SkyrimMajorRecord), validateModels: false);
                return 0;
            }

            if (args.Length == 2 && string.Equals(args[0], "generate-major", StringComparison.Ordinal))
            {
                new MajorRecordCoverageEmitter().Generate(Path.GetFullPath(args[1]));
                return 0;
            }

            if (args.Length != 2 || !string.Equals(args[0], "generate", StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    "Usage: dotnet run --project Tools/CreationsForge.RecordFieldGenerator/CreationsForge.RecordFieldGenerator.csproj -- generate <repository-root> | generate-major <repository-root> | scan-major",
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

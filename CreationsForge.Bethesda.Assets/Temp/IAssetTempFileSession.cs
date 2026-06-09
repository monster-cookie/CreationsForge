namespace CreationsForge.Bethesda.Assets.Temp;

public interface IAssetTempFileSession : IDisposable
{
    string RootDirectory { get; }

    string CreateExtractionDirectory(string scopeName);
}

using System.Text.Json;

namespace CreationsForge.UnitTests.Engine.ExternalSamples;

/// <summary>Verifies the documented closed manifest shape without opening plugins.</summary>
public sealed class ExternalPluginSampleManifestTests
{
    /// <summary>Initializes the stateless manifest contract test owner.</summary>
    public ExternalPluginSampleManifestTests()
    { }

    /// <summary>Accepts documented camelCase properties and rejects one unknown property.</summary>
    [Fact]
    public void LoadFromPath_UsesDocumentedClosedCamelCaseSchema()
    {
        var root = Directory.CreateTempSubdirectory();
        try
        {
            var sourceRoot = root.CreateSubdirectory("Source");
            var dataDirectory = sourceRoot.CreateSubdirectory("Data");
            var sourcePluginPath = Path.Combine(dataDirectory.FullName, "Contract.esm");
            File.WriteAllBytes(sourcePluginPath, []);
            var taskOutputRoot = Path.Combine(root.FullName, "TaskOutput");
            var manifestPath = Path.Combine(root.FullName, "manifest.json");
            var manifest = new
            {
                schemaVersion = 1,
                game = "Fallout4",
                release = "Fallout4",
                sourcePluginPath,
                loadOrderPluginPaths = new[] { sourcePluginPath },
                dataDirectoryPath = dataDirectory.FullName,
                stringDirectoryPaths = Array.Empty<string>(),
                selectedFormKeys = Array.Empty<string>(),
                taskOutputRoot,
                outputPluginFileName = "CFExternalContract.esm",
                outputModes = new[] { "Embedded" },
            };
            File.WriteAllText(manifestPath, JsonSerializer.Serialize(manifest));

            var loaded = ExternalPluginSampleManifest.LoadFromPath(manifestPath);

            Assert.Equal("Fallout4", loaded.Game);
            Assert.Equal("Fallout4", loaded.Release);
            Assert.Equal(sourcePluginPath, loaded.CanonicalSourcePluginPath);

            var invalidJson = JsonSerializer.Serialize(manifest).TrimEnd('}') + ",\"unknownProperty\":true}";
            File.WriteAllText(manifestPath, invalidJson);
            var error = Assert.Throws<InvalidDataException>(() => ExternalPluginSampleManifest.LoadFromPath(manifestPath));
            Assert.Contains("closed-schema JSON", error.Message, StringComparison.Ordinal);
        }
        finally
        {
            root.Delete(recursive: true);
        }
    }
}

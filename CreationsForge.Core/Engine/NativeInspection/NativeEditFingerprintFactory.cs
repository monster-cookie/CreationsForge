using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.NativeInspection;

/// <summary>Streams complete defensively copied native edit payloads into versioned canonical SHA-256 identities.</summary>
public static class NativeEditFingerprintFactory
{
    /// <summary>Writes one complete canonical payload without retaining its JSON representation.</summary>
    /// <param name="commandName">The stable non-empty typed command discriminator.</param>
    /// <param name="writePayload">The synchronous writer for every field of the defensively copied command payload.</param>
    /// <returns>The complete fingerprint and the first deterministic validation failure recorded while all fields were written.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="commandName"/> is empty or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="writePayload"/> is <see langword="null"/>.</exception>
    public static NativeEditFingerprintResult Create(
        string commandName,
        Action<Utf8JsonWriter, NativeJsonWriteContext> writePayload)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);
        ArgumentNullException.ThrowIfNull(writePayload);
        var context = new NativeJsonWriteContext(NativeJsonWriteMode.CanonicalFingerprintV1);
        using var algorithm = SHA256.Create();
        using var hashStream = new CryptoStream(Stream.Null, algorithm, CryptoStreamMode.Write, leaveOpen: true);
        using (var writer = new Utf8JsonWriter(hashStream, new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.Default,
            Indented = false,
            SkipValidation = false
        }))
        {
            writer.WriteStartObject();
            writer.WriteNumber("version", 1);
            writer.WritePropertyName("commandName");
            NativeJsonLeafWriter.WriteString(writer, commandName, context);
            writer.WritePropertyName("payload");
            writePayload(writer, context);
            writer.WriteEndObject();
            writer.Flush();
        }

        hashStream.FlushFinalBlock();
        return new NativeEditFingerprintResult(
            new OperationFingerprint(algorithm.Hash!),
            context.ValidationError);
    }
}

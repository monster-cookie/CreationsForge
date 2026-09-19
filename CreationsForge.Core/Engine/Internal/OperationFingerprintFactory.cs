using System.Security.Cryptography;
using System.Text;
using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Internal;

/// <summary>
/// Creates unambiguous versioned fingerprints and revision baseline identities from canonical Core-owned fields.
/// </summary>
internal sealed class OperationFingerprintFactory
{
    /// <summary>Identifies the lossless UTF-16 operation-envelope format.</summary>
    private const int FingerprintFormatVersion = 2;

    /// <summary>Creates the deterministic revision baseline for one source and selected output observation.</summary>
    /// <param name="sourceBaselineId">The immutable source-set baseline established while opening the workspace.</param>
    /// <param name="output">The selected canonical output association.</param>
    /// <param name="baseline">The complete selected output artifact-set observation.</param>
    /// <returns>A non-empty identifier derived from every supplied source and output identity field.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="sourceBaselineId"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> or <paramref name="baseline"/> is <see langword="null"/>.</exception>
    internal Guid CreateRevisionBaseline(
        Guid sourceBaselineId,
        OutputAssociation output,
        OutputArtifactSetBaseline baseline)
    {
        if (sourceBaselineId == Guid.Empty)
        {
            throw new ArgumentException("A workspace revision baseline requires a non-empty source baseline identifier.", nameof(sourceBaselineId));
        }

        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(baseline);
        var digest = Build(writer =>
        {
            WriteString(writer, "workspace-revision-baseline");
            writer.Write(sourceBaselineId.ToByteArray());
            WriteOutput(writer, output);
            WriteBaseline(writer, baseline);
        }).ToArray();
        var revisionBaselineId = new Guid(digest.AsSpan(0, 16));
        if (revisionBaselineId == Guid.Empty)
        {
            digest[0] = 1;
            revisionBaselineId = new Guid(digest.AsSpan(0, 16));
        }

        return revisionBaselineId;
    }

    /// <summary>Creates a fingerprint for output selection.</summary>
    /// <param name="workspaceId">The workspace in which the operation executes.</param>
    /// <param name="request">The canonical output-selection request.</param>
    /// <returns>The deterministic complete operation fingerprint.</returns>
    internal OperationFingerprint Create(Guid workspaceId, SelectOutputRequest request)
    {
        return Build(writer =>
        {
            WriteHeader(writer, "select-output", workspaceId, request.ExpectedRevision);
            writer.Write((int)request.Mode);
            WriteOutput(writer, request.Output);
        });
    }

    /// <summary>Creates a fingerprint for beginning a new or override edit.</summary>
    /// <param name="workspaceId">The workspace in which the operation executes.</param>
    /// <param name="request">The guarded begin-edit request.</param>
    /// <returns>The deterministic complete operation fingerprint.</returns>
    internal OperationFingerprint Create(Guid workspaceId, BeginEditRequest request)
    {
        return Build(writer =>
        {
            WriteHeader(writer, "begin-edit", workspaceId, request.ExpectedRevision);
            writer.Write((int)request.Role);
            WriteString(writer, request.RecordType);
            WriteNullableFormKey(writer, request.OriginFormKey);
            WriteNullableReferenceRequest(writer, request.OriginSelection);
            WriteNullableFormKey(writer, request.TargetFormKey);
        });
    }

    /// <summary>Creates a fingerprint that combines Core request context with an adapter-prepared engine payload.</summary>
    /// <param name="workspaceId">The workspace in which the operation executes.</param>
    /// <param name="request">The guarded typed-edit request.</param>
    /// <param name="preparedEdit">The immutable adapter-prepared payload and engine fingerprint.</param>
    /// <returns>The deterministic complete operation fingerprint.</returns>
    internal OperationFingerprint Create(
        Guid workspaceId,
        FormListEditRequest request,
        PreparedFormListEdit preparedEdit)
    {
        return Build(writer =>
        {
            WriteHeader(writer, "apply-form-list-edit", workspaceId, request.ExpectedRevision);
            writer.Write(request.EditId.ToByteArray());
            WriteString(writer, request.Edit.CommandName);
            WriteBytes(writer, preparedEdit.Fingerprint.ToArray());
        });
    }

    /// <summary>Fingerprints every immutable native GameSettingFloat value and the exact staged edit identity.</summary>
    /// <param name="workspaceId">The workspace in which the operation executes.</param>
    /// <param name="request">The complete guarded native field replacement.</param>
    /// <returns>The deterministic operation fingerprint, including nullable float bits.</returns>
    internal OperationFingerprint Create(Guid workspaceId, GameSettingFloatEditRequest request)
    {
        return Build(writer =>
        {
            WriteHeader(writer, "apply-game-setting-float-edit", workspaceId, request.ExpectedRevision);
            writer.Write(request.EditId.ToByteArray());
            WriteString(writer, request.EditorId);
            writer.Write(request.Data.HasValue);
            if (request.Data.HasValue)
            {
                writer.Write(BitConverter.SingleToInt32Bits(request.Data.Value));
            }

            writer.Write(request.MajorRecordFlagsRaw);
            writer.Write(request.FormVersion);
            writer.Write(request.Version2);
            writer.Write(request.VersionControl);
            writer.Write(request.Xalg.HasValue);
            if (request.Xalg.HasValue)
            {
                writer.Write(request.Xalg.Value);
            }
        });
    }

    /// <summary>Creates a fingerprint for reopening a selected output.</summary>
    /// <param name="workspaceId">The workspace in which the operation executes.</param>
    /// <param name="request">The guarded reopen request.</param>
    /// <returns>The deterministic complete operation fingerprint.</returns>
    internal OperationFingerprint Create(Guid workspaceId, ReopenOutputRequest request)
    {
        return Build(writer =>
        {
            WriteHeader(writer, "reopen-output", workspaceId, request.ExpectedRevision);
            WriteBaseline(writer, request.ExpectedBaseline);
        });
    }

    /// <summary>Creates a fingerprint for discarding staged output changes.</summary>
    /// <param name="workspaceId">The workspace in which the operation executes.</param>
    /// <param name="request">The guarded discard request.</param>
    /// <returns>The deterministic complete operation fingerprint.</returns>
    internal OperationFingerprint Create(Guid workspaceId, DiscardChangesRequest request)
    {
        return Build(writer =>
        {
            WriteHeader(writer, "discard-changes", workspaceId, request.ExpectedRevision);
            WriteBaseline(writer, request.ExpectedBaseline);
        });
    }

    /// <summary>Creates a fingerprint for a guarded save request.</summary>
    /// <param name="workspaceId">The workspace in which the operation executes.</param>
    /// <param name="request">The guarded save request.</param>
    /// <returns>The deterministic complete operation fingerprint.</returns>
    internal OperationFingerprint Create(Guid workspaceId, SaveRequest request)
    {
        return Build(writer =>
        {
            WriteHeader(writer, "save", workspaceId, request.ExpectedRevision);
            WriteBaseline(writer, request.ExpectedOutputBaseline);
        });
    }

    /// <summary>Creates a fingerprint for explicit terminal recovery adoption.</summary>
    /// <param name="workspaceId">The live workspace in which recovery adoption executes.</param>
    /// <param name="request">The guarded adoption request and complete caller-supplied evidence claims.</param>
    /// <returns>The deterministic complete operation fingerprint.</returns>
    internal OperationFingerprint Create(Guid workspaceId, ResolveOutputRecoveryRequest request)
    {
        return Build(writer =>
        {
            WriteHeader(writer, "resolve-output-recovery", workspaceId, request.ExpectedRevision);
            writer.Write((int)request.Mode);
            WriteResolvedEvidence(writer, request.Evidence);
        });
    }

    /// <summary>Builds one SHA-256 fingerprint from length-prefixed canonical values.</summary>
    /// <param name="write">The operation-specific canonical field writer.</param>
    /// <returns>The resulting SHA-256 operation fingerprint.</returns>
    private static OperationFingerprint Build(Action<BinaryWriter> write)
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
        {
            writer.Write(FingerprintFormatVersion);
            write(writer);
            writer.Flush();
        }

        return new OperationFingerprint(SHA256.HashData(stream.GetBuffer().AsSpan(0, checked((int)stream.Length))));
    }

    /// <summary>Writes common operation context.</summary>
    /// <param name="writer">The canonical binary writer.</param>
    /// <param name="operationKind">The stable versioned operation discriminator.</param>
    /// <param name="workspaceId">The workspace in which the operation executes.</param>
    /// <param name="revision">The exact expected workspace revision.</param>
    private static void WriteHeader(BinaryWriter writer, string operationKind, Guid workspaceId, WorkspaceRevision revision)
    {
        WriteString(writer, operationKind);
        writer.Write(workspaceId.ToByteArray());
        writer.Write(revision.BaselineId.ToByteArray());
        writer.Write(revision.Sequence);
    }

    /// <summary>Writes a complete output association.</summary>
    /// <param name="writer">The canonical binary writer.</param>
    /// <param name="output">The exact selected output identity and mode.</param>
    private static void WriteOutput(BinaryWriter writer, OutputAssociation output)
    {
        WritePath(writer, output.PluginPath);
        WriteString(writer, output.ModKey.Name.ToUpperInvariant());
        writer.Write((int)output.ModKey.Type);
        writer.Write((int)output.LocalizedOutputMode);
        writer.Write((int)output.MasterStyle);
    }

    /// <summary>Writes a complete output-set baseline, including expected absence.</summary>
    /// <param name="writer">The canonical binary writer.</param>
    /// <param name="baseline">The ordered complete artifact-set observation.</param>
    private static void WriteBaseline(BinaryWriter writer, OutputArtifactSetBaseline baseline)
    {
        writer.Write(baseline.BaselineId.ToByteArray());
        WriteArtifacts(writer, baseline.Artifacts);
    }

    /// <summary>Writes complete caller-supplied terminal recovery evidence claims.</summary>
    /// <param name="writer">The canonical binary writer.</param>
    /// <param name="evidence">The terminal recovery evidence.</param>
    private static void WriteResolvedEvidence(BinaryWriter writer, ResolvedOutputEvidence evidence)
    {
        WriteString(writer, evidence.EvidenceToken.Value);
        writer.Write((int)evidence.Game);
        writer.Write((int)evidence.Release);
        writer.Write(evidence.OriginalWorkspaceId.ToByteArray());
        writer.Write(evidence.SaveOperationId.ToByteArray());
        writer.Write(evidence.SaveBaseRevision.BaselineId.ToByteArray());
        writer.Write(evidence.SaveBaseRevision.Sequence);
        writer.Write(evidence.SourceBaseline.BaselineId.ToByteArray());
        WriteArtifacts(writer, evidence.SourceBaseline.Artifacts);
        WriteOutput(writer, evidence.Output);
        WriteBaseline(writer, evidence.ResolvedOutputBaseline);
        writer.Write((int)evidence.Status);
    }

    /// <summary>Writes every field of a complete ordered plugin artifact collection.</summary>
    /// <param name="writer">The canonical binary writer.</param>
    /// <param name="artifacts">The ordered artifacts to encode.</param>
    private static void WriteArtifacts(BinaryWriter writer, IReadOnlyList<PluginArtifactAssociation> artifacts)
    {
        writer.Write(artifacts.Count);

        foreach (var artifact in artifacts)
        {
            WritePath(writer, artifact.Path);
            writer.Write((int)artifact.Role);
            WriteNullableString(writer, artifact.Language?.ToUpperInvariant());
            writer.Write(artifact.Fingerprint.Exists);
            writer.Write(artifact.Fingerprint.Length);
            WriteNullableString(writer, artifact.Fingerprint.Sha256);
            writer.Write(artifact.FileIdentity is not null);
            if (artifact.FileIdentity is not null)
            {
                WriteString(writer, artifact.FileIdentity.Provider);
                WriteString(writer, artifact.FileIdentity.VolumeId);
                WriteString(writer, artifact.FileIdentity.FileId);
                writer.Write(artifact.FileIdentity.LinkCount.HasValue);
                if (artifact.FileIdentity.LinkCount.HasValue)
                {
                    writer.Write(artifact.FileIdentity.LinkCount.Value);
                }
            }
        }
    }

    /// <summary>Writes an optional engine FormKey without using lossy string conversion.</summary>
    /// <param name="writer">The canonical binary writer.</param>
    /// <param name="formKey">The optional record identity.</param>
    private static void WriteNullableFormKey(BinaryWriter writer, FormKey? formKey)
    {
        writer.Write(formKey.HasValue);
        if (!formKey.HasValue)
        {
            return;
        }

        WriteString(writer, formKey.Value.ModKey.Name.ToUpperInvariant());
        writer.Write((int)formKey.Value.ModKey.Type);
        writer.Write(formKey.Value.ID);
    }

    /// <summary>Writes every field of an optional record context selector.</summary>
    /// <param name="writer">The canonical binary writer.</param>
    /// <param name="request">The optional record context selector.</param>
    private static void WriteNullableReferenceRequest(BinaryWriter writer, ReferenceRequest? request)
    {
        writer.Write(request is not null);
        if (request is null)
        {
            return;
        }

        WriteNullableFormKey(writer, request.FormKey);
        writer.Write((int)request.Scope);
        writer.Write(request.ContainingModKey.HasValue);
        if (request.ContainingModKey.HasValue)
        {
            WriteString(writer, request.ContainingModKey.Value.Name.ToUpperInvariant());
            writer.Write((int)request.ContainingModKey.Value.Type);
        }
    }

    /// <summary>Writes a length-prefixed lossless sequence of UTF-16 code units.</summary>
    /// <param name="writer">The canonical binary writer.</param>
    /// <param name="value">The required string value.</param>
    private static void WriteString(BinaryWriter writer, string value)
    {
        writer.Write(value.Length);
        foreach (var codeUnit in value)
        {
            writer.Write((ushort)codeUnit);
        }
    }

    /// <summary>Writes a platform-canonical path while preserving case on case-sensitive systems.</summary>
    /// <param name="writer">The canonical binary writer.</param>
    /// <param name="value">The canonical absolute path.</param>
    private static void WritePath(BinaryWriter writer, string value)
    {
        WriteString(writer, OperatingSystem.IsWindows() ? value.ToUpperInvariant() : value);
    }

    /// <summary>Writes an optional length-prefixed lossless string while preserving null versus empty.</summary>
    /// <param name="writer">The canonical binary writer.</param>
    /// <param name="value">The optional string value.</param>
    private static void WriteNullableString(BinaryWriter writer, string? value)
    {
        writer.Write(value is not null);
        if (value is not null)
        {
            WriteString(writer, value);
        }
    }

    /// <summary>Writes a length-prefixed byte sequence.</summary>
    /// <param name="writer">The canonical binary writer.</param>
    /// <param name="bytes">The exact byte sequence.</param>
    private static void WriteBytes(BinaryWriter writer, ReadOnlySpan<byte> bytes)
    {
        writer.Write(bytes.Length);
        writer.Write(bytes);
    }
}

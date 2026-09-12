using System.Text;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Persistence;

/// <summary>Reads and writes the bounded versioned binary save-journal format.</summary>
internal static class SaveTransactionJournalCodec
{
    /// <summary>The fixed journal magic identifying CreationsForge save metadata.</summary>
    private const ulong Magic = 0x314C4E524A534643;

    /// <summary>The maximum number of artifacts accepted from one journal collection.</summary>
    private const int MaximumArtifactCount = 512;

    /// <summary>The maximum number of repair attempts retained by one save journal.</summary>
    private const int MaximumRepairCount = 64;

    /// <summary>The maximum UTF-8 byte length accepted for any one journal string.</summary>
    private const int MaximumStringBytes = 32768;

    /// <summary>Serializes one complete immutable journal snapshot.</summary>
    /// <param name="journal">The validated journal snapshot.</param>
    /// <returns>The complete versioned journal bytes.</returns>
    internal static byte[] Write(SaveTransactionJournal journal)
    {
        ArgumentNullException.ThrowIfNull(journal);
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
        writer.Write(Magic);
        writer.Write(SaveTransactionJournal.CurrentVersion);
        WriteGuid(writer, journal.WorkspaceId);
        WriteGuid(writer, journal.SaveOperationId);
        WriteString(writer, journal.RequestFingerprint);
        WriteRevision(writer, journal.SaveBaseRevision);
        writer.Write((int)journal.Game);
        writer.Write((int)journal.Release);
        WriteSourceBaseline(writer, journal.SourceBaseline);
        WriteOutput(writer, journal.Output);
        WriteOutputBaseline(writer, journal.BeforeBaseline);
        writer.Write((int)journal.Disposition);
        writer.Write((int)journal.Phase);
        writer.Write(journal.MutationProgress);
        WriteCount(writer, journal.ArtifactPlans.Count, MaximumArtifactCount, "artifact plan");
        foreach (var plan in journal.ArtifactPlans)
        {
            WriteAssociation(writer, plan.Before);
            WriteAssociation(writer, plan.Staged);
            WriteOptionalAssociation(writer, plan.Publish);
            WriteOptionalAssociation(writer, plan.Backup);
            WriteString(writer, plan.RetiredPath);
        }

        WriteOptionalOutputBaseline(writer, journal.TerminalBaseline);
        WriteCount(writer, journal.RepairAttempts.Count, MaximumRepairCount, "repair attempt");
        foreach (var attempt in journal.RepairAttempts)
        {
            WriteGuid(writer, attempt.OperationId);
            WriteString(writer, attempt.RequestFingerprint);
            writer.Write((int)attempt.Direction);
            writer.Write(attempt.Status.HasValue);
            if (attempt.Status.HasValue)
            {
                writer.Write((int)attempt.Status.Value);
            }

            WriteCount(writer, attempt.ArtifactPlans.Count, MaximumArtifactCount, "repair artifact plan");
            foreach (var plan in attempt.ArtifactPlans)
            {
                WriteAssociation(writer, plan.Current);
                WriteAssociation(writer, plan.Target);
                WriteOptionalAssociation(writer, plan.Publish);
                WriteOptionalAssociation(writer, plan.Retired);
            }
        }

        writer.Flush();
        return stream.ToArray();
    }

    /// <summary>Parses and validates one complete bounded journal snapshot.</summary>
    /// <param name="bytes">The bounded journal bytes.</param>
    /// <returns>The parsed immutable journal.</returns>
    /// <exception cref="InvalidDataException">Thrown when the journal is malformed, unsupported, incomplete, or internally inconsistent.</exception>
    internal static SaveTransactionJournal Read(ReadOnlyMemory<byte> bytes)
    {
        try
        {
            using var stream = new MemoryStream(bytes.ToArray(), writable: false);
            using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
            if (reader.ReadUInt64() != Magic)
            {
                throw new InvalidDataException("The save journal magic is invalid.");
            }

            var version = reader.ReadInt32();
            if (version != SaveTransactionJournal.CurrentVersion)
            {
                throw new InvalidDataException($"Save journal version {version} is unsupported.");
            }

            var workspaceId = ReadNonEmptyGuid(reader, "workspace");
            var saveOperationId = ReadNonEmptyGuid(reader, "save operation");
            var requestFingerprint = ReadDigest(reader, "save request fingerprint");
            var revision = ReadRevision(reader);
            var game = ReadEnum<SupportedGame>(reader, "game");
            var release = ReadEnum<GameRelease>(reader, "release");
            var sourceBaseline = ReadSourceBaseline(reader);
            var output = ReadOutput(reader);
            var beforeBaseline = ReadOutputBaseline(reader);
            var disposition = ReadEnum<NativeWriteDisposition>(reader, "write disposition");
            var phase = ReadEnum<SaveTransactionPhase>(reader, "transaction phase");
            var mutationProgress = reader.ReadInt32();
            var planCount = ReadCount(reader, MaximumArtifactCount, "artifact plan");
            var plans = new SaveArtifactPlan[planCount];
            for (var index = 0; index < planCount; index++)
            {
                plans[index] = new SaveArtifactPlan(
                    ReadAssociation(reader),
                    ReadAssociation(reader),
                    ReadOptionalAssociation(reader),
                    ReadOptionalAssociation(reader),
                    ReadAbsolutePath(reader, "retired artifact"));
            }

            var terminalBaseline = ReadOptionalOutputBaseline(reader);
            var repairCount = ReadCount(reader, MaximumRepairCount, "repair attempt");
            var attempts = new SaveRepairAttempt[repairCount];
            var repairIds = new HashSet<Guid>();
            for (var index = 0; index < repairCount; index++)
            {
                var operationId = ReadNonEmptyGuid(reader, "repair operation");
                if (!repairIds.Add(operationId))
                {
                    throw new InvalidDataException("The save journal contains duplicate repair operation identifiers.");
                }

                var fingerprint = ReadDigest(reader, "repair request fingerprint");
                var direction = ReadEnum<RepairSaveDirection>(reader, "repair direction");
                RepairSaveStatus? status = reader.ReadBoolean()
                    ? ReadEnum<RepairSaveStatus>(reader, "repair status")
                    : null;
                var repairPlanCount = ReadCount(reader, MaximumArtifactCount, "repair artifact plan");
                var repairPlans = new SaveRepairArtifactPlan[repairPlanCount];
                for (var planIndex = 0; planIndex < repairPlanCount; planIndex++)
                {
                    repairPlans[planIndex] = new SaveRepairArtifactPlan(
                        ReadAssociation(reader),
                        ReadAssociation(reader),
                        ReadOptionalAssociation(reader),
                        ReadOptionalAssociation(reader));
                }

                attempts[index] = new SaveRepairAttempt(operationId, fingerprint, direction, status, repairPlans);
            }

            if (stream.Position != stream.Length)
            {
                throw new InvalidDataException("The save journal contains trailing data.");
            }

            ValidateState(disposition, phase, mutationProgress, plans, terminalBaseline, attempts);
            return new SaveTransactionJournal(
                workspaceId,
                saveOperationId,
                requestFingerprint,
                revision,
                game,
                release,
                sourceBaseline,
                output,
                beforeBaseline,
                disposition,
                phase,
                mutationProgress,
                plans,
                terminalBaseline,
                attempts);
        }
        catch (InvalidDataException)
        {
            throw;
        }
        catch (Exception exception) when (exception is EndOfStreamException or IOException or ArgumentException or OverflowException)
        {
            throw new InvalidDataException("The save journal is malformed.", exception);
        }
    }

    /// <summary>Validates phase-dependent completeness after bounded parsing.</summary>
    /// <param name="disposition">The recorded native write disposition.</param>
    /// <param name="phase">The recorded durable phase.</param>
    /// <param name="mutationProgress">The recorded artifact progress.</param>
    /// <param name="plans">The complete artifact plans.</param>
    /// <param name="terminalBaseline">The optional terminal baseline.</param>
    /// <param name="repairAttempts">The bounded repair attempts and exact per-artifact plans.</param>
    /// <exception cref="InvalidDataException">Thrown when the phase and metadata disagree.</exception>
    private static void ValidateState(
        NativeWriteDisposition disposition,
        SaveTransactionPhase phase,
        int mutationProgress,
        IReadOnlyList<SaveArtifactPlan> plans,
        OutputArtifactSetBaseline? terminalBaseline,
        IReadOnlyList<SaveRepairAttempt> repairAttempts)
    {
        if (mutationProgress < 0 || mutationProgress > plans.Count)
        {
            throw new InvalidDataException("The save journal mutation progress is outside its artifact plan.");
        }

        if (disposition == NativeWriteDisposition.Unchanged && plans.Count != 0)
        {
            throw new InvalidDataException("A no-op save journal cannot contain artifact mutation plans.");
        }

        if (disposition == NativeWriteDisposition.StagedChanges
            && phase is (SaveTransactionPhase.Prepared or SaveTransactionPhase.MutationStarted or SaveTransactionPhase.Committed)
            && plans.Count == 0)
        {
            throw new InvalidDataException("A prepared changed save journal requires artifact mutation plans.");
        }

        var terminal = phase is SaveTransactionPhase.Committed or SaveTransactionPhase.NotCommitted;
        if (terminal != (terminalBaseline is not null))
        {
            throw new InvalidDataException("A terminal save journal requires exactly one resolved output baseline.");
        }

        var pathComparer = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        if (plans.Select(plan => plan.Before.Path).Distinct(pathComparer).Count() != plans.Count)
        {
            throw new InvalidDataException("The save journal contains duplicate destination artifact plans.");
        }

        foreach (var plan in plans)
        {
            if (plan.Before.Role != plan.Staged.Role
                || !string.Equals(plan.Before.Language, plan.Staged.Language, StringComparison.Ordinal))
            {
                throw new InvalidDataException("A staged artifact role or language does not match its destination plan.");
            }

            if (plan.Staged.Fingerprint.Exists != (plan.Publish is not null))
            {
                throw new InvalidDataException("A staged artifact and its publication-file presence disagree.");
            }

            if (plan.Before.Fingerprint.Exists != (plan.Backup is not null))
            {
                throw new InvalidDataException("A prior artifact and its backup-file presence disagree.");
            }
        }


        foreach (var attempt in repairAttempts)
        {
            if (attempt.ArtifactPlans.Count != plans.Count)
            {
                throw new InvalidDataException("A repair attempt must contain one exact plan for every destination artifact.");
            }

            for (var index = 0; index < attempt.ArtifactPlans.Count; index++)
            {
                var savePlan = plans[index];
                var repairPlan = attempt.ArtifactPlans[index];
                if (!pathComparer.Equals(savePlan.Before.Path, repairPlan.Current.Path)
                    || !pathComparer.Equals(savePlan.Before.Path, repairPlan.Target.Path)
                    || repairPlan.Current.Role != savePlan.Before.Role
                    || repairPlan.Target.Role != savePlan.Before.Role
                    || !string.Equals(repairPlan.Current.Language, savePlan.Before.Language, StringComparison.Ordinal)
                    || !string.Equals(repairPlan.Target.Language, savePlan.Before.Language, StringComparison.Ordinal))
                {
                    throw new InvalidDataException("A repair artifact plan does not match its destination artifact.");
                }

                if (repairPlan.Publish is not null && !repairPlan.Target.Fingerprint.Exists)
                {
                    throw new InvalidDataException("A repair publication file requires a present destination target.");
                }

                if (repairPlan.Retired is not null && repairPlan.Target.Fingerprint.Exists)
                {
                    throw new InvalidDataException("A repair retirement file requires an absent destination target.");
                }
            }
        }
    }

    /// <summary>Writes one workspace revision.</summary>
    /// <param name="writer">The journal writer.</param>
    /// <param name="revision">The revision to write.</param>
    private static void WriteRevision(BinaryWriter writer, WorkspaceRevision revision)
    {
        WriteGuid(writer, revision.BaselineId);
        writer.Write(revision.Sequence);
    }

    /// <summary>Reads one workspace revision.</summary>
    /// <param name="reader">The journal reader.</param>
    /// <returns>The validated revision.</returns>
    private static WorkspaceRevision ReadRevision(BinaryReader reader)
    {
        return new WorkspaceRevision(ReadNonEmptyGuid(reader, "revision baseline"), reader.ReadUInt64());
    }

    /// <summary>Writes one complete source baseline.</summary>
    /// <param name="writer">The journal writer.</param>
    /// <param name="baseline">The source baseline.</param>
    private static void WriteSourceBaseline(BinaryWriter writer, NativeSourceInputBaseline baseline)
    {
        WriteGuid(writer, baseline.BaselineId);
        WriteAssociations(writer, baseline.Artifacts);
    }

    /// <summary>Reads one complete source baseline.</summary>
    /// <param name="reader">The journal reader.</param>
    /// <returns>The parsed source baseline.</returns>
    private static NativeSourceInputBaseline ReadSourceBaseline(BinaryReader reader)
    {
        return new NativeSourceInputBaseline(
            ReadNonEmptyGuid(reader, "source baseline"),
            ReadAssociations(reader));
    }

    /// <summary>Writes one complete output baseline.</summary>
    /// <param name="writer">The journal writer.</param>
    /// <param name="baseline">The output baseline.</param>
    private static void WriteOutputBaseline(BinaryWriter writer, OutputArtifactSetBaseline baseline)
    {
        WriteGuid(writer, baseline.BaselineId);
        WriteAssociations(writer, baseline.Artifacts);
    }

    /// <summary>Reads one complete output baseline.</summary>
    /// <param name="reader">The journal reader.</param>
    /// <returns>The parsed output baseline.</returns>
    private static OutputArtifactSetBaseline ReadOutputBaseline(BinaryReader reader)
    {
        return new OutputArtifactSetBaseline(
            ReadNonEmptyGuid(reader, "output baseline"),
            ReadAssociations(reader));
    }

    /// <summary>Writes an optional complete output baseline.</summary>
    /// <param name="writer">The journal writer.</param>
    /// <param name="baseline">The optional output baseline.</param>
    private static void WriteOptionalOutputBaseline(BinaryWriter writer, OutputArtifactSetBaseline? baseline)
    {
        writer.Write(baseline is not null);
        if (baseline is not null)
        {
            WriteOutputBaseline(writer, baseline);
        }
    }

    /// <summary>Reads an optional complete output baseline.</summary>
    /// <param name="reader">The journal reader.</param>
    /// <returns>The optional parsed output baseline.</returns>
    private static OutputArtifactSetBaseline? ReadOptionalOutputBaseline(BinaryReader reader)
    {
        return reader.ReadBoolean() ? ReadOutputBaseline(reader) : null;
    }

    /// <summary>Writes the complete output association.</summary>
    /// <param name="writer">The journal writer.</param>
    /// <param name="output">The output association.</param>
    private static void WriteOutput(BinaryWriter writer, OutputAssociation output)
    {
        WriteString(writer, output.PluginPath);
        WriteString(writer, output.ModKey.FileName);
        writer.Write((int)output.LocalizedOutputMode);
        writer.Write((int)output.MasterStyle);
    }

    /// <summary>Reads and validates the complete output association.</summary>
    /// <param name="reader">The journal reader.</param>
    /// <returns>The parsed output association.</returns>
    private static OutputAssociation ReadOutput(BinaryReader reader)
    {
        var path = ReadAbsolutePath(reader, "output plugin");
        var modKeyText = ReadString(reader);
        if (!ModKey.TryFromNameAndExtension(modKeyText, out var modKey, out var error))
        {
            throw new InvalidDataException($"The save journal output ModKey is invalid: {error}");
        }

        return new OutputAssociation(
            path,
            modKey,
            ReadEnum<LocalizedOutputMode>(reader, "localized output mode"),
            ReadEnum<OutputMasterStyle>(reader, "output master style"));
    }

    /// <summary>Writes an ordered artifact association collection.</summary>
    /// <param name="writer">The journal writer.</param>
    /// <param name="artifacts">The artifacts to write.</param>
    private static void WriteAssociations(BinaryWriter writer, IReadOnlyList<NativeArtifactAssociation> artifacts)
    {
        WriteCount(writer, artifacts.Count, MaximumArtifactCount, "artifact");
        foreach (var artifact in artifacts)
        {
            WriteAssociation(writer, artifact);
        }
    }

    /// <summary>Reads an ordered artifact association collection.</summary>
    /// <param name="reader">The journal reader.</param>
    /// <returns>The parsed artifact collection.</returns>
    private static IReadOnlyList<NativeArtifactAssociation> ReadAssociations(BinaryReader reader)
    {
        var count = ReadCount(reader, MaximumArtifactCount, "artifact");
        var artifacts = new NativeArtifactAssociation[count];
        for (var index = 0; index < count; index++)
        {
            artifacts[index] = ReadAssociation(reader);
        }

        return Array.AsReadOnly(artifacts);
    }

    /// <summary>Writes one complete artifact association.</summary>
    /// <param name="writer">The journal writer.</param>
    /// <param name="artifact">The artifact to write.</param>
    private static void WriteAssociation(BinaryWriter writer, NativeArtifactAssociation artifact)
    {
        WriteString(writer, artifact.Path);
        writer.Write((int)artifact.Role);
        WriteOptionalString(writer, artifact.Language);
        writer.Write(artifact.Fingerprint.Exists);
        writer.Write(artifact.Fingerprint.Length);
        WriteOptionalString(writer, artifact.Fingerprint.Sha256);
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

    /// <summary>Reads one complete artifact association.</summary>
    /// <param name="reader">The journal reader.</param>
    /// <returns>The parsed artifact association.</returns>
    private static NativeArtifactAssociation ReadAssociation(BinaryReader reader)
    {
        var path = ReadAbsolutePath(reader, "artifact");
        var role = ReadEnum<NativeArtifactRole>(reader, "artifact role");
        var language = ReadOptionalString(reader);
        var exists = reader.ReadBoolean();
        var length = reader.ReadInt64();
        var digest = ReadOptionalString(reader);
        NativeFileIdentity? identity = null;
        if (reader.ReadBoolean())
        {
            var provider = ReadString(reader);
            var volume = ReadString(reader);
            var fileId = ReadString(reader);
            ulong? linkCount = reader.ReadBoolean() ? reader.ReadUInt64() : null;
            identity = new NativeFileIdentity(provider, volume, fileId, linkCount);
        }

        return new NativeArtifactAssociation(
            path,
            role,
            language,
            new NativeArtifactFingerprint(exists, length, digest),
            identity);
    }

    /// <summary>Writes an optional artifact association.</summary>
    /// <param name="writer">The journal writer.</param>
    /// <param name="artifact">The optional artifact.</param>
    private static void WriteOptionalAssociation(BinaryWriter writer, NativeArtifactAssociation? artifact)
    {
        writer.Write(artifact is not null);
        if (artifact is not null)
        {
            WriteAssociation(writer, artifact);
        }
    }

    /// <summary>Reads an optional artifact association.</summary>
    /// <param name="reader">The journal reader.</param>
    /// <returns>The optional parsed artifact.</returns>
    private static NativeArtifactAssociation? ReadOptionalAssociation(BinaryReader reader)
    {
        return reader.ReadBoolean() ? ReadAssociation(reader) : null;
    }

    /// <summary>Writes a bounded optional string.</summary>
    /// <param name="writer">The journal writer.</param>
    /// <param name="value">The optional value.</param>
    private static void WriteOptionalString(BinaryWriter writer, string? value)
    {
        writer.Write(value is not null);
        if (value is not null)
        {
            WriteString(writer, value);
        }
    }

    /// <summary>Reads a bounded optional string.</summary>
    /// <param name="reader">The journal reader.</param>
    /// <returns>The optional value.</returns>
    private static string? ReadOptionalString(BinaryReader reader)
    {
        return reader.ReadBoolean() ? ReadString(reader) : null;
    }

    /// <summary>Writes a length-delimited bounded UTF-8 string.</summary>
    /// <param name="writer">The journal writer.</param>
    /// <param name="value">The value to write.</param>
    private static void WriteString(BinaryWriter writer, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        if (bytes.Length > MaximumStringBytes)
        {
            throw new InvalidDataException("A save journal string exceeds the supported length.");
        }

        writer.Write(bytes.Length);
        writer.Write(bytes);
    }

    /// <summary>Reads a length-delimited bounded UTF-8 string.</summary>
    /// <param name="reader">The journal reader.</param>
    /// <returns>The decoded value.</returns>
    private static string ReadString(BinaryReader reader)
    {
        var length = reader.ReadInt32();
        if (length < 0 || length > MaximumStringBytes)
        {
            throw new InvalidDataException("A save journal string length is invalid.");
        }

        var bytes = reader.ReadBytes(length);
        if (bytes.Length != length)
        {
            throw new EndOfStreamException();
        }

        return new UTF8Encoding(false, true).GetString(bytes);
    }

    /// <summary>Reads a canonical absolute path.</summary>
    /// <param name="reader">The journal reader.</param>
    /// <param name="description">The path description used in failures.</param>
    /// <returns>The canonical absolute path.</returns>
    private static string ReadAbsolutePath(BinaryReader reader, string description)
    {
        var path = ReadString(reader);
        if (!Path.IsPathFullyQualified(path) || !string.Equals(path, Path.GetFullPath(path), PathComparison))
        {
            throw new InvalidDataException($"The save journal {description} path is not canonical and absolute.");
        }

        return path;
    }

    /// <summary>Writes a non-empty identifier.</summary>
    /// <param name="writer">The journal writer.</param>
    /// <param name="value">The identifier.</param>
    private static void WriteGuid(BinaryWriter writer, Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidDataException("A save journal identifier cannot be empty.");
        }

        writer.Write(value.ToByteArray());
    }

    /// <summary>Reads a non-empty identifier.</summary>
    /// <param name="reader">The journal reader.</param>
    /// <param name="description">The identifier description.</param>
    /// <returns>The identifier.</returns>
    private static Guid ReadNonEmptyGuid(BinaryReader reader, string description)
    {
        var bytes = reader.ReadBytes(16);
        if (bytes.Length != 16)
        {
            throw new EndOfStreamException();
        }

        var value = new Guid(bytes);
        if (value == Guid.Empty)
        {
            throw new InvalidDataException($"The save journal {description} identifier is empty.");
        }

        return value;
    }

    /// <summary>Writes a bounded collection count.</summary>
    /// <param name="writer">The journal writer.</param>
    /// <param name="count">The count to write.</param>
    /// <param name="maximum">The maximum accepted count.</param>
    /// <param name="description">The collection description.</param>
    private static void WriteCount(BinaryWriter writer, int count, int maximum, string description)
    {
        if (count < 0 || count > maximum)
        {
            throw new InvalidDataException($"The save journal {description} count exceeds its bound.");
        }

        writer.Write(count);
    }

    /// <summary>Reads a bounded collection count.</summary>
    /// <param name="reader">The journal reader.</param>
    /// <param name="maximum">The maximum accepted count.</param>
    /// <param name="description">The collection description.</param>
    /// <returns>The validated count.</returns>
    private static int ReadCount(BinaryReader reader, int maximum, string description)
    {
        var count = reader.ReadInt32();
        if (count < 0 || count > maximum)
        {
            throw new InvalidDataException($"The save journal {description} count is invalid.");
        }

        return count;
    }

    /// <summary>Reads and validates an enum value.</summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="reader">The journal reader.</param>
    /// <param name="description">The value description.</param>
    /// <returns>The defined enum value.</returns>
    private static T ReadEnum<T>(BinaryReader reader, string description)
        where T : struct, Enum
    {
        var value = (T)Enum.ToObject(typeof(T), reader.ReadInt32());
        if (!Enum.IsDefined(value))
        {
            throw new InvalidDataException($"The save journal {description} is undefined.");
        }

        return value;
    }

    /// <summary>Reads and validates a canonical SHA-256 digest string.</summary>
    /// <param name="reader">The journal reader.</param>
    /// <param name="description">The digest description.</param>
    /// <returns>The uppercase digest.</returns>
    private static string ReadDigest(BinaryReader reader, string description)
    {
        var digest = ReadString(reader);
        if (digest.Length != 64 || digest.Any(character => !Uri.IsHexDigit(character)))
        {
            throw new InvalidDataException($"The save journal {description} is invalid.");
        }

        return digest.ToUpperInvariant();
    }

    /// <summary>Gets platform path comparison semantics.</summary>
    private static StringComparison PathComparison => OperatingSystem.IsWindows()
        ? StringComparison.OrdinalIgnoreCase
        : StringComparison.Ordinal;
}

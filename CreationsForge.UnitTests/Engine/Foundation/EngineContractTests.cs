using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Foundation;

/// <summary>
/// Verifies immutable engine value contracts and complete plugin artifact baseline invariants.
/// </summary>
public sealed class EngineContractTests
{
    /// <summary>Verifies optional content digests are validated as SHA-256 hexadecimal values and stored canonically.</summary>
    [Fact]
    public void PluginArtifactFingerprint_WithDigest_ValidatesAndNormalizesHexadecimalValue()
    {
        var fingerprint = new PluginArtifactFingerprint(true, 12, new string('a', 64));

        fingerprint.Sha256.ShouldBe(new string('A', 64));
        new PluginArtifactFingerprint(true, 12, null).Sha256.ShouldBeNull();
        Should.Throw<ArgumentException>(() =>
            new PluginArtifactFingerprint(true, 12, new string('Z', 64)));
    }


    /// <summary>Verifies metadata-only fingerprints remain restricted to source observations.</summary>
    [Fact]
    public void OutputArtifactSetBaseline_WithPresentMetadataOnlyArtifact_RejectsValue()
    {
        var artifact = new PluginArtifactAssociation(
            Path.GetFullPath("Output.esp"),
            PluginArtifactRole.Plugin,
            null,
            new PluginArtifactFingerprint(true, 12, null),
            new ArtifactFileIdentity("test", "volume", "file", 1));

        Should.Throw<ArgumentException>(() =>
            new OutputArtifactSetBaseline(Guid.NewGuid(), [artifact]));
    }
    /// <summary>Verifies role, language, existence, and platform identity cannot describe an incoherent artifact.</summary>
    [Fact]
    public void PluginArtifactAssociation_WithIncoherentMetadata_RejectsValue()
    {
        var present = new PluginArtifactFingerprint(true, 0, new string('B', 64));
        var absent = new PluginArtifactFingerprint(false, 0, null);
        var pluginPath = Path.GetFullPath("Output.esp");
        var stringsPath = Path.GetFullPath("Output_en.STRINGS");

        Should.Throw<ArgumentException>(() =>
            new PluginArtifactAssociation(pluginPath, PluginArtifactRole.Plugin, "English", present));
        Should.Throw<ArgumentException>(() =>
            new PluginArtifactAssociation(stringsPath, PluginArtifactRole.Strings, null, present));
        Should.Throw<ArgumentException>(() =>
            new PluginArtifactAssociation(
                pluginPath,
                PluginArtifactRole.Plugin,
                null,
                absent,
                new ArtifactFileIdentity("test", "volume", "file", 1)));
    }

    /// <summary>Verifies an output path, record identity, and closed output modes must agree.</summary>
    [Fact]
    public void OutputAssociation_WithMismatchedIdentityOrUndefinedMode_RejectsValue()
    {
        Mutagen.Bethesda.Plugins.ModKey.TryFromNameAndExtension(
            "Different.esp",
            out var differentModKey,
            out var parsingError).ShouldBeTrue(parsingError);

        Should.Throw<ArgumentException>(() => new OutputAssociation(
            Path.GetFullPath("Output.esp"),
            differentModKey,
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full));
        Should.Throw<ArgumentOutOfRangeException>(() => new OutputAssociation(
            Path.GetFullPath("Different.esp"),
            differentModKey,
            (LocalizedOutputMode)int.MaxValue,
            OutputMasterStyle.Full));
    }

    /// <summary>Verifies output selection requires an explicit supported create-or-open mode.</summary>
    [Fact]
    public void SelectOutputRequest_WithUndefinedMode_RejectsValue()
    {
        var output = new OutputAssociation(
            Path.GetFullPath("Output.esp"),
            ModKey.FromNameAndExtension("Output.esp"),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);

        Should.Throw<ArgumentOutOfRangeException>(() => new SelectOutputRequest(
            Guid.NewGuid(),
            new WorkspaceRevision(Guid.NewGuid(), 0),
            (OutputSelectionMode)int.MaxValue,
            output));
    }

    /// <summary>Verifies begin-edit roles accept only their matching source origin or existing-output target identities.</summary>
    [Fact]
    public void BeginEditRequest_WithIncoherentRoleIdentities_RejectsValue()
    {
        var revision = new WorkspaceRevision(Guid.NewGuid(), 0);
        var origin = new FormKey(ModKey.FromNameAndExtension("Source.esp"), 0x800);
        var other = new FormKey(origin.ModKey, 0x801);
        var sourceSelection = new ReferenceRequest(origin, RecordScope.Source, origin.ModKey);
        var mismatchedSelection = new ReferenceRequest(other, RecordScope.Source, origin.ModKey);
        var stagedSelection = new ReferenceRequest(origin, RecordScope.StagedOutput, origin.ModKey);

        Should.Throw<ArgumentException>(() => new BeginEditRequest(
            Guid.NewGuid(), revision, FormListEditRole.New, origin));
        Should.Throw<ArgumentException>(() => new BeginEditRequest(
            Guid.NewGuid(), revision, FormListEditRole.Override));
        Should.Throw<ArgumentException>(() => new BeginEditRequest(
            Guid.NewGuid(), revision, FormListEditRole.Override, origin, mismatchedSelection));
        Should.Throw<ArgumentException>(() => new BeginEditRequest(
            Guid.NewGuid(), revision, FormListEditRole.Override, origin, stagedSelection));
        Should.Throw<ArgumentException>(() => new BeginEditRequest(
            Guid.NewGuid(), revision, FormListEditRole.Override, origin, sourceSelection, other));
        Should.Throw<ArgumentException>(() => new BeginEditRequest(
            Guid.NewGuid(), revision, FormListEditRole.ExistingOutput));
        Should.Throw<ArgumentException>(() => new BeginEditRequest(
            Guid.NewGuid(), revision, FormListEditRole.ExistingOutput, origin, targetFormKey: other));

        var implicitWinner = new BeginEditRequest(
            Guid.NewGuid(), revision, FormListEditRole.Override, origin);
        var existingOutput = new BeginEditRequest(
            Guid.NewGuid(), revision, FormListEditRole.ExistingOutput, targetFormKey: other);

        implicitWinner.OriginSelection.ShouldBeNull();
        existingOutput.TargetFormKey.ShouldBe(other);
    }

    /// <summary>Verifies common typed edit commands preserve exact ordered and plugin header values.</summary>
    [Fact]
    public void CommonFormListEdits_PreserveExactTypedValues()
    {
        var move = new MoveItemEdit(4, 1);
        var versionControl = new SetVersionControlEdit(uint.MaxValue);
        var formVersion = new SetFormVersionEdit(ushort.MaxValue);
        var version2 = new SetVersion2Edit(ushort.MaxValue);
        var compressed = new SetCompressedEdit(true);
        var deleted = new SetDeletedEdit(false);

        move.CommandName.ShouldBe("form-list.move-item");
        move.SourceIndex.ShouldBe(4);
        move.DestinationIndex.ShouldBe(1);
        versionControl.CommandName.ShouldBe("form-list.set-version-control");
        versionControl.VersionControl.ShouldBe(uint.MaxValue);
        formVersion.CommandName.ShouldBe("form-list.set-form-version");
        formVersion.FormVersion.ShouldBe(ushort.MaxValue);
        version2.CommandName.ShouldBe("form-list.set-version-2");
        version2.Version2.ShouldBe(ushort.MaxValue);
        compressed.CommandName.ShouldBe("form-list.set-compressed");
        compressed.IsCompressed.ShouldBeTrue();
        deleted.CommandName.ShouldBe("form-list.set-deleted");
        deleted.IsDeleted.ShouldBeFalse();
        Should.Throw<ArgumentOutOfRangeException>(() => new MoveItemEdit(-1, 0));
        Should.Throw<ArgumentOutOfRangeException>(() => new MoveItemEdit(0, -1));
    }

    /// <summary>Verifies staged-edit provenance admits only exact absent, original-output, or immutable source baselines.</summary>
    [Fact]
    public void RecordEditProvenance_WithBaselineCombination_ValidatesExactOrigin()
    {
        var target = new FormKey(ModKey.FromNameAndExtension("Source.esm"), 0x800);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var outputContext = new FormListContext(
            new ReferenceRequest(target, RecordScope.StagedOutput, outputModKey),
            ReferenceResolutionStatus.Resolved,
            outputModKey,
            Path.GetFullPath("Output.esp"),
            2,
            PluginRole.Output);
        var sourceContext = new FormListContext(
            new ReferenceRequest(target, RecordScope.Source, target.ModKey),
            ReferenceResolutionStatus.Deleted,
            target.ModKey,
            Path.GetFullPath("Source.esm"),
            0,
            PluginRole.Source);
        var sourceBaselineId = Guid.NewGuid();

        var absent = new RecordEditProvenance(
            Guid.NewGuid(), target, EditBaselineKind.Absent);
        var originalOutput = new RecordEditProvenance(
            Guid.NewGuid(), target, EditBaselineKind.OriginalOutput, outputContext);
        var source = new RecordEditProvenance(
            Guid.NewGuid(), target, EditBaselineKind.SourceContext, sourceContext, sourceBaselineId);

        absent.BaselineContext.ShouldBeNull();
        originalOutput.BaselineContext.ShouldBeSameAs(outputContext);
        source.SourceBaselineId.ShouldBe(sourceBaselineId);
        Should.Throw<ArgumentException>(() => new RecordEditProvenance(
            Guid.NewGuid(), target, EditBaselineKind.Absent, sourceContext));
        Should.Throw<ArgumentException>(() => new RecordEditProvenance(
            Guid.NewGuid(), target, EditBaselineKind.OriginalOutput, sourceContext));
        Should.Throw<ArgumentException>(() => new RecordEditProvenance(
            Guid.NewGuid(), target, EditBaselineKind.SourceContext, sourceContext));
        Should.Throw<ArgumentException>(() => new RecordEditProvenance(
            Guid.NewGuid(), target, EditBaselineKind.SourceContext,
            new FormListContext(
                new ReferenceRequest(target, RecordScope.Source, target.ModKey),
                ReferenceResolutionStatus.Unresolved,
                null,
                null,
                null,
                null),
            sourceBaselineId));
    }

    /// <summary>Verifies a complete baseline snapshots and canonically orders one plugin plus its sidecars.</summary>
    [Fact]
    public void OutputArtifactSetBaseline_WithCompleteArtifacts_SnapshotsCanonicalOrder()
    {
        var fingerprint = new PluginArtifactFingerprint(true, 0, new string('C', 64));
        var pluginPath = Path.GetFullPath("Output.esp");
        var stringsPath = Path.GetFullPath("Output_en.STRINGS");
        var artifacts = new List<PluginArtifactAssociation>
        {
            new(stringsPath, PluginArtifactRole.Strings, "English", fingerprint),
            new(pluginPath, PluginArtifactRole.Plugin, null, fingerprint)
        };

        var baseline = new OutputArtifactSetBaseline(Guid.NewGuid(), artifacts);
        artifacts.Clear();

        baseline.Artifacts.Count.ShouldBe(2);
        baseline.Artifacts[0].Role.ShouldBe(PluginArtifactRole.Plugin);
        baseline.Artifacts[1].Role.ShouldBe(PluginArtifactRole.Strings);
    }

    /// <summary>Verifies a complete output baseline rejects missing plugin identity and path aliases.</summary>
    [Fact]
    public void OutputArtifactSetBaseline_WithIncompleteOrDuplicateArtifacts_RejectsValue()
    {
        var fingerprint = new PluginArtifactFingerprint(true, 0, new string('D', 64));
        var pluginPath = Path.GetFullPath("Output.esp");
        var stringsPath = Path.GetFullPath("Output_en.STRINGS");
        var stringsOnly = new[]
        {
            new PluginArtifactAssociation(stringsPath, PluginArtifactRole.Strings, "English", fingerprint)
        };
        var duplicatePath = new[]
        {
            new PluginArtifactAssociation(pluginPath, PluginArtifactRole.Plugin, null, fingerprint),
            new PluginArtifactAssociation(pluginPath, PluginArtifactRole.Strings, "English", fingerprint)
        };

        Should.Throw<ArgumentException>(() => new OutputArtifactSetBaseline(Guid.NewGuid(), stringsOnly));
        Should.Throw<ArgumentException>(() => new OutputArtifactSetBaseline(Guid.NewGuid(), duplicatePath));
    }
}

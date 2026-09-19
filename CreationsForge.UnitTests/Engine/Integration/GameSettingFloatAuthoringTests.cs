using CreationsForge.Bootstrap.Composition;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda.Plugins;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Integration;

/// <summary>Exercises complete native float game-setting authoring through guarded save and fresh plugin reopen.</summary>
public sealed class GameSettingFloatAuthoringTests
{
    /// <summary>Proves a new native record survives preview, guarded save, and an independent workspace reopen.</summary>
    [Theory]
    [InlineData(SupportedGame.Starfield)]
    [InlineData(SupportedGame.Fallout4)]
    [InlineData(SupportedGame.Skyrim)]
    public async Task NewGameSettingFloat_SavesAndFreshlyReopens(SupportedGame game)
    {
        using var fixture = WorkspaceIntegrationFixture.Create(game);
        var fileName = $"{game}FloatAuthoring.esm";
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("FloatAuthoringOutput");
        var association = new OutputAssociation(
            Path.Combine(outputDirectory.FullName, fileName),
            ModKey.FromNameAndExtension(fileName),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
        var logSink = new ExceptionLogSink();
        using var logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Sink(logSink).CreateLogger();
        await using var services = EngineComposition.Create(logger);

        var opened = await services.WorkspaceFactory.OpenAsync(fixture.CreateOpenRequest(), TestContext.Current.CancellationToken);
        opened.Succeeded.ShouldBeTrue(opened.Error?.Message);
        FormKey formKey;
        await using (var workspace = opened.Value!)
        {
            var selected = await workspace.SelectOutputAsync(
                new SelectOutputRequest(Guid.NewGuid(), workspace.Revision, OutputSelectionMode.CreateNew, association),
                TestContext.Current.CancellationToken);
            selected.Succeeded.ShouldBeTrue(selected.Error?.Message);
            var begun = await workspace.BeginEditAsync(
                new BeginEditRequest(Guid.NewGuid(), workspace.Revision, FormListEditRole.New, recordType: "GameSettingFloat"),
                TestContext.Current.CancellationToken);
            begun.Succeeded.ShouldBeTrue($"{begun.Error?.Message}; {logSink.Exceptions}");
            begun.Value!.RecordType.ShouldBe("GameSettingFloat");
            formKey = begun.Value.FormKey;

            var applied = await workspace.ApplyGameSettingFloatEditAsync(
                new GameSettingFloatEditRequest(
                    Guid.NewGuid(), workspace.Revision, begun.Value.EditId,
                    "fCreationsForgeTest", 1.25f, 0, 0, 0, 0),
                TestContext.Current.CancellationToken);
            applied.Succeeded.ShouldBeTrue(applied.Error?.Message);

            var preview = await workspace.PreviewAsync(TestContext.Current.CancellationToken);
            preview.Succeeded.ShouldBeTrue(preview.Error?.Message);
            preview.Value!.MajorRecordComparisons.Count.ShouldBe(1);
            preview.Value.MajorRecordComparisons[0].RecordType.ShouldBe("GameSettingFloat");
            preview.Value.MajorRecordComparisons[0].Before.ShouldBeNull();
            preview.Value.MajorRecordComparisons[0].After.ShouldNotBeNull();

            var saved = await workspace.SaveAsync(
                new SaveRequest(Guid.NewGuid(), workspace.Revision, selected.Value!.Baseline),
                TestContext.Current.CancellationToken);
            saved.Status.ShouldBe(SaveCommitStatus.Committed, saved.Error?.Message);

            var existing = await workspace.BeginEditAsync(
                new BeginEditRequest(
                    Guid.NewGuid(), workspace.Revision, FormListEditRole.ExistingOutput,
                    targetFormKey: formKey, recordType: "GameSettingFloat"),
                TestContext.Current.CancellationToken);
            existing.Succeeded.ShouldBeTrue(existing.Error?.Message);
            var updated = await workspace.ApplyGameSettingFloatEditAsync(
                new GameSettingFloatEditRequest(
                    Guid.NewGuid(), workspace.Revision, existing.Value!.EditId,
                    "fCreationsForgeTest", 2.5f, 0, 0, 0, 0),
                TestContext.Current.CancellationToken);
            updated.Succeeded.ShouldBeTrue(updated.Error?.Message);
            var secondPreview = await workspace.PreviewAsync(TestContext.Current.CancellationToken);
            secondPreview.Succeeded.ShouldBeTrue(secondPreview.Error?.Message);
            var secondComparison = secondPreview.Value!.MajorRecordComparisons.Single();
            secondComparison.Before!.Value.GetProperty("Data").GetProperty("number").GetSingle().ShouldBe(1.25f);
            secondComparison.After!.Value.GetProperty("Data").GetProperty("number").GetSingle().ShouldBe(2.5f);
            var secondSave = await workspace.SaveAsync(
                new SaveRequest(Guid.NewGuid(), workspace.Revision, saved.CommittedBaseline!),
                TestContext.Current.CancellationToken);
            secondSave.Status.ShouldBe(SaveCommitStatus.Committed, secondSave.Error?.Message);
        }

        var reopened = await services.WorkspaceFactory.OpenAsync(fixture.CreateOpenRequest(), TestContext.Current.CancellationToken);
        reopened.Succeeded.ShouldBeTrue(reopened.Error?.Message);
        await using (var workspace = reopened.Value!)
        {
            var selected = await workspace.SelectOutputAsync(
                new SelectOutputRequest(Guid.NewGuid(), workspace.Revision, OutputSelectionMode.OpenExisting, association),
                TestContext.Current.CancellationToken);
            selected.Succeeded.ShouldBeTrue(selected.Error?.Message);
            var read = await workspace.ReadMajorRecordViewAsync(
                new ReferenceRequest(formKey, RecordScope.StagedOutput),
                TestContext.Current.CancellationToken);
            read.Succeeded.ShouldBeTrue(read.Error?.Message);
            read.Value!.RecordType.ShouldBe("GameSettingFloat");
            var fields = read.Value.Record!.Value;
            fields.GetProperty("EditorID").GetString().ShouldBe("fCreationsForgeTest");
            fields.GetProperty("Data").GetProperty("number").GetSingle().ShouldBe(2.5f);
            fields.GetProperty("MajorRecordFlagsRaw").GetInt32().ShouldBe(0);
            fields.GetProperty("FormVersion").GetUInt16().ShouldBe((ushort)0);
            fields.GetProperty("Version2").GetUInt16().ShouldBe((ushort)0);
            fields.GetProperty("VersionControl").GetUInt32().ShouldBe(0U);
            fields.TryGetProperty("XALG", out _).ShouldBe(game == SupportedGame.Starfield);
        }

        var template = fixture.CreateOpenRequest();
        WorkspaceOpenRequest PatchOpenRequest() => new(
            Guid.NewGuid(), game, template.Release, association.PluginPath,
            [association.PluginPath], template.DataDirectoryPath, template.StringDirectoryPaths);
        var patchFileName = $"{game}FloatPatch.esm";
        var patchAssociation = new OutputAssociation(
            Path.Combine(outputDirectory.FullName, patchFileName),
            ModKey.FromNameAndExtension(patchFileName),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
        var patchOpen = await services.WorkspaceFactory.OpenAsync(PatchOpenRequest(), TestContext.Current.CancellationToken);
        patchOpen.Succeeded.ShouldBeTrue(patchOpen.Error?.Message);
        await using (var workspace = patchOpen.Value!)
        {
            var selected = await workspace.SelectOutputAsync(
                new SelectOutputRequest(Guid.NewGuid(), workspace.Revision, OutputSelectionMode.CreateNew, patchAssociation),
                TestContext.Current.CancellationToken);
            selected.Succeeded.ShouldBeTrue(selected.Error?.Message);
            var originRead = await workspace.ReadRecordContextAsync(
                new ReferenceRequest(formKey, RecordScope.WinningOverrides),
                TestContext.Current.CancellationToken);
            originRead.Succeeded.ShouldBeTrue(originRead.Error?.Message);
            originRead.Value!.RecordType.ShouldBe("GameSettingFloat");
            var begun = await workspace.BeginEditAsync(
                new BeginEditRequest(
                    Guid.NewGuid(), workspace.Revision, FormListEditRole.Override,
                    originFormKey: formKey, recordType: "GameSettingFloat"),
                TestContext.Current.CancellationToken);
            begun.Succeeded.ShouldBeTrue($"{begun.Error?.Message}; {logSink.Exceptions}");
            begun.Value!.FormKey.ShouldBe(formKey);
            var applied = await workspace.ApplyGameSettingFloatEditAsync(
                new GameSettingFloatEditRequest(
                    Guid.NewGuid(), workspace.Revision, begun.Value.EditId,
                    "fCreationsForgeTest", 3.75f, 0, 0, 0, 0),
                TestContext.Current.CancellationToken);
            applied.Succeeded.ShouldBeTrue(applied.Error?.Message);
            var preview = await workspace.PreviewAsync(TestContext.Current.CancellationToken);
            preview.Succeeded.ShouldBeTrue(preview.Error?.Message);
            preview.Value!.MajorRecordComparisons.Single().Before!.Value
                .GetProperty("Data").GetProperty("number").GetSingle().ShouldBe(2.5f);
            var saved = await workspace.SaveAsync(
                new SaveRequest(Guid.NewGuid(), workspace.Revision, selected.Value!.Baseline),
                TestContext.Current.CancellationToken);
            saved.Status.ShouldBe(SaveCommitStatus.Committed, saved.Error?.Message);
        }

        var reopenedPatch = await services.WorkspaceFactory.OpenAsync(PatchOpenRequest(), TestContext.Current.CancellationToken);
        reopenedPatch.Succeeded.ShouldBeTrue(reopenedPatch.Error?.Message);
        await using (var workspace = reopenedPatch.Value!)
        {
            var selected = await workspace.SelectOutputAsync(
                new SelectOutputRequest(Guid.NewGuid(), workspace.Revision, OutputSelectionMode.OpenExisting, patchAssociation),
                TestContext.Current.CancellationToken);
            selected.Succeeded.ShouldBeTrue(selected.Error?.Message);
            var read = await workspace.ReadMajorRecordViewAsync(
                new ReferenceRequest(formKey, RecordScope.StagedOutput),
                TestContext.Current.CancellationToken);
            read.Succeeded.ShouldBeTrue(read.Error?.Message);
            read.Value!.Record!.Value.GetProperty("Data").GetProperty("number").GetSingle().ShouldBe(3.75f);
        }
    }
}

internal sealed class ExceptionLogSink : ILogEventSink
{
    private readonly List<string> _exceptions = [];

    public void Emit(LogEvent logEvent)
    {
        if (logEvent.Exception is not null)
        {
            lock (_exceptions)
            {
                _exceptions.Add($"{logEvent.RenderMessage()}: {logEvent.Exception}");
            }
        }
    }

    public string Exceptions
    {
        get
        {
            lock (_exceptions)
            {
                return string.Join(" | ", _exceptions);
            }
        }
    }
}

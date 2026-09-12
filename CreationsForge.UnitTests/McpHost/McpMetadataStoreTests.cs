using System.Security.Cryptography;
using System.Text.Json;
using CreationsForge.Mcp;
using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.UnitTests.McpHost;

/// <summary>Verifies bounded exact-reference retention and pre-invocation operation admission.</summary>
public sealed class McpMetadataStoreTests
{
    /// <summary>Verifies that a first operation reserves its full budget and same-key retries reuse the admission at global capacity.</summary>
    /// <returns>A task that completes after the asynchronous admissions are observed.</returns>
    [Fact]
    public async Task AcquireOperationAsync_AtHostCapacity_ReusesKnownKeyOnly()
    {
        using var store = new McpMetadataStore(maximumHandles: 32, operationReservationSize: 16);
        var firstWorkspaceId = Guid.NewGuid();
        var firstOperationId = Guid.NewGuid();
        var secondWorkspaceId = Guid.NewGuid();
        var secondOperationId = Guid.NewGuid();

        await using (var first = await store.AcquireOperationAsync(
            firstWorkspaceId,
            firstOperationId,
            McpMetadataOperationKind.WorkspaceMutation,
            2))
        {
            first.ShouldNotBeNull();
        }

        await using (var second = await store.AcquireOperationAsync(
            secondWorkspaceId,
            secondOperationId,
            McpMetadataOperationKind.WorkspaceMutation,
            2))
        {
            second.ShouldNotBeNull();
        }

        store.AllocatedSlots.ShouldBe(32);
        await using var replay = await store.AcquireOperationAsync(
            firstWorkspaceId,
            firstOperationId,
            McpMetadataOperationKind.WorkspaceMutation,
            2);
        replay.ShouldNotBeNull();
        var rejected = await store.AcquireOperationAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            McpMetadataOperationKind.WorkspaceMutation,
            2);
        rejected.ShouldBeNull();
    }

    /// <summary>Verifies that callers sharing an operation identity cannot invoke or publish concurrently.</summary>
    /// <returns>A task that completes after the serialized leases are observed.</returns>
    [Fact]
    public async Task AcquireOperationAsync_ForSameKey_SerializesLeases()
    {
        using var store = new McpMetadataStore(maximumHandles: 16, operationReservationSize: 16);
        var workspaceId = Guid.NewGuid();
        var operationId = Guid.NewGuid();
        var first = await store.AcquireOperationAsync(
            workspaceId,
            operationId,
            McpMetadataOperationKind.WorkspaceMutation,
            1);
        first.ShouldNotBeNull();

        var pending = store.AcquireOperationAsync(
            workspaceId,
            operationId,
            McpMetadataOperationKind.WorkspaceMutation,
            1).AsTask();
        pending.IsCompleted.ShouldBeFalse();

        await first.DisposeAsync();
        await using var second = await pending;
        second.ShouldNotBeNull();
    }

    /// <summary>Verifies that workspace-state publication is atomic when two new references cannot both fit.</summary>
    [Fact]
    public void TryPublishWorkspaceStateMetadata_WhenPairCannotFit_DoesNotPartiallyPublish()
    {
        using var store = new McpMetadataStore(maximumHandles: 1, operationReservationSize: 1);
        var output = CreateOutput(0);
        var baseline = CreateBaseline(0);

        var published = store.TryPublishWorkspaceStateMetadata(
            Guid.NewGuid(),
            output,
            baseline,
            null,
            out var outputReference,
            out var baselineReference,
            out var pendingOutputReference);

        published.ShouldBeFalse();
        outputReference.ShouldBeNull();
        baselineReference.ShouldBeNull();
        pendingOutputReference.ShouldBeNull();
        store.HandleCount.ShouldBe(0);
        store.AllocatedSlots.ShouldBe(0);
    }

    /// <summary>Verifies that a journaled pending output is retained independently when no output is currently selected.</summary>
    [Fact]
    public void TryPublishWorkspaceStateMetadata_WithOnlyPendingOutput_PublishesExactPendingReference()
    {
        using var store = new McpMetadataStore(maximumHandles: 1, operationReservationSize: 1);
        var pendingOutput = CreateOutput(0);

        var published = store.TryPublishWorkspaceStateMetadata(
            Guid.NewGuid(),
            null,
            null,
            pendingOutput,
            out var outputReference,
            out var baselineReference,
            out var pendingOutputReference);

        published.ShouldBeTrue();
        outputReference.ShouldBeNull();
        baselineReference.ShouldBeNull();
        pendingOutputReference.ShouldNotBeNull();
        store.TryResolve(pendingOutputReference.Handle, out _, out var retained).ShouldBeTrue();
        retained.ShouldBeSameAs(pendingOutput);
    }

    /// <summary>Verifies exact reference deduplication while adding a second workspace origin to the same handle.</summary>
    /// <returns>A task that completes after exact reference identity and origins are verified.</returns>
    [Fact]
    public async Task Publish_SameReference_ReusesHandleAndTracksWorkspaceOrigins()
    {
        using var store = new McpMetadataStore(maximumHandles: 32, operationReservationSize: 16);
        var firstWorkspaceId = Guid.NewGuid();
        var secondWorkspaceId = Guid.NewGuid();
        var baseline = CreateBaseline(0);
        await using var operation = await store.AcquireOperationAsync(
            firstWorkspaceId,
            Guid.NewGuid(),
            McpMetadataOperationKind.WorkspaceMutation,
            1);
        operation.ShouldNotBeNull();
        var originalReference = operation.Publish(baseline);
        originalReference.ShouldNotBeNull();
        store.TryResolveOutputBaseline(originalReference.Handle, secondWorkspaceId, out _).ShouldBeFalse();

        store.TryPublishWorkspaceMetadata(secondWorkspaceId, baseline, out var secondReference).ShouldBeTrue();

        secondReference.Handle.ShouldBe(originalReference.Handle);
        store.HandleCount.ShouldBe(1);
        store.TryResolveOutputBaseline(originalReference.Handle, secondWorkspaceId, out var resolved).ShouldBeTrue();
        resolved.ShouldBeSameAs(baseline);
    }

    /// <summary>Verifies every fresh observation retry requires its full publication allowance from the retained reservation.</summary>
    /// <returns>A task that completes after same-key recovery and repair admissions are rejected.</returns>
    [Fact]
    public async Task AcquireOperationAsync_WhenReservationHasOnlyThreeSlots_RejectsFreshObservationRetries()
    {
        using var store = new McpMetadataStore(maximumHandles: 16, operationReservationSize: 16);
        var workspaceId = Guid.NewGuid();
        var operationId = Guid.NewGuid();

        await using (var first = await store.AcquireOperationAsync(
            workspaceId,
            operationId,
            McpMetadataOperationKind.Recover,
            4))
        {
            first.ShouldNotBeNull();
            for (var index = 0; index < 13; index++)
            {
                first.Publish(CreateOutput(index)).ShouldNotBeNull();
            }
        }

        var recovery = await store.AcquireOperationAsync(
            workspaceId,
            operationId,
            McpMetadataOperationKind.Recover,
            4);
        var repair = await store.AcquireOperationAsync(
            workspaceId,
            operationId,
            McpMetadataOperationKind.Repair,
            4);

        recovery.ShouldBeNull();
        repair.ShouldBeNull();
    }

    /// <summary>Verifies an insufficient save metadata budget publishes no partial committed baseline or detail handle.</summary>
    [Fact]
    public void TryPublishSaveMetadata_WhenCompleteGroupCannotFit_PublishesNothing()
    {
        using var store = new McpMetadataStore(maximumHandles: 1, operationReservationSize: 1);
        var workspaceId = Guid.NewGuid();
        var operationId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 0);
        var baseline = CreateBaseline(0);
        var saveResult = new SaveResult(
            workspaceId,
            operationId,
            revision,
            revision,
            SaveCommitStatus.Committed,
            baseline,
            null,
            null,
            null,
            Array.Empty<EngineWarning>());

        var published = store.TryPublishSaveMetadata(workspaceId, saveResult, out var references);

        published.ShouldBeFalse();
        references.CommittedBaseline.ShouldBeNull();
        references.Output.ShouldBeNull();
        references.ResolvedEvidence.ShouldBeNull();
        references.Details.ShouldBeNull();
        store.HandleCount.ShouldBe(0);
        store.AllocatedSlots.ShouldBe(0);
    }

    /// <summary>Verifies that checksummed cursors with overflowing numeric bindings are rejected without throwing.</summary>
    [Fact]
    public void TryDecode_WithOverflowingNumericBindings_ReturnsFalse()
    {
        const string Handle = "cfmd_test";
        const string Path = "/artifacts";
        const int MaximumResults = 50;
        var invalidVersion = EncodeCursorPayload(new
        {
            version = (long)int.MaxValue + 1,
            handle = Handle,
            path = Path,
            maximumResults = MaximumResults,
            policy = McpJsonPager.Policy,
            nextPosition = 1,
        });
        var invalidMaximumResults = EncodeCursorPayload(new
        {
            version = 1,
            handle = Handle,
            path = Path,
            maximumResults = (long)int.MaxValue + 1,
            policy = McpJsonPager.Policy,
            nextPosition = 1,
        });

        McpMetadataCursor.TryDecode(invalidVersion, Handle, Path, MaximumResults, out _).ShouldBeFalse();
        McpMetadataCursor.TryDecode(invalidMaximumResults, Handle, Path, MaximumResults, out _).ShouldBeFalse();
    }

    /// <summary>Creates an exact test output association with a unique canonical path and ModKey.</summary>
    /// <param name="index">The unique association index.</param>
    /// <returns>An immutable output association.</returns>
    private static OutputAssociation CreateOutput(int index)
    {
        var fileName = $"Metadata{index:D2}.esp";
        return new OutputAssociation(
            Path.Combine(Path.GetTempPath(), fileName),
            ModKey.FromNameAndExtension(fileName),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
    }

    /// <summary>Creates a complete one-plugin absent baseline for store identity tests.</summary>
    /// <param name="index">The unique baseline index.</param>
    /// <returns>An immutable output baseline.</returns>
    private static OutputArtifactSetBaseline CreateBaseline(int index)
    {
        return new OutputArtifactSetBaseline(
            Guid.NewGuid(),
            Array.AsReadOnly(new[]
            {
                new NativeArtifactAssociation(
                    CreateOutput(index).PluginPath,
                    NativeArtifactRole.Plugin,
                    null,
                    new NativeArtifactFingerprint(false, 0, null)),
            }));
    }

    /// <summary>Creates a canonical Base64Url cursor with a valid checksum around a caller-controlled JSON payload.</summary>
    /// <param name="payload">The JSON value to checksum and encode.</param>
    /// <returns>A canonical unpadded Base64Url cursor.</returns>
    private static string EncodeCursorPayload(object payload)
    {
        var payloadBytes = JsonSerializer.SerializeToUtf8Bytes(payload);
        var bytes = new byte[payloadBytes.Length + 32];
        payloadBytes.CopyTo(bytes, 0);
        SHA256.HashData(payloadBytes, bytes.AsSpan(payloadBytes.Length, 32));
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}

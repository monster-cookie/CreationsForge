using System.Buffers;
using System.Globalization;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.NativeEditing.Schema;
using CreationsForge.Services;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Creates bounded typed command drafts from exact schemas and revision-bound detached record seeds.</summary>
public sealed class NativeFormListDraftFactory : INativeFormListDraftFactory
{
    /// <inheritdoc />
    public EngineResult<NativeFormListDraftSeed> CaptureSeed(
        NativeFormListWireCatalogContext context,
        Guid workspaceId,
        WorkspaceRevision revision,
        FormListContext recordContext,
        JsonElement record,
        NativeWireReadLimits limits,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(recordContext);
        ArgumentNullException.ThrowIfNull(limits);
        cancellationToken.ThrowIfCancellationRequested();
        if (workspaceId == Guid.Empty)
        {
            return FailureSeed("$: native editor seed capture requires a non-empty workspace identifier.");
        }

        if (record.ValueKind != JsonValueKind.Object)
        {
            return FailureSeed("$: native editor seed capture requires one detached expanded record object.");
        }

        if (context.Identity.Game != context.Codec.Game || context.Identity.Release != context.Codec.Release)
        {
            return FailureSeed("$: native editor seed capture rejected a mismatched schema catalog and codec context.");
        }

        try
        {
            var counter = new JsonLimitCounter(limits, cancellationToken);
            counter.Visit(record, "$", 1);
            if (!record.TryGetProperty("FormKey", out var formKey) || formKey.ValueKind != JsonValueKind.String || !string.Equals(formKey.GetString(), recordContext.Selection.FormKey.ToString(), StringComparison.Ordinal))
            {
                return FailureSeed("$.FormKey: detached native record identity does not match the selected record context.");
            }

            return EngineResult<NativeFormListDraftSeed>.Success(
                new NativeFormListDraftSeed(workspaceId, revision, recordContext, context.Identity, record));
        }
        catch (DraftBuildException exception)
        {
            return FailureSeed(exception.Message);
        }
    }

    /// <inheritdoc />
    public EngineResult<NativeFormListDraft> Create(
        NativeFormListWireCatalogContext context,
        NativeWireSchemaNodeKey commandKey,
        NativeFormListDraftSeed? seed,
        NativeFormListDraftSeedSelection selection,
        NativeWireReadLimits limits,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(commandKey);
        ArgumentNullException.ThrowIfNull(selection);
        ArgumentNullException.ThrowIfNull(limits);
        cancellationToken.ThrowIfCancellationRequested();
        if (commandKey.Kind != NativeWireSchemaNodeKind.Command || !string.Equals(commandKey.CatalogId, context.Identity.CatalogId, StringComparison.Ordinal))
        {
            return FailureDraft($"$: command key '{commandKey.Name}' does not belong to active catalog {context.Identity.CatalogId}.");
        }

        if (seed is not null && !CatalogIdentityEquals(seed.CatalogIdentity, context.Identity))
        {
            return FailureDraft("$: revision-bound seed catalog identity does not match the active editor catalog.");
        }

        var policyResult = NativeFormListCommandSeedCatalog.Resolve(context, commandKey.Name);
        if (!policyResult.Succeeded || policyResult.Value is null)
        {
            return FailureDraft(policyResult.Error?.Message ?? $"$: command '{commandKey.Name}' has no seed policy.");
        }

        var schemaResult = new NativeWireSchemaIndex(context).Resolve(commandKey, limits, cancellationToken);
        if (!schemaResult.Succeeded || schemaResult.Value is null)
        {
            return FailureDraft(schemaResult.Error?.Message ?? $"$: command schema '{commandKey.Name}' could not be resolved.");
        }

        try
        {
            var arguments = CreateSeedArguments(policyResult.Value, seed, selection, cancellationToken);
            JsonElement? initialValue = arguments;
            var initialState = arguments.HasValue ? NativeWireDraftValueState.Seeded : NativeWireDraftValueState.Defaulted;
            if (!initialValue.HasValue && schemaResult.Value.DefaultTemplate.HasValue)
            {
                initialValue = schemaResult.Value.DefaultTemplate.Value;
                initialState = NativeWireDraftValueState.Defaulted;
            }

            var state = new DraftBuildState(limits, cancellationToken);
            var root = state.Build(schemaResult.Value, "$", commandKey.Name, true, false, initialValue, initialState, false, 1);
            if (root is not NativeWireObjectDraftNode)
            {
                return FailureDraft($"$: command '{commandKey.Name}' schema did not resolve to a closed object draft.");
            }

            return EngineResult<NativeFormListDraft>.Success(new NativeFormListDraft(commandKey, context.Identity, seed, root, limits));
        }
        catch (DraftBuildException exception)
        {
            return FailureDraft(exception.Message);
        }
    }

    /// <summary>Creates the fixed detached command seed selected by one admitted policy.</summary>
    private static JsonElement? CreateSeedArguments(
        NativeFormListCommandSeedPolicy policy,
        NativeFormListDraftSeed? seed,
        NativeFormListDraftSeedSelection selection,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return policy.Kind switch
        {
            NativeFormListCommandSeedPolicyKind.EmptyArguments => ParseArguments("{}"),
            NativeFormListCommandSeedPolicyKind.DirectRecordProperty => CreateDirectPropertyArguments(RequireSeed(seed, policy), policy.RecordProperty!, policy.ArgumentProperty!),
            NativeFormListCommandSeedPolicyKind.DerivedCompressed => CreateCompressedArguments(RequireSeed(seed, policy)),
            NativeFormListCommandSeedPolicyKind.DerivedDeleted => CreateDeletedArguments(RequireSeed(seed, policy)),
            NativeFormListCommandSeedPolicyKind.CompleteCollection => CreateDirectPropertyArguments(RequireSeed(seed, policy), policy.RecordProperty!, policy.ArgumentProperty!),
            NativeFormListCommandSeedPolicyKind.SelectedExistingIndex => CreateIndexArguments(RequireSeed(seed, policy), policy, selection, true),
            NativeFormListCommandSeedPolicyKind.SelectedInsertionIndex => CreateIndexArguments(seed, policy, selection, false),
            NativeFormListCommandSeedPolicyKind.ExplicitMoveIndices => CreateMoveArguments(RequireSeed(seed, policy), policy, selection),
            NativeFormListCommandSeedPolicyKind.SelectedExistingComponent => CreateComponentArguments(RequireSeed(seed, policy), policy, selection),
            _ => throw new DraftBuildException($"$: unsupported command seed policy {policy.Kind}.")
        };
    }

    /// <summary>Requires a matching revision-bound seed for a value-preserving command.</summary>
    private static NativeFormListDraftSeed RequireSeed(NativeFormListDraftSeed? seed, NativeFormListCommandSeedPolicy policy)
    {
        return seed ?? throw new DraftBuildException($"$: command '{policy.CommandName}' requires a matching revision-bound record seed.");
    }

    /// <summary>Copies one exact record property into one exact command argument.</summary>
    private static JsonElement CreateDirectPropertyArguments(NativeFormListDraftSeed seed, string recordProperty, string argumentProperty)
    {
        if (!seed.Record.TryGetProperty(recordProperty, out var value))
        {
            throw new DraftBuildException($"$.{recordProperty}: revision-bound record seed is missing the required property.");
        }

        return WriteArguments(writer =>
        {
            writer.WriteStartObject();
            writer.WritePropertyName(argumentProperty);
            value.WriteTo(writer);
            writer.WriteEndObject();
        });
    }

    /// <summary>Derives the compressed command value from the exact native raw flag bit.</summary>
    private static JsonElement CreateCompressedArguments(NativeFormListDraftSeed seed)
    {
        if (!seed.Record.TryGetProperty("MajorRecordFlagsRaw", out var flags) || !flags.TryGetInt32(out var rawFlags))
        {
            throw new DraftBuildException("$.MajorRecordFlagsRaw: revision-bound record seed requires an Int32 raw flags value.");
        }

        return WriteArguments(writer =>
        {
            writer.WriteStartObject();
            writer.WriteBoolean("isCompressed", (rawFlags & 0x00040000) != 0);
            writer.WriteEndObject();
        });
    }

    /// <summary>Derives the deleted command value from the exact selected record context.</summary>
    private static JsonElement CreateDeletedArguments(NativeFormListDraftSeed seed)
    {
        return WriteArguments(writer =>
        {
            writer.WriteStartObject();
            writer.WriteBoolean("isDeleted", seed.RecordContext.Status == ReferenceResolutionStatus.Deleted);
            writer.WriteEndObject();
        });
    }

    /// <summary>Creates selected existing or insertion index arguments.</summary>
    private static JsonElement CreateIndexArguments(NativeFormListDraftSeed? seed, NativeFormListCommandSeedPolicy policy, NativeFormListDraftSeedSelection selection, bool requireExisting)
    {
        var expectedKind = requireExisting ? NativeFormListDraftSeedSelectionKind.AtIndex : NativeFormListDraftSeedSelectionKind.InsertAt;
        if (selection.Kind != expectedKind || !selection.Index.HasValue)
        {
            throw new DraftBuildException($"$.index: command '{policy.CommandName}' requires {expectedKind} seed selection.");
        }

        var index = selection.Index.Value;
        if (requireExisting)
        {
            var collectionName = policy.CommandName.Contains("component", StringComparison.Ordinal) ? "Components" : "Items";
            var count = RequireArray(seed!, collectionName).GetArrayLength();
            if (index >= count)
            {
                throw new DraftBuildException($"$.index: selected index {index} is outside seeded {collectionName} count {count}.");
            }
        }
        else if (seed is not null)
        {
            var collectionName = policy.CommandName.Contains("component", StringComparison.Ordinal) ? "Components" : "Items";
            var count = RequireArray(seed, collectionName).GetArrayLength();
            if (index > count)
            {
                throw new DraftBuildException($"$.index: insertion index {index} is outside seeded {collectionName} range 0 through {count}.");
            }
        }

        return WriteArguments(writer =>
        {
            writer.WriteStartObject();
            writer.WriteNumber("index", index);
            writer.WriteEndObject();
        });
    }

    /// <summary>Creates explicit move indices after checking the complete seeded Items collection.</summary>
    private static JsonElement CreateMoveArguments(NativeFormListDraftSeed seed, NativeFormListCommandSeedPolicy policy, NativeFormListDraftSeedSelection selection)
    {
        if (selection.Kind != NativeFormListDraftSeedSelectionKind.Move || !selection.SourceIndex.HasValue || !selection.DestinationIndex.HasValue)
        {
            throw new DraftBuildException($"$: command '{policy.CommandName}' requires Move seed selection.");
        }

        var count = RequireArray(seed, "Items").GetArrayLength();
        if (selection.SourceIndex.Value >= count || selection.DestinationIndex.Value >= count)
        {
            throw new DraftBuildException($"$: move indices {selection.SourceIndex.Value} and {selection.DestinationIndex.Value} must both be within seeded Items count {count}.");
        }

        return WriteArguments(writer =>
        {
            writer.WriteStartObject();
            writer.WriteNumber("sourceIndex", selection.SourceIndex.Value);
            writer.WriteNumber("destinationIndex", selection.DestinationIndex.Value);
            writer.WriteEndObject();
        });
    }

    /// <summary>Creates an indexed complete component replacement seed.</summary>
    private static JsonElement CreateComponentArguments(NativeFormListDraftSeed seed, NativeFormListCommandSeedPolicy policy, NativeFormListDraftSeedSelection selection)
    {
        if (selection.Kind != NativeFormListDraftSeedSelectionKind.AtIndex || !selection.Index.HasValue)
        {
            throw new DraftBuildException($"$: command '{policy.CommandName}' requires AtIndex seed selection.");
        }

        var components = RequireArray(seed, "Components");
        var index = selection.Index.Value;
        if (index >= components.GetArrayLength())
        {
            throw new DraftBuildException($"$.index: selected component index {index} is outside seeded Components count {components.GetArrayLength()}.");
        }

        var component = components[index];
        return WriteArguments(writer =>
        {
            writer.WriteStartObject();
            writer.WriteNumber("index", index);
            writer.WritePropertyName("component");
            component.WriteTo(writer);
            writer.WriteEndObject();
        });
    }

    /// <summary>Requires one exact seeded array property.</summary>
    private static JsonElement RequireArray(NativeFormListDraftSeed seed, string propertyName)
    {
        if (!seed.Record.TryGetProperty(propertyName, out var value) || value.ValueKind != JsonValueKind.Array)
        {
            throw new DraftBuildException($"$.{propertyName}: revision-bound record seed requires an array.");
        }

        return value;
    }

    /// <summary>Writes and detaches exactly one command arguments object.</summary>
    private static JsonElement WriteArguments(Action<Utf8JsonWriter> write)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using (var writer = new Utf8JsonWriter(buffer))
        {
            write(writer);
        }

        using var document = JsonDocument.Parse(buffer.WrittenMemory);
        return document.RootElement.Clone();
    }

    /// <summary>Parses and detaches one fixed command argument template.</summary>
    private static JsonElement ParseArguments(string json)
    {
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }

    /// <summary>Compares every field of two catalog identities.</summary>
    private static bool CatalogIdentityEquals(NativeWireSchemaCatalogIdentity left, NativeWireSchemaCatalogIdentity right)
    {
        return left.Game == right.Game && left.Release == right.Release &&
            string.Equals(left.SchemaVersion, right.SchemaVersion, StringComparison.Ordinal) &&
            string.Equals(left.CatalogId, right.CatalogId, StringComparison.Ordinal);
    }

    /// <summary>Creates a typed seed failure.</summary>
    private static EngineResult<NativeFormListDraftSeed> FailureSeed(string message)
    {
        return EngineResult<NativeFormListDraftSeed>.Failure(new EngineError(EngineErrorCode.ValidationFailed, message));
    }

    /// <summary>Creates a typed draft failure.</summary>
    private static EngineResult<NativeFormListDraft> FailureDraft(string message)
    {
        return EngineResult<NativeFormListDraft>.Failure(new EngineError(EngineErrorCode.ValidationFailed, message));
    }

    /// <summary>Builds typed nodes while enforcing the operation's limits.</summary>
    internal sealed class DraftBuildState
    {
        /// <summary>The operation limits.</summary>
        private readonly NativeWireReadLimits Limits;

        /// <summary>The operation cancellation token.</summary>
        private readonly CancellationToken CancellationToken;

        /// <summary>The number of materialized values.</summary>
        private int MaterializedNodes;

        /// <summary>Initializes one bounded builder.</summary>
        internal DraftBuildState(NativeWireReadLimits limits, CancellationToken cancellationToken)
        {
            Limits = limits;
            CancellationToken = cancellationToken;
        }

        /// <summary>Builds one typed node from a descriptor and optional current value.</summary>
        internal NativeWireDraftNode Build(
            NativeWireSchemaDescriptor descriptor,
            string path,
            string displayName,
            bool isRequired,
            bool isReadOnly,
            JsonElement? value,
            NativeWireDraftValueState valueState,
            bool strictSeed,
            int depth)
        {
            Visit(path, depth);
            var hasValue = value.HasValue && value.Value.ValueKind != JsonValueKind.Undefined;
            if (hasValue && value.GetValueOrDefault().ValueKind == JsonValueKind.Null && descriptor.Kind != NativeWireSchemaValueKind.Nullable)
            {
                return BuildUnset(descriptor, path, displayName, isRequired, isReadOnly, depth);
            }

            if (!hasValue && descriptor.Constant.HasValue)
            {
                value = descriptor.Constant.Value;
                hasValue = true;
                valueState = NativeWireDraftValueState.SchemaConstant;
            }
            else if (!hasValue && descriptor.DefaultValue.HasValue)
            {
                value = descriptor.DefaultValue.Value;
                hasValue = true;
                valueState = NativeWireDraftValueState.Defaulted;
            }

            return descriptor.Kind switch
            {
                NativeWireSchemaValueKind.Object => BuildObject(descriptor, NativeWireDraftNodeKind.Object, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                NativeWireSchemaValueKind.Array => BuildArray(descriptor, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                NativeWireSchemaValueKind.Union => BuildUnion(descriptor, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                NativeWireSchemaValueKind.Nullable => BuildNullable(descriptor, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                NativeWireSchemaValueKind.Boolean => BuildBoolean(descriptor, path, displayName, isRequired, isReadOnly, value, valueState),
                NativeWireSchemaValueKind.Integer => BuildInteger(descriptor, path, displayName, isRequired, isReadOnly, value, valueState),
                NativeWireSchemaValueKind.String => BuildString(descriptor, NativeWireDraftNodeKind.String, path, displayName, isRequired, isReadOnly, value, valueState),
                NativeWireSchemaValueKind.Enum => BuildEnum(descriptor, path, displayName, isRequired, isReadOnly, value, valueState),
                NativeWireSchemaValueKind.FormLink => BuildFormLink(descriptor, path, displayName, isRequired, isReadOnly, value, valueState),
                NativeWireSchemaValueKind.FormLinkOrIndex => BuildFormLinkOrIndex(descriptor, path, displayName, isRequired, isReadOnly, value, valueState, depth),
                NativeWireSchemaValueKind.FloatBits => BuildSpecialObject(descriptor, NativeWireDraftNodeKind.FloatBits, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                NativeWireSchemaValueKind.ByteArray => BuildSpecialObject(descriptor, NativeWireDraftNodeKind.ByteArray, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                NativeWireSchemaValueKind.TranslatedString => BuildSpecialObject(descriptor, NativeWireDraftNodeKind.TranslatedString, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                NativeWireSchemaValueKind.Asset => BuildSpecialObject(descriptor, NativeWireDraftNodeKind.Asset, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                NativeWireSchemaValueKind.Color => BuildSpecialObject(descriptor, NativeWireDraftNodeKind.Color, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                NativeWireSchemaValueKind.Array2D => BuildSpecialObject(descriptor, NativeWireDraftNodeKind.Array2D, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                _ => throw new DraftBuildException($"{path}: unsupported typed draft shape {descriptor.Kind}.")
            };
        }

        /// <summary>Builds a placeholder whose required value has no honest default.</summary>
        private NativeWireDraftNode BuildUnset(NativeWireSchemaDescriptor descriptor, string path, string displayName, bool isRequired, bool isReadOnly, int depth)
        {
            return descriptor.Kind switch
            {
                NativeWireSchemaValueKind.Object => BuildObject(descriptor, NativeWireDraftNodeKind.Object, path, displayName, isRequired, isReadOnly, null, NativeWireDraftValueState.RequiredUnset, false, depth),
                NativeWireSchemaValueKind.Array => BuildArray(descriptor, path, displayName, isRequired, isReadOnly, null, NativeWireDraftValueState.RequiredUnset, false, depth),
                NativeWireSchemaValueKind.Union => BuildUnion(descriptor, path, displayName, isRequired, isReadOnly, null, NativeWireDraftValueState.RequiredUnset, false, depth),
                NativeWireSchemaValueKind.Nullable => BuildNullable(descriptor, path, displayName, isRequired, isReadOnly, null, NativeWireDraftValueState.RequiredUnset, false, depth),
                NativeWireSchemaValueKind.Boolean => new NativeWireBooleanDraftNode(path, displayName, descriptor, isRequired, isReadOnly, NativeWireDraftValueState.RequiredUnset, false),
                NativeWireSchemaValueKind.Integer => new NativeWireIntegerDraftNode(NativeWireDraftNodeKind.Integer, path, displayName, descriptor, isRequired, isReadOnly, NativeWireDraftValueState.RequiredUnset, "0"),
                NativeWireSchemaValueKind.String => new NativeWireStringDraftNode(NativeWireDraftNodeKind.String, path, displayName, descriptor, isRequired, isReadOnly, NativeWireDraftValueState.RequiredUnset, string.Empty),
                NativeWireSchemaValueKind.Enum => new NativeWireEnumDraftNode(path, displayName, descriptor, isRequired, isReadOnly, NativeWireDraftValueState.RequiredUnset, "0"),
                NativeWireSchemaValueKind.FormLink => new NativeWireFormLinkDraftNode(path, displayName, descriptor, isRequired, isReadOnly, NativeWireDraftValueState.RequiredUnset, false, null),
                NativeWireSchemaValueKind.ByteArray => BuildSpecialObject(descriptor, NativeWireDraftNodeKind.ByteArray, path, displayName, isRequired, isReadOnly, null, NativeWireDraftValueState.RequiredUnset, false, depth),
                NativeWireSchemaValueKind.FormLinkOrIndex => BuildFormLinkOrIndex(descriptor, path, displayName, isRequired, isReadOnly, null, NativeWireDraftValueState.RequiredUnset, depth),
                _ => BuildSpecialObject(descriptor, ToDraftKind(descriptor.Kind), path, displayName, isRequired, isReadOnly, null, NativeWireDraftValueState.RequiredUnset, false, depth),
            };
        }

        /// <summary>Builds one ordinary or specialized object and preserves property order.</summary>
        private NativeWireDraftNode BuildObject(NativeWireSchemaDescriptor descriptor, NativeWireDraftNodeKind kind, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, NativeWireDraftValueState valueState, bool strictSeed, int depth)
        {
            if (value.HasValue && value.Value.ValueKind != JsonValueKind.Object)
            {
                throw new DraftBuildException($"{path}: expected an object, found {value.Value.ValueKind}.");
            }

            ValidateClosedObject(descriptor, value, path);
            var properties = new List<NativeWireObjectDraftProperty>();
            foreach (var property in descriptor.Properties)
            {
                CancellationToken.ThrowIfCancellationRequested();
                var propertyPath = AppendProperty(path, property.Name);
                JsonElement propertyValue = default;
                var present = value.HasValue && value.GetValueOrDefault().TryGetProperty(property.Name, out propertyValue);
                if (!present && strictSeed && property.IsRequired)
                {
                    throw new DraftBuildException($"{propertyPath}: seeded object is missing a required property.");
                }

                var childState = present ? valueState : NativeWireDraftValueState.RequiredUnset;
                var child = Build(property.Descriptor, propertyPath, property.Name, property.IsRequired, property.IsReadOnly, present ? propertyValue : null, childState, present && valueState == NativeWireDraftValueState.Seeded, depth + 1);
                properties.Add(new NativeWireObjectDraftProperty(property.Name, child));
            }

            var objectState = valueState;
            var objectNode = kind switch
            {
                NativeWireDraftNodeKind.Object => new NativeWireObjectDraftNode(kind, path, displayName, descriptor, isRequired, isReadOnly, objectState, properties),
                NativeWireDraftNodeKind.FloatBits => new NativeWireFloatBitsDraftNode(path, displayName, descriptor, isRequired, isReadOnly, objectState, properties),
                NativeWireDraftNodeKind.ByteArray => new NativeWireByteArrayDraftNode(path, displayName, descriptor, isRequired, isReadOnly, objectState, properties),
                NativeWireDraftNodeKind.TranslatedString => new NativeWireTranslatedStringDraftNode(path, displayName, descriptor, isRequired, isReadOnly, objectState, properties),
                NativeWireDraftNodeKind.Asset => new NativeWireAssetDraftNode(path, displayName, descriptor, isRequired, isReadOnly, objectState, properties),
                NativeWireDraftNodeKind.Color => new NativeWireColorDraftNode(path, displayName, descriptor, isRequired, isReadOnly, objectState, properties),
                NativeWireDraftNodeKind.Array2D => new NativeWireArray2DDraftNode(path, displayName, descriptor, isRequired, isReadOnly, objectState, properties),
                _ => throw new DraftBuildException($"{path}: unsupported object draft kind {kind}.")
            };
            BindOwnerDerivedModes(objectNode);
            if (objectNode is NativeWireAssetDraftNode asset &&
                asset.Descriptor.Annotations.TryGetValue("x-native-wrapper-type", out var wrapperType) &&
                !string.IsNullOrWhiteSpace(wrapperType))
            {
                BindAssetAuthoritativeFields(asset);
            }

            return objectNode;
        }

        /// <summary>Binds annotated direct FormLink-or-index values to their exact owner-mode siblings.</summary>
        /// <param name="objectNode">The complete constructed containing object.</param>
        private static void BindOwnerDerivedModes(NativeWireObjectDraftNode objectNode)
        {
            foreach (var property in objectNode.Properties)
            {
                if (property.Node is not NativeWireFormLinkOrIndexDraftNode formLinkOrIndex ||
                    !property.Node.Descriptor.Annotations.TryGetValue("x-native-owner-derived-mode", out var annotation))
                {
                    continue;
                }

                if (!string.Equals(annotation, "true", StringComparison.Ordinal))
                {
                    throw new DraftBuildException($"{property.Node.Path}: x-native-owner-derived-mode must be the Boolean value true.");
                }

                var usesAliases = objectNode.FindProperty("UseAliases") as NativeWireBooleanDraftNode
                    ?? throw new DraftBuildException($"{property.Node.Path}: owner-derived mode requires a direct Boolean UseAliases sibling.");
                var usesPackageData = objectNode.FindProperty("UsePackageData") as NativeWireBooleanDraftNode
                    ?? throw new DraftBuildException($"{property.Node.Path}: owner-derived mode requires a direct Boolean UsePackageData sibling.");
                formLinkOrIndex.BindOwnerMode(usesAliases, usesPackageData);
            }
        }

        /// <summary>Binds one asset wrapper's exact editable authority and optional read-only projections.</summary>
        /// <param name="asset">The constructed specialized asset object.</param>
        private static void BindAssetAuthoritativeFields(NativeWireAssetDraftNode asset)
        {
            var value = asset.FindProperty("value") as NativeWireObjectDraftNode
                ?? throw new DraftBuildException($"{asset.Path}.value: asset schema requires a direct object value property.");
            var isNull = value.FindProperty("isNull") as NativeWireBooleanDraftNode
                ?? throw new DraftBuildException($"{value.Path}.isNull: asset schema requires a direct Boolean authority.");
            var givenPath = value.FindProperty("givenPath") as NativeWireStringDraftNode
                ?? throw new DraftBuildException($"{value.Path}.givenPath: asset schema requires a direct string authority.");
            var dataRelativePath = RequireOptionalReadOnlyAssetProjection(value, "dataRelativePath");
            var extension = RequireOptionalReadOnlyAssetProjection(value, "extension");
            asset.BindAuthoritativeFields(isNull, givenPath, new[] { dataRelativePath, extension });
        }

        /// <summary>Requires one optional read-only asset projection with its exact direct name.</summary>
        /// <param name="value">The asset value object.</param>
        /// <param name="propertyName">The exact projection property name.</param>
        /// <returns>The validated projection node.</returns>
        private static NativeWireDraftNode RequireOptionalReadOnlyAssetProjection(NativeWireObjectDraftNode value, string propertyName)
        {
            var projection = value.FindProperty(propertyName)
                ?? throw new DraftBuildException($"{value.Path}.{propertyName}: asset schema requires this direct projection.");
            if (projection.IsRequired || !projection.IsReadOnly)
            {
                throw new DraftBuildException($"{projection.Path}: asset projection must be optional and read-only.");
            }

            return projection;
        }

        /// <summary>Builds one specialized object using the ordinary closed-property hydrator.</summary>
        private NativeWireDraftNode BuildSpecialObject(NativeWireSchemaDescriptor descriptor, NativeWireDraftNodeKind kind, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, NativeWireDraftValueState valueState, bool strictSeed, int depth)
        {
            return BuildObject(descriptor, kind, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth);
        }

        /// <summary>Builds one ordered array without normalizing duplicates.</summary>
        private NativeWireDraftNode BuildArray(NativeWireSchemaDescriptor descriptor, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, NativeWireDraftValueState valueState, bool strictSeed, int depth)
        {
            if (descriptor.ItemDescriptor is null)
            {
                throw new DraftBuildException($"{path}: array descriptor has no item shape.");
            }

            if (value.HasValue && value.Value.ValueKind != JsonValueKind.Array)
            {
                throw new DraftBuildException($"{path}: expected an array, found {value.Value.ValueKind}.");
            }

            var count = value?.GetArrayLength() ?? 0;
            var maximum = Math.Min(Limits.MaximumArrayElements, descriptor.MaximumItems ?? int.MaxValue);
            if (count > maximum)
            {
                throw new DraftBuildException($"{path}: array contains {count} elements, exceeding maximum {maximum}.");
            }

            var items = new List<NativeWireDraftNode>(count);
            if (value.HasValue)
            {
                var index = 0;
                foreach (var item in value.Value.EnumerateArray())
                {
                    items.Add(Build(descriptor.ItemDescriptor, $"{path}[{index}]", $"{displayName} {index + 1}", true, false, item, valueState, strictSeed, depth + 1));
                    index++;
                }
            }

            var array = new NativeWireArrayDraftNode(
                path,
                displayName,
                descriptor,
                isRequired,
                isReadOnly,
                value.HasValue ? valueState : NativeWireDraftValueState.RequiredUnset,
                items,
                (index, token) =>
                {
                    try
                    {
                        token.ThrowIfCancellationRequested();
                        var builder = new DraftBuildState(Limits, token);
                        var initial = descriptor.ItemDescriptor.DefaultTemplate;
                        var initialState = initial.HasValue ? NativeWireDraftValueState.Defaulted : NativeWireDraftValueState.RequiredUnset;
                        var created = builder.Build(descriptor.ItemDescriptor, $"{path}[{index}]", $"{displayName} {index + 1}", true, false, initial, initialState, false, depth + 1);
                        return EngineResult<NativeWireDraftNode>.Success(created);
                    }
                    catch (DraftBuildException exception)
                    {
                        return FailureNode(exception.Message);
                    }
                });
            return array;
        }

        /// <summary>Builds one lazy union and materializes only a seeded exact discriminator match.</summary>
        private NativeWireDraftNode BuildUnion(NativeWireSchemaDescriptor descriptor, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, NativeWireDraftValueState valueState, bool strictSeed, int depth)
        {
            NativeWireSchemaUnionOption? selectedOption = null;
            NativeWireDraftNode? selectedValue = null;
            if (value.HasValue)
            {
                selectedOption = FindSeededUnionOption(descriptor, value.Value, path);
                var resolved = selectedOption.Resolve(Limits, CancellationToken);
                if (!resolved.Succeeded || resolved.Value is null)
                {
                    throw new DraftBuildException(resolved.Error?.Message ?? $"{path}: selected union option could not be resolved.");
                }

                selectedValue = Build(resolved.Value, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth + 1);
            }

            return new NativeWireUnionDraftNode(
                path,
                displayName,
                descriptor,
                isRequired,
                isReadOnly,
                value.HasValue ? valueState : NativeWireDraftValueState.RequiredUnset,
                descriptor.UnionOptions,
                selectedOption,
                selectedValue,
                (currentPath, option, token) =>
                {
                    var resolved = option.Resolve(Limits, token);
                    if (!resolved.Succeeded || resolved.Value is null)
                    {
                        return FailureNode(resolved.Error?.Message ?? $"{currentPath}: union option '{option.Key}' could not be resolved.");
                    }

                    try
                    {
                        var builder = new DraftBuildState(Limits, token);
                        var initial = resolved.Value.DefaultTemplate;
                        var initialState = initial.HasValue ? NativeWireDraftValueState.Defaulted : NativeWireDraftValueState.RequiredUnset;
                        var created = builder.Build(resolved.Value, currentPath, option.DisplayName, isRequired, isReadOnly, initial, initialState, false, depth + 1);
                        return EngineResult<NativeWireDraftNode>.Success(created);
                    }
                    catch (DraftBuildException exception)
                    {
                        return FailureNode(exception.Message);
                    }
                });
        }

        /// <summary>Builds one whole-value nullable wrapper without collapsing nested FormLink null identities.</summary>
        private NativeWireDraftNode BuildNullable(NativeWireSchemaDescriptor descriptor, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, NativeWireDraftValueState valueState, bool strictSeed, int depth)
        {
            if (descriptor.NonNullDescriptor is null)
            {
                throw new DraftBuildException($"{path}: nullable descriptor has no non-null shape.");
            }

            var isNull = value.HasValue && value.Value.ValueKind == JsonValueKind.Null;
            NativeWireDraftNode? presentValue = null;
            if (value.HasValue && !isNull)
            {
                presentValue = Build(descriptor.NonNullDescriptor, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth + 1);
            }

            return new NativeWireNullableDraftNode(
                path,
                displayName,
                descriptor,
                isRequired,
                isReadOnly,
                value.HasValue ? valueState : NativeWireDraftValueState.RequiredUnset,
                isNull,
                presentValue,
                (currentPath, token) =>
                {
                    try
                    {
                        var builder = new DraftBuildState(Limits, token);
                        var initial = descriptor.NonNullDescriptor.DefaultTemplate;
                        var initialState = initial.HasValue ? NativeWireDraftValueState.Defaulted : NativeWireDraftValueState.RequiredUnset;
                        var created = builder.Build(descriptor.NonNullDescriptor, currentPath, displayName, isRequired, isReadOnly, initial, initialState, false, depth + 1);
                        return EngineResult<NativeWireDraftNode>.Success(created);
                    }
                    catch (DraftBuildException exception)
                    {
                        return FailureNode(exception.Message);
                    }
                });
        }

        /// <summary>Builds one Boolean value.</summary>
        private static NativeWireDraftNode BuildBoolean(NativeWireSchemaDescriptor descriptor, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, NativeWireDraftValueState valueState)
        {
            if (value.HasValue && value.Value.ValueKind is not JsonValueKind.True and not JsonValueKind.False)
            {
                throw new DraftBuildException($"{path}: expected a Boolean, found {value.Value.ValueKind}.");
            }

            return new NativeWireBooleanDraftNode(path, displayName, descriptor, isRequired, isReadOnly, value.HasValue ? valueState : NativeWireDraftValueState.RequiredUnset, value?.GetBoolean() ?? false);
        }

        /// <summary>Builds one exact integer value.</summary>
        private static NativeWireDraftNode BuildInteger(NativeWireSchemaDescriptor descriptor, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, NativeWireDraftValueState valueState)
        {
            if (value.HasValue && value.Value.ValueKind != JsonValueKind.Number)
            {
                throw new DraftBuildException($"{path}: expected an integer, found {value.Value.ValueKind}.");
            }

            return new NativeWireIntegerDraftNode(NativeWireDraftNodeKind.Integer, path, displayName, descriptor, isRequired, isReadOnly, value.HasValue ? valueState : NativeWireDraftValueState.RequiredUnset, value?.GetRawText() ?? "0");
        }

        /// <summary>Builds one exact known-or-unknown enum integer value.</summary>
        private static NativeWireDraftNode BuildEnum(NativeWireSchemaDescriptor descriptor, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, NativeWireDraftValueState valueState)
        {
            if (value.HasValue && value.Value.ValueKind != JsonValueKind.Number)
            {
                throw new DraftBuildException($"{path}: expected an enum integer, found {value.Value.ValueKind}.");
            }

            return new NativeWireEnumDraftNode(path, displayName, descriptor, isRequired, isReadOnly, value.HasValue ? valueState : NativeWireDraftValueState.RequiredUnset, value?.GetRawText() ?? "0");
        }

        /// <summary>Builds one exact bounded string or Base64 text value.</summary>
        private NativeWireDraftNode BuildString(NativeWireSchemaDescriptor descriptor, NativeWireDraftNodeKind kind, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, NativeWireDraftValueState valueState)
        {
            var isJsonNumber = descriptor.Annotations.ContainsKey("x-presentation-json-number");
            if (value.HasValue && value.Value.ValueKind != (isJsonNumber ? JsonValueKind.Number : JsonValueKind.String))
            {
                throw new DraftBuildException($"{path}: expected {(isJsonNumber ? "a JSON number" : "a string")}, found {value.Value.ValueKind}.");
            }

            var text = value.HasValue ? isJsonNumber ? value.Value.GetRawText() : value.Value.GetString()! : string.Empty;
            if (text.Length > Limits.MaximumStringLength)
            {
                throw new DraftBuildException($"{path}: string length {text.Length} exceeds operation maximum {Limits.MaximumStringLength}.");
            }

            return new NativeWireStringDraftNode(kind, path, displayName, descriptor, isRequired, isReadOnly, value.HasValue ? valueState : NativeWireDraftValueState.RequiredUnset, text);
        }

        /// <summary>Builds one specialized FormLink wrapper.</summary>
        private static NativeWireDraftNode BuildFormLink(NativeWireSchemaDescriptor descriptor, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, NativeWireDraftValueState valueState)
        {
            if (!value.HasValue)
            {
                return new NativeWireFormLinkDraftNode(path, displayName, descriptor, isRequired, isReadOnly, NativeWireDraftValueState.RequiredUnset, false, null);
            }

            if (value.Value.ValueKind != JsonValueKind.Object || !value.Value.TryGetProperty("isNull", out var isNull) || isNull.ValueKind is not JsonValueKind.True and not JsonValueKind.False || !value.Value.TryGetProperty("formKey", out var formKey) || formKey.ValueKind is not JsonValueKind.String and not JsonValueKind.Null)
            {
                throw new DraftBuildException($"{path}: expected a FormLink object with Boolean isNull and string-or-null formKey.");
            }

            ValidateClosedObject(descriptor, value, path);
            var typeProperty = descriptor.Properties.FirstOrDefault(property => string.Equals(property.Name, "$type", StringComparison.Ordinal));
            var hasType = value.Value.TryGetProperty("$type", out var type);
            if (hasType && (type.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(type.GetString())))
            {
                throw new DraftBuildException($"{path}.$type: FormLink wrapper discriminator must be a non-empty string.");
            }

            if (typeProperty?.IsRequired == true && !hasType)
            {
                throw new DraftBuildException($"{path}.$type: generated FormLink wrapper requires its exact discriminator.");
            }

            return new NativeWireFormLinkDraftNode(path, displayName, descriptor, isRequired, isReadOnly, valueState, isNull.GetBoolean(), formKey.ValueKind == JsonValueKind.Null ? null : formKey.GetString(), hasType ? type.GetString() : null);
        }

        /// <summary>Builds one owner-mode-sensitive FormLink-or-index representation.</summary>
        private NativeWireDraftNode BuildFormLinkOrIndex(NativeWireSchemaDescriptor descriptor, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, NativeWireDraftValueState valueState, int depth)
        {
            if (!value.HasValue)
            {
                var unsetLinkDescriptor = descriptor.Properties.FirstOrDefault(property => property.Name == "link")?.Descriptor ?? throw new DraftBuildException($"{path}: FormLinkOrIndex schema is missing link.");
                var unsetLink = (NativeWireFormLinkDraftNode)BuildFormLink(unsetLinkDescriptor, AppendProperty(path, "link"), "link", true, false, null, NativeWireDraftValueState.RequiredUnset);
                return new NativeWireFormLinkOrIndexDraftNode(path, displayName, descriptor, isRequired, isReadOnly, NativeWireDraftValueState.RequiredUnset, false, string.Empty, unsetLink, NativeWireFormLinkOrIndexActiveBranch.Link);
            }

            if (value.Value.ValueKind != JsonValueKind.Object || !value.Value.TryGetProperty("index", out var index) || index.ValueKind is not JsonValueKind.Number and not JsonValueKind.Null || !value.Value.TryGetProperty("link", out var linkValue))
            {
                throw new DraftBuildException($"{path}: expected a FormLinkOrIndex object with integer-or-null index and FormLink link.");
            }

            var linkDescriptor = descriptor.Properties.FirstOrDefault(property => property.Name == "link")?.Descriptor ?? throw new DraftBuildException($"{path}: FormLinkOrIndex schema is missing link.");
            var link = Build(linkDescriptor, AppendProperty(path, "link"), "link", true, false, linkValue, valueState, true, depth + 1) as NativeWireFormLinkDraftNode ?? throw new DraftBuildException($"{path}.link: FormLinkOrIndex link did not resolve to a FormLink node.");
            var usesAlias = value.Value.TryGetProperty("usesAlias", out var alias) && alias.ValueKind == JsonValueKind.True;
            var usesPackage = value.Value.TryGetProperty("usesPackageData", out var package) && package.ValueKind == JsonValueKind.True;
            var active = usesAlias ? NativeWireFormLinkOrIndexActiveBranch.AliasIndex : usesPackage ? NativeWireFormLinkOrIndexActiveBranch.PackageDataIndex : NativeWireFormLinkOrIndexActiveBranch.Link;
            var isIndexNull = index.ValueKind == JsonValueKind.Null;
            return new NativeWireFormLinkOrIndexDraftNode(path, displayName, descriptor, isRequired, isReadOnly, valueState, isIndexNull, isIndexNull ? string.Empty : index.GetRawText(), link, active);
        }

        /// <summary>Finds one seeded union option by exact discriminator, null identity, or a uniquely admitted top-level JSON token kind.</summary>
        /// <param name="descriptor">The resolved union descriptor containing every candidate alternative.</param>
        /// <param name="value">The exact seeded JSON value whose union identity must be preserved.</param>
        /// <param name="path">The current structural path used in deterministic failures.</param>
        /// <returns>The single schema alternative selected without value inference.</returns>
        /// <exception cref="DraftBuildException">Thrown when a discriminator is invalid, an option cannot resolve, or zero or multiple alternatives admit the value.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested during bounded alternative resolution.</exception>
        private NativeWireSchemaUnionOption FindSeededUnionOption(NativeWireSchemaDescriptor descriptor, JsonElement value, string path)
        {
            if (value.ValueKind == JsonValueKind.Object && value.TryGetProperty("$type", out var discriminator) && discriminator.ValueKind == JsonValueKind.String)
            {
                var exact = discriminator.GetString()!;
                var typeName = exact.Contains(':') ? exact[(exact.LastIndexOf(':') + 1)..] : exact;
                var matches = descriptor.UnionOptions.Where(option => string.Equals(option.Discriminator, exact, StringComparison.Ordinal) || string.Equals(option.Key, typeName, StringComparison.Ordinal) || string.Equals(option.Discriminator, typeName, StringComparison.Ordinal)).ToArray();
                if (matches.Length == 1)
                {
                    return matches[0];
                }

                throw new DraftBuildException($"{path}.$type: discriminator '{exact}' resolves to {matches.Length} alternatives in the active catalog.");
            }

            if (value.ValueKind == JsonValueKind.Null)
            {
                var nullOption = descriptor.UnionOptions.Where(option => option.DisplayName.Contains("null", StringComparison.OrdinalIgnoreCase)).ToArray();
                if (nullOption.Length == 1)
                {
                    return nullOption[0];
                }
            }

            if (descriptor.UnionOptions.Count == 1)
            {
                return descriptor.UnionOptions[0];
            }

            var kindMatches = new List<NativeWireSchemaUnionOption>();
            foreach (var option in descriptor.UnionOptions)
            {
                CancellationToken.ThrowIfCancellationRequested();
                var resolved = option.Resolve(Limits, CancellationToken);
                if (!resolved.Succeeded || resolved.Value is null)
                {
                    throw new DraftBuildException(resolved.Error?.Message ?? $"{path}: union option '{option.Key}' could not be resolved for exact JSON-kind selection.");
                }

                if (AcceptsJsonKind(resolved.Value, value.ValueKind))
                {
                    kindMatches.Add(option);
                }
            }

            if (kindMatches.Count == 1)
            {
                return kindMatches[0];
            }

            throw new DraftBuildException($"{path}: seeded union value has no exact discriminator and its {value.ValueKind} token matches {kindMatches.Count} alternatives; selection would require inference.");
        }

        /// <summary>Determines whether one resolved alternative admits the exact top-level JSON token kind.</summary>
        /// <param name="descriptor">The resolved alternative descriptor.</param>
        /// <param name="valueKind">The exact seeded JSON token kind.</param>
        /// <returns><see langword="true"/> only when the descriptor admits that token kind without inspecting or guessing its value.</returns>
        private static bool AcceptsJsonKind(NativeWireSchemaDescriptor descriptor, JsonValueKind valueKind)
        {
            return descriptor.Kind switch
            {
                NativeWireSchemaValueKind.Object or
                NativeWireSchemaValueKind.FormLink or
                NativeWireSchemaValueKind.FormLinkOrIndex or
                NativeWireSchemaValueKind.FloatBits or
                NativeWireSchemaValueKind.ByteArray or
                NativeWireSchemaValueKind.TranslatedString or
                NativeWireSchemaValueKind.Asset or
                NativeWireSchemaValueKind.Color or
                NativeWireSchemaValueKind.Array2D => valueKind == JsonValueKind.Object,
                NativeWireSchemaValueKind.Array => valueKind == JsonValueKind.Array,
                NativeWireSchemaValueKind.Boolean => valueKind is JsonValueKind.True or JsonValueKind.False,
                NativeWireSchemaValueKind.Integer or NativeWireSchemaValueKind.Enum => valueKind == JsonValueKind.Number,
                NativeWireSchemaValueKind.String => valueKind == (descriptor.Annotations.ContainsKey("x-presentation-json-number") ? JsonValueKind.Number : JsonValueKind.String),
                NativeWireSchemaValueKind.Nullable => valueKind == JsonValueKind.Null || descriptor.NonNullDescriptor is not null && AcceptsJsonKind(descriptor.NonNullDescriptor, valueKind),
                _ => false,
            };
        }

        /// <summary>Rejects unexpected properties in a closed object.</summary>
        private static void ValidateClosedObject(NativeWireSchemaDescriptor descriptor, JsonElement? value, string path)
        {
            if (!value.HasValue || value.Value.ValueKind != JsonValueKind.Object)
            {
                return;
            }

            var admitted = descriptor.Properties.Select(property => property.Name).ToHashSet(StringComparer.Ordinal);
            foreach (var property in value.Value.EnumerateObject())
            {
                if (!admitted.Contains(property.Name))
                {
                    throw new DraftBuildException($"{AppendProperty(path, property.Name)}: property is not admitted by the closed schema.");
                }
            }
        }

        /// <summary>Maps a schema value kind to its specialized object draft kind.</summary>
        private static NativeWireDraftNodeKind ToDraftKind(NativeWireSchemaValueKind kind)
        {
            return kind switch
            {
                NativeWireSchemaValueKind.FloatBits => NativeWireDraftNodeKind.FloatBits,
                NativeWireSchemaValueKind.ByteArray => NativeWireDraftNodeKind.ByteArray,
                NativeWireSchemaValueKind.TranslatedString => NativeWireDraftNodeKind.TranslatedString,
                NativeWireSchemaValueKind.Asset => NativeWireDraftNodeKind.Asset,
                NativeWireSchemaValueKind.Color => NativeWireDraftNodeKind.Color,
                NativeWireSchemaValueKind.Array2D => NativeWireDraftNodeKind.Array2D,
                _ => NativeWireDraftNodeKind.Object,
            };
        }

        /// <summary>Counts one materialized node and enforces depth and node limits.</summary>
        private void Visit(string path, int depth)
        {
            CancellationToken.ThrowIfCancellationRequested();
            if (depth > Limits.MaximumDepth)
            {
                throw new DraftBuildException($"{path}: draft materialization exceeds maximum depth {Limits.MaximumDepth}.");
            }

            if (++MaterializedNodes > Limits.MaximumNodes)
            {
                throw new DraftBuildException($"{path}: draft materialization exceeds maximum node count {Limits.MaximumNodes}.");
            }
        }
    }

    /// <summary>Counts detached seed values under exact operation limits.</summary>
    private sealed class JsonLimitCounter
    {
        /// <summary>The operation limits.</summary>
        private readonly NativeWireReadLimits Limits;

        /// <summary>The operation cancellation token.</summary>
        private readonly CancellationToken CancellationToken;

        /// <summary>The number of visited JSON values.</summary>
        private int Nodes;

        /// <summary>Initializes one bounded JSON traversal.</summary>
        internal JsonLimitCounter(NativeWireReadLimits limits, CancellationToken cancellationToken)
        {
            Limits = limits;
            CancellationToken = cancellationToken;
        }

        /// <summary>Visits one detached JSON value recursively.</summary>
        internal void Visit(JsonElement value, string path, int depth)
        {
            CancellationToken.ThrowIfCancellationRequested();
            if (depth > Limits.MaximumDepth)
            {
                throw new DraftBuildException($"{path}: seed capture exceeds maximum depth {Limits.MaximumDepth}.");
            }

            if (++Nodes > Limits.MaximumNodes)
            {
                throw new DraftBuildException($"{path}: seed capture exceeds maximum node count {Limits.MaximumNodes}.");
            }

            if (value.ValueKind == JsonValueKind.String && (value.GetString()?.Length ?? 0) > Limits.MaximumStringLength)
            {
                throw new DraftBuildException($"{path}: seed string exceeds maximum length {Limits.MaximumStringLength}.");
            }

            if (value.ValueKind == JsonValueKind.Array)
            {
                if (value.GetArrayLength() > Limits.MaximumArrayElements)
                {
                    throw new DraftBuildException($"{path}: seed array contains {value.GetArrayLength()} elements, exceeding maximum {Limits.MaximumArrayElements}.");
                }

                var index = 0;
                foreach (var item in value.EnumerateArray())
                {
                    Visit(item, $"{path}[{index++}]", depth + 1);
                }
            }
            else if (value.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in value.EnumerateObject())
                {
                    Visit(property.Value, AppendProperty(path, property.Name), depth + 1);
                }
            }
        }
    }

    /// <summary>Creates one typed node construction failure.</summary>
    private static EngineResult<NativeWireDraftNode> FailureNode(string message)
    {
        return EngineResult<NativeWireDraftNode>.Failure(new EngineError(EngineErrorCode.ValidationFailed, message));
    }

    /// <summary>Appends one exact object property to a root-based JSON path.</summary>
    private static string AppendProperty(string path, string property)
    {
        return property.All(character => char.IsLetterOrDigit(character) || character is '_' or '$')
            ? $"{path}.{property}"
            : $"{path}['{property.Replace("'", "\\'", StringComparison.Ordinal)}']";
    }

    /// <summary>Represents one path-specific seed or draft construction failure.</summary>
    private sealed class DraftBuildException : Exception
    {
        /// <summary>Initializes one construction failure.</summary>
        internal DraftBuildException(string message)
            : base(message)
        {
        }
    }
}

using System.Buffers;
using System.Globalization;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.RecordEditing.Schema;
using CreationsForge.Services;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Creates bounded typed command drafts from exact schemas and revision-bound detached record seeds.</summary>
public sealed class FormListDraftFactory : IFormListDraftFactory
{
    /// <inheritdoc />
    public EngineResult<FormListDraftSeed> CaptureSeed(
        FormListWireCatalogContext context,
        Guid workspaceId,
        WorkspaceRevision revision,
        FormListContext recordContext,
        JsonElement record,
        RecordWireReadLimits limits,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(recordContext);
        ArgumentNullException.ThrowIfNull(limits);
        cancellationToken.ThrowIfCancellationRequested();
        if (workspaceId == Guid.Empty)
        {
            return FailureSeed("$: record editor seed capture requires a non-empty workspace identifier.");
        }

        if (record.ValueKind != JsonValueKind.Object)
        {
            return FailureSeed("$: record editor seed capture requires one detached expanded record object.");
        }

        if (context.Identity.Game != context.Codec.Game || context.Identity.Release != context.Codec.Release)
        {
            return FailureSeed("$: record editor seed capture rejected a mismatched schema catalog and codec context.");
        }

        try
        {
            var counter = new JsonLimitCounter(limits, cancellationToken);
            counter.Visit(record, "$", 1);
            if (!record.TryGetProperty("FormKey", out var formKey) || formKey.ValueKind != JsonValueKind.String || !string.Equals(formKey.GetString(), recordContext.Selection.FormKey.ToString(), StringComparison.Ordinal))
            {
                return FailureSeed("$.FormKey: detached record identity does not match the selected record context.");
            }

            return EngineResult<FormListDraftSeed>.Success(
                new FormListDraftSeed(workspaceId, revision, recordContext, context.Identity, record));
        }
        catch (DraftBuildException exception)
        {
            return FailureSeed(exception.Message);
        }
    }

    /// <inheritdoc />
    public EngineResult<FormListDraft> Create(
        FormListWireCatalogContext context,
        RecordWireSchemaNodeKey commandKey,
        FormListDraftSeed? seed,
        FormListDraftSeedSelection selection,
        RecordWireReadLimits limits,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(commandKey);
        ArgumentNullException.ThrowIfNull(selection);
        ArgumentNullException.ThrowIfNull(limits);
        cancellationToken.ThrowIfCancellationRequested();
        if (commandKey.Kind != RecordWireSchemaNodeKind.Command || !string.Equals(commandKey.CatalogId, context.Identity.CatalogId, StringComparison.Ordinal))
        {
            return FailureDraft($"$: command key '{commandKey.Name}' does not belong to active catalog {context.Identity.CatalogId}.");
        }

        if (seed is not null && !CatalogIdentityEquals(seed.CatalogIdentity, context.Identity))
        {
            return FailureDraft("$: revision-bound seed catalog identity does not match the active editor catalog.");
        }

        var policyResult = FormListCommandSeedCatalog.Resolve(context, commandKey.Name);
        if (!policyResult.Succeeded || policyResult.Value is null)
        {
            return FailureDraft(policyResult.Error?.Message ?? $"$: command '{commandKey.Name}' has no seed policy.");
        }

        var schemaResult = new RecordWireSchemaIndex(context).Resolve(commandKey, limits, cancellationToken);
        if (!schemaResult.Succeeded || schemaResult.Value is null)
        {
            return FailureDraft(schemaResult.Error?.Message ?? $"$: command schema '{commandKey.Name}' could not be resolved.");
        }

        try
        {
            var arguments = CreateSeedArguments(policyResult.Value, seed, selection, cancellationToken);
            JsonElement? initialValue = arguments;
            var initialState = arguments.HasValue ? RecordWireDraftValueState.Seeded : RecordWireDraftValueState.Defaulted;
            if (!initialValue.HasValue && schemaResult.Value.DefaultTemplate.HasValue)
            {
                initialValue = schemaResult.Value.DefaultTemplate.Value;
                initialState = RecordWireDraftValueState.Defaulted;
            }

            var state = new DraftBuildState(limits, cancellationToken);
            var root = state.Build(schemaResult.Value, "$", commandKey.Name, true, false, initialValue, initialState, false, 1);
            if (root is not RecordWireObjectDraftNode)
            {
                return FailureDraft($"$: command '{commandKey.Name}' schema did not resolve to a closed object draft.");
            }

            return EngineResult<FormListDraft>.Success(new FormListDraft(commandKey, context.Identity, seed, root, limits));
        }
        catch (DraftBuildException exception)
        {
            return FailureDraft(exception.Message);
        }
    }

    /// <summary>Creates the fixed detached command seed selected by one admitted policy.</summary>
    private static JsonElement? CreateSeedArguments(
        FormListCommandSeedPolicy policy,
        FormListDraftSeed? seed,
        FormListDraftSeedSelection selection,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return policy.Kind switch
        {
            FormListCommandSeedPolicyKind.EmptyArguments => ParseArguments("{}"),
            FormListCommandSeedPolicyKind.DirectRecordProperty => CreateDirectPropertyArguments(RequireSeed(seed, policy), policy.RecordProperty!, policy.ArgumentProperty!),
            FormListCommandSeedPolicyKind.DerivedCompressed => CreateCompressedArguments(RequireSeed(seed, policy)),
            FormListCommandSeedPolicyKind.DerivedDeleted => CreateDeletedArguments(RequireSeed(seed, policy)),
            FormListCommandSeedPolicyKind.CompleteCollection => CreateDirectPropertyArguments(RequireSeed(seed, policy), policy.RecordProperty!, policy.ArgumentProperty!),
            FormListCommandSeedPolicyKind.SelectedExistingIndex => CreateIndexArguments(RequireSeed(seed, policy), policy, selection, true),
            FormListCommandSeedPolicyKind.SelectedInsertionIndex => CreateIndexArguments(seed, policy, selection, false),
            FormListCommandSeedPolicyKind.ExplicitMoveIndices => CreateMoveArguments(RequireSeed(seed, policy), policy, selection),
            FormListCommandSeedPolicyKind.SelectedExistingComponent => CreateComponentArguments(RequireSeed(seed, policy), policy, selection),
            _ => throw new DraftBuildException($"$: unsupported command seed policy {policy.Kind}.")
        };
    }

    /// <summary>Requires a matching revision-bound seed for a value-preserving command.</summary>
    private static FormListDraftSeed RequireSeed(FormListDraftSeed? seed, FormListCommandSeedPolicy policy)
    {
        return seed ?? throw new DraftBuildException($"$: command '{policy.CommandName}' requires a matching revision-bound record seed.");
    }

    /// <summary>Copies one exact record property into one exact command argument.</summary>
    private static JsonElement CreateDirectPropertyArguments(FormListDraftSeed seed, string recordProperty, string argumentProperty)
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

    /// <summary>Derives the compressed command value from the exact record raw flag bit.</summary>
    private static JsonElement CreateCompressedArguments(FormListDraftSeed seed)
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
    private static JsonElement CreateDeletedArguments(FormListDraftSeed seed)
    {
        return WriteArguments(writer =>
        {
            writer.WriteStartObject();
            writer.WriteBoolean("isDeleted", seed.RecordContext.Status == ReferenceResolutionStatus.Deleted);
            writer.WriteEndObject();
        });
    }

    /// <summary>Creates selected existing or insertion index arguments.</summary>
    private static JsonElement CreateIndexArguments(FormListDraftSeed? seed, FormListCommandSeedPolicy policy, FormListDraftSeedSelection selection, bool requireExisting)
    {
        var expectedKind = requireExisting ? FormListDraftSeedSelectionKind.AtIndex : FormListDraftSeedSelectionKind.InsertAt;
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
    private static JsonElement CreateMoveArguments(FormListDraftSeed seed, FormListCommandSeedPolicy policy, FormListDraftSeedSelection selection)
    {
        if (selection.Kind != FormListDraftSeedSelectionKind.Move || !selection.SourceIndex.HasValue || !selection.DestinationIndex.HasValue)
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
    private static JsonElement CreateComponentArguments(FormListDraftSeed seed, FormListCommandSeedPolicy policy, FormListDraftSeedSelection selection)
    {
        if (selection.Kind != FormListDraftSeedSelectionKind.AtIndex || !selection.Index.HasValue)
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
    private static JsonElement RequireArray(FormListDraftSeed seed, string propertyName)
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
    private static bool CatalogIdentityEquals(RecordWireSchemaCatalogIdentity left, RecordWireSchemaCatalogIdentity right)
    {
        return left.Game == right.Game && left.Release == right.Release &&
            string.Equals(left.SchemaVersion, right.SchemaVersion, StringComparison.Ordinal) &&
            string.Equals(left.CatalogId, right.CatalogId, StringComparison.Ordinal);
    }

    /// <summary>Creates a typed seed failure.</summary>
    private static EngineResult<FormListDraftSeed> FailureSeed(string message)
    {
        return EngineResult<FormListDraftSeed>.Failure(new EngineError(EngineErrorCode.ValidationFailed, message));
    }

    /// <summary>Creates a typed draft failure.</summary>
    private static EngineResult<FormListDraft> FailureDraft(string message)
    {
        return EngineResult<FormListDraft>.Failure(new EngineError(EngineErrorCode.ValidationFailed, message));
    }

    /// <summary>Builds typed nodes while enforcing the operation's limits.</summary>
    internal sealed class DraftBuildState
    {
        /// <summary>The operation limits.</summary>
        private readonly RecordWireReadLimits Limits;

        /// <summary>The operation cancellation token.</summary>
        private readonly CancellationToken CancellationToken;

        /// <summary>The number of materialized values.</summary>
        private int MaterializedNodes;

        /// <summary>Initializes one bounded builder.</summary>
        internal DraftBuildState(RecordWireReadLimits limits, CancellationToken cancellationToken)
        {
            Limits = limits;
            CancellationToken = cancellationToken;
        }

        /// <summary>Builds one typed node from a descriptor and optional current value.</summary>
        internal RecordWireDraftNode Build(
            RecordWireSchemaDescriptor descriptor,
            string path,
            string displayName,
            bool isRequired,
            bool isReadOnly,
            JsonElement? value,
            RecordWireDraftValueState valueState,
            bool strictSeed,
            int depth)
        {
            Visit(path, depth);
            var hasValue = value.HasValue && value.Value.ValueKind != JsonValueKind.Undefined;
            if (hasValue && value.GetValueOrDefault().ValueKind == JsonValueKind.Null && descriptor.Kind != RecordWireSchemaValueKind.Nullable)
            {
                return BuildUnset(descriptor, path, displayName, isRequired, isReadOnly, depth);
            }

            if (!hasValue && descriptor.Constant.HasValue)
            {
                value = descriptor.Constant.Value;
                hasValue = true;
                valueState = RecordWireDraftValueState.SchemaConstant;
            }
            else if (!hasValue && descriptor.DefaultValue.HasValue)
            {
                value = descriptor.DefaultValue.Value;
                hasValue = true;
                valueState = RecordWireDraftValueState.Defaulted;
            }

            return descriptor.Kind switch
            {
                RecordWireSchemaValueKind.Object => BuildObject(descriptor, RecordWireDraftNodeKind.Object, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                RecordWireSchemaValueKind.Array => BuildArray(descriptor, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                RecordWireSchemaValueKind.Union => BuildUnion(descriptor, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                RecordWireSchemaValueKind.Nullable => BuildNullable(descriptor, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                RecordWireSchemaValueKind.Boolean => BuildBoolean(descriptor, path, displayName, isRequired, isReadOnly, value, valueState),
                RecordWireSchemaValueKind.Integer => BuildInteger(descriptor, path, displayName, isRequired, isReadOnly, value, valueState),
                RecordWireSchemaValueKind.String => BuildString(descriptor, RecordWireDraftNodeKind.String, path, displayName, isRequired, isReadOnly, value, valueState),
                RecordWireSchemaValueKind.Enum => BuildEnum(descriptor, path, displayName, isRequired, isReadOnly, value, valueState),
                RecordWireSchemaValueKind.FormLink => BuildFormLink(descriptor, path, displayName, isRequired, isReadOnly, value, valueState),
                RecordWireSchemaValueKind.FormLinkOrIndex => BuildFormLinkOrIndex(descriptor, path, displayName, isRequired, isReadOnly, value, valueState, depth),
                RecordWireSchemaValueKind.FloatBits => BuildSpecialObject(descriptor, RecordWireDraftNodeKind.FloatBits, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                RecordWireSchemaValueKind.ByteArray => BuildSpecialObject(descriptor, RecordWireDraftNodeKind.ByteArray, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                RecordWireSchemaValueKind.TranslatedString => BuildSpecialObject(descriptor, RecordWireDraftNodeKind.TranslatedString, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                RecordWireSchemaValueKind.Asset => BuildSpecialObject(descriptor, RecordWireDraftNodeKind.Asset, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                RecordWireSchemaValueKind.Color => BuildSpecialObject(descriptor, RecordWireDraftNodeKind.Color, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                RecordWireSchemaValueKind.Array2D => BuildSpecialObject(descriptor, RecordWireDraftNodeKind.Array2D, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth),
                _ => throw new DraftBuildException($"{path}: unsupported typed draft shape {descriptor.Kind}.")
            };
        }

        /// <summary>Builds a placeholder whose required value has no honest default.</summary>
        private RecordWireDraftNode BuildUnset(RecordWireSchemaDescriptor descriptor, string path, string displayName, bool isRequired, bool isReadOnly, int depth)
        {
            return descriptor.Kind switch
            {
                RecordWireSchemaValueKind.Object => BuildObject(descriptor, RecordWireDraftNodeKind.Object, path, displayName, isRequired, isReadOnly, null, RecordWireDraftValueState.RequiredUnset, false, depth),
                RecordWireSchemaValueKind.Array => BuildArray(descriptor, path, displayName, isRequired, isReadOnly, null, RecordWireDraftValueState.RequiredUnset, false, depth),
                RecordWireSchemaValueKind.Union => BuildUnion(descriptor, path, displayName, isRequired, isReadOnly, null, RecordWireDraftValueState.RequiredUnset, false, depth),
                RecordWireSchemaValueKind.Nullable => BuildNullable(descriptor, path, displayName, isRequired, isReadOnly, null, RecordWireDraftValueState.RequiredUnset, false, depth),
                RecordWireSchemaValueKind.Boolean => new RecordWireBooleanDraftNode(path, displayName, descriptor, isRequired, isReadOnly, RecordWireDraftValueState.RequiredUnset, false),
                RecordWireSchemaValueKind.Integer => new RecordWireIntegerDraftNode(RecordWireDraftNodeKind.Integer, path, displayName, descriptor, isRequired, isReadOnly, RecordWireDraftValueState.RequiredUnset, "0"),
                RecordWireSchemaValueKind.String => new RecordWireStringDraftNode(RecordWireDraftNodeKind.String, path, displayName, descriptor, isRequired, isReadOnly, RecordWireDraftValueState.RequiredUnset, string.Empty),
                RecordWireSchemaValueKind.Enum => new RecordWireEnumDraftNode(path, displayName, descriptor, isRequired, isReadOnly, RecordWireDraftValueState.RequiredUnset, "0"),
                RecordWireSchemaValueKind.FormLink => new RecordWireFormLinkDraftNode(path, displayName, descriptor, isRequired, isReadOnly, RecordWireDraftValueState.RequiredUnset, false, null),
                RecordWireSchemaValueKind.ByteArray => BuildSpecialObject(descriptor, RecordWireDraftNodeKind.ByteArray, path, displayName, isRequired, isReadOnly, null, RecordWireDraftValueState.RequiredUnset, false, depth),
                RecordWireSchemaValueKind.FormLinkOrIndex => BuildFormLinkOrIndex(descriptor, path, displayName, isRequired, isReadOnly, null, RecordWireDraftValueState.RequiredUnset, depth),
                _ => BuildSpecialObject(descriptor, ToDraftKind(descriptor.Kind), path, displayName, isRequired, isReadOnly, null, RecordWireDraftValueState.RequiredUnset, false, depth),
            };
        }

        /// <summary>Builds one ordinary or specialized object and preserves property order.</summary>
        private RecordWireDraftNode BuildObject(RecordWireSchemaDescriptor descriptor, RecordWireDraftNodeKind kind, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, RecordWireDraftValueState valueState, bool strictSeed, int depth)
        {
            if (value.HasValue && value.Value.ValueKind != JsonValueKind.Object)
            {
                throw new DraftBuildException($"{path}: expected an object, found {value.Value.ValueKind}.");
            }

            ValidateClosedObject(descriptor, value, path);
            var properties = new List<RecordWireObjectDraftProperty>();
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

                var childState = present ? valueState : RecordWireDraftValueState.RequiredUnset;
                var child = Build(property.Descriptor, propertyPath, property.Name, property.IsRequired, property.IsReadOnly, present ? propertyValue : null, childState, present && valueState == RecordWireDraftValueState.Seeded, depth + 1);
                properties.Add(new RecordWireObjectDraftProperty(property.Name, child));
            }

            var objectState = valueState;
            var objectNode = kind switch
            {
                RecordWireDraftNodeKind.Object => new RecordWireObjectDraftNode(kind, path, displayName, descriptor, isRequired, isReadOnly, objectState, properties),
                RecordWireDraftNodeKind.FloatBits => new RecordWireFloatBitsDraftNode(path, displayName, descriptor, isRequired, isReadOnly, objectState, properties),
                RecordWireDraftNodeKind.ByteArray => new RecordWireByteArrayDraftNode(path, displayName, descriptor, isRequired, isReadOnly, objectState, properties),
                RecordWireDraftNodeKind.TranslatedString => new RecordWireTranslatedStringDraftNode(path, displayName, descriptor, isRequired, isReadOnly, objectState, properties),
                RecordWireDraftNodeKind.Asset => new RecordWireAssetDraftNode(path, displayName, descriptor, isRequired, isReadOnly, objectState, properties),
                RecordWireDraftNodeKind.Color => new RecordWireColorDraftNode(path, displayName, descriptor, isRequired, isReadOnly, objectState, properties),
                RecordWireDraftNodeKind.Array2D => new RecordWireArray2DDraftNode(path, displayName, descriptor, isRequired, isReadOnly, objectState, properties),
                _ => throw new DraftBuildException($"{path}: unsupported object draft kind {kind}.")
            };
            BindOwnerDerivedModes(objectNode);
            if (objectNode is RecordWireAssetDraftNode asset &&
                asset.Descriptor.Annotations.TryGetValue("x-record-wrapper-type", out var wrapperType) &&
                !string.IsNullOrWhiteSpace(wrapperType))
            {
                BindAssetAuthoritativeFields(asset);
            }

            return objectNode;
        }

        /// <summary>Binds annotated direct FormLink-or-index values to their exact owner-mode siblings.</summary>
        /// <param name="objectNode">The complete constructed containing object.</param>
        private static void BindOwnerDerivedModes(RecordWireObjectDraftNode objectNode)
        {
            foreach (var property in objectNode.Properties)
            {
                if (property.Node is not RecordWireFormLinkOrIndexDraftNode formLinkOrIndex ||
                    !property.Node.Descriptor.Annotations.TryGetValue("x-record-owner-derived-mode", out var annotation))
                {
                    continue;
                }

                if (!string.Equals(annotation, "true", StringComparison.Ordinal))
                {
                    throw new DraftBuildException($"{property.Node.Path}: x-record-owner-derived-mode must be the Boolean value true.");
                }

                var usesAliases = objectNode.FindProperty("UseAliases") as RecordWireBooleanDraftNode
                    ?? throw new DraftBuildException($"{property.Node.Path}: owner-derived mode requires a direct Boolean UseAliases sibling.");
                var usesPackageData = objectNode.FindProperty("UsePackageData") as RecordWireBooleanDraftNode
                    ?? throw new DraftBuildException($"{property.Node.Path}: owner-derived mode requires a direct Boolean UsePackageData sibling.");
                formLinkOrIndex.BindOwnerMode(usesAliases, usesPackageData);
            }
        }

        /// <summary>Binds one asset wrapper's exact editable authority and optional read-only projections.</summary>
        /// <param name="asset">The constructed specialized asset object.</param>
        private static void BindAssetAuthoritativeFields(RecordWireAssetDraftNode asset)
        {
            var value = asset.FindProperty("value") as RecordWireObjectDraftNode
                ?? throw new DraftBuildException($"{asset.Path}.value: asset schema requires a direct object value property.");
            var isNull = value.FindProperty("isNull") as RecordWireBooleanDraftNode
                ?? throw new DraftBuildException($"{value.Path}.isNull: asset schema requires a direct Boolean authority.");
            var givenPath = value.FindProperty("givenPath") as RecordWireStringDraftNode
                ?? throw new DraftBuildException($"{value.Path}.givenPath: asset schema requires a direct string authority.");
            var dataRelativePath = RequireOptionalReadOnlyAssetProjection(value, "dataRelativePath");
            var extension = RequireOptionalReadOnlyAssetProjection(value, "extension");
            asset.BindAuthoritativeFields(isNull, givenPath, new[] { dataRelativePath, extension });
        }

        /// <summary>Requires one optional read-only asset projection with its exact direct name.</summary>
        /// <param name="value">The asset value object.</param>
        /// <param name="propertyName">The exact projection property name.</param>
        /// <returns>The validated projection node.</returns>
        private static RecordWireDraftNode RequireOptionalReadOnlyAssetProjection(RecordWireObjectDraftNode value, string propertyName)
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
        private RecordWireDraftNode BuildSpecialObject(RecordWireSchemaDescriptor descriptor, RecordWireDraftNodeKind kind, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, RecordWireDraftValueState valueState, bool strictSeed, int depth)
        {
            return BuildObject(descriptor, kind, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth);
        }

        /// <summary>Builds one ordered array without normalizing duplicates.</summary>
        private RecordWireDraftNode BuildArray(RecordWireSchemaDescriptor descriptor, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, RecordWireDraftValueState valueState, bool strictSeed, int depth)
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

            var items = new List<RecordWireDraftNode>(count);
            if (value.HasValue)
            {
                var index = 0;
                foreach (var item in value.Value.EnumerateArray())
                {
                    items.Add(Build(descriptor.ItemDescriptor, $"{path}[{index}]", $"{displayName} {index + 1}", true, false, item, valueState, strictSeed, depth + 1));
                    index++;
                }
            }

            var array = new RecordWireArrayDraftNode(
                path,
                displayName,
                descriptor,
                isRequired,
                isReadOnly,
                value.HasValue ? valueState : RecordWireDraftValueState.RequiredUnset,
                items,
                (index, token) =>
                {
                    try
                    {
                        token.ThrowIfCancellationRequested();
                        var builder = new DraftBuildState(Limits, token);
                        var initial = descriptor.ItemDescriptor.DefaultTemplate;
                        var initialState = initial.HasValue ? RecordWireDraftValueState.Defaulted : RecordWireDraftValueState.RequiredUnset;
                        var created = builder.Build(descriptor.ItemDescriptor, $"{path}[{index}]", $"{displayName} {index + 1}", true, false, initial, initialState, false, depth + 1);
                        return EngineResult<RecordWireDraftNode>.Success(created);
                    }
                    catch (DraftBuildException exception)
                    {
                        return FailureNode(exception.Message);
                    }
                });
            return array;
        }

        /// <summary>Builds one lazy union and materializes only a seeded exact discriminator match.</summary>
        private RecordWireDraftNode BuildUnion(RecordWireSchemaDescriptor descriptor, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, RecordWireDraftValueState valueState, bool strictSeed, int depth)
        {
            RecordWireSchemaUnionOption? selectedOption = null;
            RecordWireDraftNode? selectedValue = null;
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

            return new RecordWireUnionDraftNode(
                path,
                displayName,
                descriptor,
                isRequired,
                isReadOnly,
                value.HasValue ? valueState : RecordWireDraftValueState.RequiredUnset,
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
                        var initialState = initial.HasValue ? RecordWireDraftValueState.Defaulted : RecordWireDraftValueState.RequiredUnset;
                        var created = builder.Build(resolved.Value, currentPath, option.DisplayName, isRequired, isReadOnly, initial, initialState, false, depth + 1);
                        return EngineResult<RecordWireDraftNode>.Success(created);
                    }
                    catch (DraftBuildException exception)
                    {
                        return FailureNode(exception.Message);
                    }
                });
        }

        /// <summary>Builds one whole-value nullable wrapper without collapsing nested FormLink null identities.</summary>
        private RecordWireDraftNode BuildNullable(RecordWireSchemaDescriptor descriptor, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, RecordWireDraftValueState valueState, bool strictSeed, int depth)
        {
            if (descriptor.NonNullDescriptor is null)
            {
                throw new DraftBuildException($"{path}: nullable descriptor has no non-null shape.");
            }

            var isNull = value.HasValue && value.Value.ValueKind == JsonValueKind.Null;
            RecordWireDraftNode? presentValue = null;
            if (value.HasValue && !isNull)
            {
                presentValue = Build(descriptor.NonNullDescriptor, path, displayName, isRequired, isReadOnly, value, valueState, strictSeed, depth + 1);
            }

            return new RecordWireNullableDraftNode(
                path,
                displayName,
                descriptor,
                isRequired,
                isReadOnly,
                value.HasValue ? valueState : RecordWireDraftValueState.RequiredUnset,
                isNull,
                presentValue,
                (currentPath, token) =>
                {
                    try
                    {
                        var builder = new DraftBuildState(Limits, token);
                        var initial = descriptor.NonNullDescriptor.DefaultTemplate;
                        var initialState = initial.HasValue ? RecordWireDraftValueState.Defaulted : RecordWireDraftValueState.RequiredUnset;
                        var created = builder.Build(descriptor.NonNullDescriptor, currentPath, displayName, isRequired, isReadOnly, initial, initialState, false, depth + 1);
                        return EngineResult<RecordWireDraftNode>.Success(created);
                    }
                    catch (DraftBuildException exception)
                    {
                        return FailureNode(exception.Message);
                    }
                });
        }

        /// <summary>Builds one Boolean value.</summary>
        private static RecordWireDraftNode BuildBoolean(RecordWireSchemaDescriptor descriptor, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, RecordWireDraftValueState valueState)
        {
            if (value.HasValue && value.Value.ValueKind is not JsonValueKind.True and not JsonValueKind.False)
            {
                throw new DraftBuildException($"{path}: expected a Boolean, found {value.Value.ValueKind}.");
            }

            return new RecordWireBooleanDraftNode(path, displayName, descriptor, isRequired, isReadOnly, value.HasValue ? valueState : RecordWireDraftValueState.RequiredUnset, value?.GetBoolean() ?? false);
        }

        /// <summary>Builds one exact integer value.</summary>
        private static RecordWireDraftNode BuildInteger(RecordWireSchemaDescriptor descriptor, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, RecordWireDraftValueState valueState)
        {
            if (value.HasValue && value.Value.ValueKind != JsonValueKind.Number)
            {
                throw new DraftBuildException($"{path}: expected an integer, found {value.Value.ValueKind}.");
            }

            return new RecordWireIntegerDraftNode(RecordWireDraftNodeKind.Integer, path, displayName, descriptor, isRequired, isReadOnly, value.HasValue ? valueState : RecordWireDraftValueState.RequiredUnset, value?.GetRawText() ?? "0");
        }

        /// <summary>Builds one exact known-or-unknown enum integer value.</summary>
        private static RecordWireDraftNode BuildEnum(RecordWireSchemaDescriptor descriptor, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, RecordWireDraftValueState valueState)
        {
            if (value.HasValue && value.Value.ValueKind != JsonValueKind.Number)
            {
                throw new DraftBuildException($"{path}: expected an enum integer, found {value.Value.ValueKind}.");
            }

            return new RecordWireEnumDraftNode(path, displayName, descriptor, isRequired, isReadOnly, value.HasValue ? valueState : RecordWireDraftValueState.RequiredUnset, value?.GetRawText() ?? "0");
        }

        /// <summary>Builds one exact bounded string or Base64 text value.</summary>
        private RecordWireDraftNode BuildString(RecordWireSchemaDescriptor descriptor, RecordWireDraftNodeKind kind, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, RecordWireDraftValueState valueState)
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

            return new RecordWireStringDraftNode(kind, path, displayName, descriptor, isRequired, isReadOnly, value.HasValue ? valueState : RecordWireDraftValueState.RequiredUnset, text);
        }

        /// <summary>Builds one specialized FormLink wrapper.</summary>
        private static RecordWireDraftNode BuildFormLink(RecordWireSchemaDescriptor descriptor, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, RecordWireDraftValueState valueState)
        {
            if (!value.HasValue)
            {
                return new RecordWireFormLinkDraftNode(path, displayName, descriptor, isRequired, isReadOnly, RecordWireDraftValueState.RequiredUnset, false, null);
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

            return new RecordWireFormLinkDraftNode(path, displayName, descriptor, isRequired, isReadOnly, valueState, isNull.GetBoolean(), formKey.ValueKind == JsonValueKind.Null ? null : formKey.GetString(), hasType ? type.GetString() : null);
        }

        /// <summary>Builds one owner-mode-sensitive FormLink-or-index representation.</summary>
        private RecordWireDraftNode BuildFormLinkOrIndex(RecordWireSchemaDescriptor descriptor, string path, string displayName, bool isRequired, bool isReadOnly, JsonElement? value, RecordWireDraftValueState valueState, int depth)
        {
            if (!value.HasValue)
            {
                var unsetLinkDescriptor = descriptor.Properties.FirstOrDefault(property => property.Name == "link")?.Descriptor ?? throw new DraftBuildException($"{path}: FormLinkOrIndex schema is missing link.");
                var unsetLink = (RecordWireFormLinkDraftNode)BuildFormLink(unsetLinkDescriptor, AppendProperty(path, "link"), "link", true, false, null, RecordWireDraftValueState.RequiredUnset);
                return new RecordWireFormLinkOrIndexDraftNode(path, displayName, descriptor, isRequired, isReadOnly, RecordWireDraftValueState.RequiredUnset, false, string.Empty, unsetLink, RecordWireFormLinkOrIndexActiveBranch.Link);
            }

            if (value.Value.ValueKind != JsonValueKind.Object || !value.Value.TryGetProperty("index", out var index) || index.ValueKind is not JsonValueKind.Number and not JsonValueKind.Null || !value.Value.TryGetProperty("link", out var linkValue))
            {
                throw new DraftBuildException($"{path}: expected a FormLinkOrIndex object with integer-or-null index and FormLink link.");
            }

            var linkDescriptor = descriptor.Properties.FirstOrDefault(property => property.Name == "link")?.Descriptor ?? throw new DraftBuildException($"{path}: FormLinkOrIndex schema is missing link.");
            var link = Build(linkDescriptor, AppendProperty(path, "link"), "link", true, false, linkValue, valueState, true, depth + 1) as RecordWireFormLinkDraftNode ?? throw new DraftBuildException($"{path}.link: FormLinkOrIndex link did not resolve to a FormLink node.");
            var usesAlias = value.Value.TryGetProperty("usesAlias", out var alias) && alias.ValueKind == JsonValueKind.True;
            var usesPackage = value.Value.TryGetProperty("usesPackageData", out var package) && package.ValueKind == JsonValueKind.True;
            var active = usesAlias ? RecordWireFormLinkOrIndexActiveBranch.AliasIndex : usesPackage ? RecordWireFormLinkOrIndexActiveBranch.PackageDataIndex : RecordWireFormLinkOrIndexActiveBranch.Link;
            var isIndexNull = index.ValueKind == JsonValueKind.Null;
            return new RecordWireFormLinkOrIndexDraftNode(path, displayName, descriptor, isRequired, isReadOnly, valueState, isIndexNull, isIndexNull ? string.Empty : index.GetRawText(), link, active);
        }

        /// <summary>Finds one seeded union option by exact discriminator, null identity, or a uniquely admitted top-level JSON token kind.</summary>
        /// <param name="descriptor">The resolved union descriptor containing every candidate alternative.</param>
        /// <param name="value">The exact seeded JSON value whose union identity must be preserved.</param>
        /// <param name="path">The current structural path used in deterministic failures.</param>
        /// <returns>The single schema alternative selected without value inference.</returns>
        /// <exception cref="DraftBuildException">Thrown when a discriminator is invalid, an option cannot resolve, or zero or multiple alternatives admit the value.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested during bounded alternative resolution.</exception>
        private RecordWireSchemaUnionOption FindSeededUnionOption(RecordWireSchemaDescriptor descriptor, JsonElement value, string path)
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

            var kindMatches = new List<RecordWireSchemaUnionOption>();
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
        private static bool AcceptsJsonKind(RecordWireSchemaDescriptor descriptor, JsonValueKind valueKind)
        {
            return descriptor.Kind switch
            {
                RecordWireSchemaValueKind.Object or
                RecordWireSchemaValueKind.FormLink or
                RecordWireSchemaValueKind.FormLinkOrIndex or
                RecordWireSchemaValueKind.FloatBits or
                RecordWireSchemaValueKind.ByteArray or
                RecordWireSchemaValueKind.TranslatedString or
                RecordWireSchemaValueKind.Asset or
                RecordWireSchemaValueKind.Color or
                RecordWireSchemaValueKind.Array2D => valueKind == JsonValueKind.Object,
                RecordWireSchemaValueKind.Array => valueKind == JsonValueKind.Array,
                RecordWireSchemaValueKind.Boolean => valueKind is JsonValueKind.True or JsonValueKind.False,
                RecordWireSchemaValueKind.Integer or RecordWireSchemaValueKind.Enum => valueKind == JsonValueKind.Number,
                RecordWireSchemaValueKind.String => valueKind == (descriptor.Annotations.ContainsKey("x-presentation-json-number") ? JsonValueKind.Number : JsonValueKind.String),
                RecordWireSchemaValueKind.Nullable => valueKind == JsonValueKind.Null || descriptor.NonNullDescriptor is not null && AcceptsJsonKind(descriptor.NonNullDescriptor, valueKind),
                _ => false,
            };
        }

        /// <summary>Rejects unexpected properties in a closed object.</summary>
        private static void ValidateClosedObject(RecordWireSchemaDescriptor descriptor, JsonElement? value, string path)
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
        private static RecordWireDraftNodeKind ToDraftKind(RecordWireSchemaValueKind kind)
        {
            return kind switch
            {
                RecordWireSchemaValueKind.FloatBits => RecordWireDraftNodeKind.FloatBits,
                RecordWireSchemaValueKind.ByteArray => RecordWireDraftNodeKind.ByteArray,
                RecordWireSchemaValueKind.TranslatedString => RecordWireDraftNodeKind.TranslatedString,
                RecordWireSchemaValueKind.Asset => RecordWireDraftNodeKind.Asset,
                RecordWireSchemaValueKind.Color => RecordWireDraftNodeKind.Color,
                RecordWireSchemaValueKind.Array2D => RecordWireDraftNodeKind.Array2D,
                _ => RecordWireDraftNodeKind.Object,
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
        private readonly RecordWireReadLimits Limits;

        /// <summary>The operation cancellation token.</summary>
        private readonly CancellationToken CancellationToken;

        /// <summary>The number of visited JSON values.</summary>
        private int Nodes;

        /// <summary>Initializes one bounded JSON traversal.</summary>
        internal JsonLimitCounter(RecordWireReadLimits limits, CancellationToken cancellationToken)
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
    private static EngineResult<RecordWireDraftNode> FailureNode(string message)
    {
        return EngineResult<RecordWireDraftNode>.Failure(new EngineError(EngineErrorCode.ValidationFailed, message));
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

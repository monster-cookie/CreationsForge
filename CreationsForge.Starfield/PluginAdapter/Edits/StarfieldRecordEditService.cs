using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordInspection;
using CreationsForge.Starfield.PluginAdapter.RecordInspection;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield.PluginAdapter.Edits;

/// <summary>Prepares, applies, and previews closed typed edits against complete unpublished Starfield output candidates.</summary>
public sealed partial class StarfieldRecordEditService
{
    /// <summary>The complete mask composed only from flags declared by the installed typed Starfield API.</summary>
    private static readonly StarfieldMajorRecord.StarfieldMajorRecordFlag KnownMajorFlags =
        Enum.GetValues<StarfieldMajorRecord.StarfieldMajorRecordFlag>()
            .Aggregate((StarfieldMajorRecord.StarfieldMajorRecordFlag)0, static (mask, flag) => mask | flag);

    /// <summary>Initializes the stateless Starfield record edit service.</summary>
    public StarfieldRecordEditService()
    {
        Inspector = new StarfieldFormListInspector();
    }

    /// <summary>Gets the complete typed Starfield inspector used for changed-state and preview comparisons.</summary>
    public StarfieldFormListInspector Inspector { get; }

    /// <summary>Defensively copies one named edit and streams its complete canonical payload into a stable fingerprint.</summary>
    /// <param name="edit">The caller-owned typed edit to copy synchronously.</param>
    /// <returns>An immutable Starfield prepared payload, including deterministic validation failure when needed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="edit"/> is <see langword="null"/>.</exception>
    public PreparedFormListEdit PrepareEdit(FormListEdit edit)
    {
        ArgumentNullException.ThrowIfNull(edit);
        return edit switch
        {
            SetEditorIdEdit command => PrepareSetEditorId(command),
            ClearEditorIdEdit command => PrepareNoPayload(command, static formList => formList.EditorID = null),
            InsertItemEdit command => PrepareInsertItem(command),
            RemoveItemEdit command => PrepareRemoveItem(command),
            MoveItemEdit command => PrepareMoveItem(command),
            ReplaceItemsEdit command => PrepareReplaceItems(command),
            ClearItemsEdit command => PrepareNoPayload(command, static formList => formList.Items.Clear()),
            SetVersionControlEdit command => PrepareScalar(
                command,
                "versionControl",
                command.VersionControl,
                static (writer, name, value) => writer.WriteNumber(name, value),
                static (formList, value) => formList.VersionControl = value),
            SetFormVersionEdit command => PrepareScalar(
                command,
                "formVersion",
                command.FormVersion,
                static (writer, name, value) => writer.WriteNumber(name, value),
                static (formList, value) => formList.FormVersion = value),
            SetVersion2Edit command => PrepareScalar(
                command,
                "version2",
                command.Version2,
                static (writer, name, value) => writer.WriteNumber(name, value),
                static (formList, value) => formList.Version2 = value),
            SetCompressedEdit command => PrepareScalar(
                command,
                "isCompressed",
                command.IsCompressed,
                static (writer, name, value) => writer.WriteBoolean(name, value),
                static (formList, value) => formList.IsCompressed = value),
            SetDeletedEdit command => PrepareScalar(
                command,
                "isDeleted",
                command.IsDeleted,
                static (writer, name, value) => writer.WriteBoolean(name, value),
                static (formList, value) => formList.IsDeleted = value),
            StarfieldSetMajorFlagsEdit command => PrepareMajorFlags(command),
            StarfieldSetNameEdit command => PrepareSetName(command),
            StarfieldClearNameEdit command => PrepareNoPayload(command, static formList => formList.Name = null),
            StarfieldSetAddToListEdit command => PrepareSetAddToList(command),
            StarfieldClearAddToListEdit command => PrepareNoPayload(command, static formList => formList.AddToList.Clear()),
            StarfieldSetConditionalEntriesEdit command => PrepareSetConditionalEntries(command),
            StarfieldClearConditionalEntriesEdit command => PrepareNoPayload(command, static formList => formList.ConditionalEntries.Clear()),
            StarfieldAddComponentEdit command => PrepareAddComponent(command),
            StarfieldReplaceComponentEdit command => PrepareReplaceComponent(command),
            StarfieldRemoveComponentEdit command => PrepareRemoveComponent(command),
            StarfieldReplaceComponentsEdit command => PrepareReplaceComponents(command),
            _ => PrepareUnsupported(edit)
        };
    }

    /// <summary>Applies one valid prepared edit to an exact FormList inside an unpublished complete candidate.</summary>
    /// <param name="sources">The borrowed immutable Starfield sources used to validate newly introduced links.</param>
    /// <param name="candidate">The unpublished complete output candidate to mutate.</param>
    /// <param name="target">The exact FormList identity selected by the staged edit.</param>
    /// <param name="edit">The immutable payload returned by this Starfield service.</param>
    /// <returns>Whether the Mutagen candidate changed, or a typed failure before publication.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after a borrowed plugin lifetime is disposed.</exception>
    public EngineResult<RecordEditMutationResult> ApplyEdit(
        StarfieldPluginSourceSet sources,
        StarfieldPluginOutputState candidate,
        FormKey target,
        PreparedFormListEdit edit)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(candidate);
        ArgumentNullException.ThrowIfNull(edit);
        if (target.IsNull)
        {
            return Failure("A Starfield edit target cannot be the null FormKey.");
        }

        if (edit is not StarfieldPreparedEdit prepared)
        {
            return EngineResult<RecordEditMutationResult>.Failure(new EngineError(
                EngineErrorCode.InvalidRequest,
                "The prepared edit was not created by the Starfield record edit service."));
        }

        if (!prepared.IsValid)
        {
            return EngineResult<RecordEditMutationResult>.Failure(
                prepared.Error ?? new EngineError(EngineErrorCode.ValidationFailed, "The Starfield edit payload is invalid."));
        }

        var targetResult = FindMutableTarget(candidate.BorrowMod(), target);
        if (!targetResult.Succeeded || targetResult.Value is null)
        {
            return EngineResult<RecordEditMutationResult>.Failure(
                targetResult.Error ?? new EngineError(EngineErrorCode.RecordNotFound, $"Starfield FormList {target} was not found."));
        }

        var formList = targetResult.Value;
        var targetError = prepared.ValidateTarget(formList);
        if (targetError is not null)
        {
            return EngineResult<RecordEditMutationResult>.Failure(targetError);
        }

        var referenceError = ValidateIntroducedReferences(
            sources,
            candidate.BorrowMod(),
            prepared.GetIntroducedReferences(formList));
        if (referenceError is not null)
        {
            return EngineResult<RecordEditMutationResult>.Failure(referenceError);
        }

        try
        {
            var before = formList.DeepCopy();
            prepared.Apply(formList);
            var changed = Inspector.Compare(before, formList, CancellationToken.None).Count != 0;
            return EngineResult<RecordEditMutationResult>.Success(new RecordEditMutationResult(changed));
        }
        catch (Exception exception)
        {
            return EngineResult<RecordEditMutationResult>.Failure(new EngineError(
                EngineErrorCode.UnexpectedFailure,
                $"The prepared Starfield edit could not be applied: {exception.Message}"));
        }
    }

    /// <summary>Creates one scalar prepared payload through a closed typed Mutagen assignment.</summary>
    /// <typeparam name="T">The immutable scalar payload type.</typeparam>
    /// <param name="command">The named caller command.</param>
    /// <param name="propertyName">The stable canonical payload property name.</param>
    /// <param name="value">The copied scalar value.</param>
    /// <param name="writeValue">The typed canonical scalar writer.</param>
    /// <param name="apply">The typed record field assignment.</param>
    /// <returns>The immutable prepared scalar command.</returns>
    private static StarfieldPreparedEdit PrepareScalar<T>(
        FormListEdit command,
        string propertyName,
        T value,
        Action<Utf8JsonWriter, string, T> writeValue,
        Action<FormList, T> apply)
    {
        return CreatePrepared(
            command.CommandName,
            (writer, _) =>
            {
                writer.WriteStartObject();
                writeValue(writer, propertyName, value);
                writer.WriteEndObject();
            },
            formList => apply(formList, value));
    }

    /// <summary>Creates a prepared command with an empty canonical payload object.</summary>
    /// <param name="command">The named payload-free command.</param>
    /// <param name="apply">The closed typed Mutagen mutation.</param>
    /// <returns>The immutable prepared command.</returns>
    private static StarfieldPreparedEdit PrepareNoPayload(FormListEdit command, Action<FormList> apply)
    {
        return CreatePrepared(
            command.CommandName,
            static (writer, _) =>
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
            },
            apply);
    }

    /// <summary>Creates the one internal prepared-edit representation after canonical traversal completes.</summary>
    /// <param name="commandName">The stable typed command discriminator.</param>
    /// <param name="writePayload">The complete canonical payload writer.</param>
    /// <param name="apply">The closed typed mutation.</param>
    /// <param name="validateTarget">Optional validation against the immediate target state.</param>
    /// <param name="references">Optional newly introduced references requiring output-first validation.</param>
    /// <param name="getExistingReferenceCredits">Optional current-field link enumeration used to exclude retained reference multiplicities.</param>
    /// <param name="validationError">An earlier typed validation failure that does not prevent complete fingerprint traversal.</param>
    /// <returns>An immutable prepared Starfield command.</returns>
    private static StarfieldPreparedEdit CreatePrepared(
        string commandName,
        Action<Utf8JsonWriter, RecordJsonWriteContext> writePayload,
        Action<FormList> apply,
        Func<FormList, EngineError?>? validateTarget = null,
        IReadOnlyList<PreparedReference>? references = null,
        Func<FormList, IReadOnlyList<PreparedReference>>? getExistingReferenceCredits = null,
        EngineError? validationError = null)
    {
        var fingerprint = RecordEditFingerprintFactory.Create(commandName, writePayload);
        return new StarfieldPreparedEdit(
            fingerprint.Fingerprint,
            apply,
            validateTarget,
            references,
            getExistingReferenceCredits,
            fingerprint.ValidationError ?? validationError);
    }

    /// <summary>Creates a typed validation failure for a target position outside the current collection.</summary>
    /// <param name="message">The deterministic position diagnostic.</param>
    /// <returns>A validation error suitable for a failed apply result.</returns>
    private static EngineError InvalidPosition(string message)
    {
        return new EngineError(EngineErrorCode.ValidationFailed, message);
    }

    /// <summary>Creates a common failed mutation result for invalid Starfield edit input.</summary>
    /// <param name="message">The deterministic request diagnostic.</param>
    /// <returns>A typed validation failure.</returns>
    private static EngineResult<RecordEditMutationResult> Failure(string message)
    {
        return EngineResult<RecordEditMutationResult>.Failure(new EngineError(
            EngineErrorCode.ValidationFailed,
            message));
    }

    /// <summary>Represents one copied prepared command through closed typed validation and mutation delegates.</summary>
    private sealed class StarfieldPreparedEdit : PreparedFormListEdit
    {
        /// <summary>The closed typed mutation over one mutable Starfield FormList.</summary>
        private readonly Action<FormList> ApplyAction;

        /// <summary>The optional target-state validation evaluated before mutation.</summary>
        private readonly Func<FormList, EngineError?>? ValidateAction;

        /// <summary>The optional current-field link enumeration used for multiplicity-aware replacement validation.</summary>
        private readonly Func<FormList, IReadOnlyList<PreparedReference>>? GetExistingReferenceCreditsAction;

        /// <summary>Initializes one immutable prepared Starfield command.</summary>
        /// <param name="fingerprint">The complete canonical payload fingerprint.</param>
        /// <param name="apply">The closed typed Mutagen mutation.</param>
        /// <param name="validateTarget">Optional immediate target-state validation.</param>
        /// <param name="references">Optional newly introduced references.</param>
        /// <param name="getExistingReferenceCredits">Optional current-field links that may be retained without revalidation.</param>
        /// <param name="error">A deterministic preparation failure, or <see langword="null"/>.</param>
        internal StarfieldPreparedEdit(
            OperationFingerprint fingerprint,
            Action<FormList> apply,
            Func<FormList, EngineError?>? validateTarget,
            IReadOnlyList<PreparedReference>? references,
            Func<FormList, IReadOnlyList<PreparedReference>>? getExistingReferenceCredits,
            EngineError? error)
            : base(fingerprint, error)
        {
            ArgumentNullException.ThrowIfNull(apply);
            ApplyAction = apply;
            ValidateAction = validateTarget;
            GetExistingReferenceCreditsAction = getExistingReferenceCredits;
            References = Array.AsReadOnly(references?.ToArray() ?? Array.Empty<PreparedReference>());
        }

        /// <summary>Gets the copied links introduced by this command in deterministic payload order.</summary>
        internal IReadOnlyList<PreparedReference> References { get; }

        /// <summary>Validates current collection positions and options without changing the target.</summary>
        /// <param name="formList">The exact mutable target.</param>
        /// <returns>A typed failure, or <see langword="null"/> when mutation may proceed.</returns>
        internal EngineError? ValidateTarget(FormList formList)
        {
            return ValidateAction?.Invoke(formList);
        }

        /// <summary>Subtracts retained current-field link multiplicities from the prepared payload links.</summary>
        /// <param name="formList">The exact immediate target state.</param>
        /// <returns>Only reference occurrences newly introduced by this command.</returns>
        internal IReadOnlyList<PreparedReference> GetIntroducedReferences(FormList formList)
        {
            if (References.Count == 0 || GetExistingReferenceCreditsAction is null)
            {
                return References;
            }

            var credits = new Dictionary<(FormKey FormKey, Type ExpectedType), int>();
            foreach (var reference in GetExistingReferenceCreditsAction(formList))
            {
                var key = (reference.FormKey, reference.ExpectedType);
                credits.TryGetValue(key, out var count);
                credits[key] = checked(count + 1);
            }

            var introduced = new List<PreparedReference>();
            foreach (var reference in References)
            {
                var key = (reference.FormKey, reference.ExpectedType);
                if (credits.TryGetValue(key, out var count) && count > 0)
                {
                    credits[key] = count - 1;
                }
                else
                {
                    introduced.Add(reference);
                }
            }

            return Array.AsReadOnly(introduced.ToArray());
        }

        /// <summary>Applies the already validated closed typed mutation.</summary>
        /// <param name="formList">The exact mutable target.</param>
        internal void Apply(FormList formList)
        {
            ApplyAction(formList);
        }
    }

    /// <summary>Describes one copied link together with its Mutagen declared getter-family constraint.</summary>
    private readonly struct PreparedReference
    {
        /// <summary>Initializes one deterministic prepared reference.</summary>
        /// <param name="formKey">The non-null linked record identity.</param>
        /// <param name="fieldPath">The command-relative field path used in diagnostics.</param>
        /// <param name="expectedType">The record getter interface declared by the typed link.</param>
        internal PreparedReference(FormKey formKey, string fieldPath, Type expectedType)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath);
            ArgumentNullException.ThrowIfNull(expectedType);
            FormKey = formKey;
            FieldPath = fieldPath;
            ExpectedType = expectedType;
        }

        /// <summary>Gets the linked record identity.</summary>
        internal FormKey FormKey { get; }

        /// <summary>Gets the stable command-relative link location.</summary>
        internal string FieldPath { get; }

        /// <summary>Gets the record getter interface that the resolved target must implement.</summary>
        internal Type ExpectedType { get; }
    }
}

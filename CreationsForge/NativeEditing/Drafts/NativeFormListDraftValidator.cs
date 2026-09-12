using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.RegularExpressions;
using CreationsForge.Core.Engine.NativeWire;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Validates typed native wire drafts with exact numeric, structural, and resource semantics.</summary>
public sealed class NativeFormListDraftValidator : INativeFormListDraftValidator
{
    /// <summary>The bounded regular-expression evaluation timeout.</summary>
    private static readonly TimeSpan PatternTimeout = TimeSpan.FromMilliseconds(250);

    /// <inheritdoc />
    public NativeFormListDraftValidationResult Validate(NativeFormListDraft draft, NativeWireReadLimits limits, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(limits);
        cancellationToken.ThrowIfCancellationRequested();
        var state = new ValidationState(limits, cancellationToken);
        state.Validate(draft.Root, 1);
        var result = new NativeFormListDraftValidationResult(state.Issues);
        draft.SetIssues(result.Issues);
        return result;
    }

    /// <summary>Tracks bounded validation and deterministic issues for one pass.</summary>
    private sealed class ValidationState
    {
        /// <summary>The per-operation resource limits.</summary>
        private readonly NativeWireReadLimits Limits;

        /// <summary>The validation cancellation token.</summary>
        private readonly CancellationToken CancellationToken;

        /// <summary>The mutable deterministic issue sink.</summary>
        private readonly List<NativeWireDraftIssue> MutableIssues = new();

        /// <summary>The number of visited materialized nodes.</summary>
        private int VisitedNodes;

        /// <summary>Initializes one bounded validation state.</summary>
        internal ValidationState(NativeWireReadLimits limits, CancellationToken cancellationToken)
        {
            Limits = limits;
            CancellationToken = cancellationToken;
        }

        /// <summary>Gets the complete issues in deterministic traversal order.</summary>
        internal IReadOnlyList<NativeWireDraftIssue> Issues => MutableIssues;

        /// <summary>Validates one typed node and its currently materialized descendants.</summary>
        /// <param name="node">The typed node to validate.</param>
        /// <param name="depth">The one-based materialized depth.</param>
        /// <param name="expandRequiredObject">Whether an explicitly inserted or selected object shell exposes its required child issues.</param>
        internal void Validate(NativeWireDraftNode node, int depth, bool expandRequiredObject = false)
        {
            CancellationToken.ThrowIfCancellationRequested();
            if (depth > Limits.MaximumDepth)
            {
                Add(NativeWireDraftIssueCode.ResourceLimitExceeded, node, $"{node.Path}: draft exceeds maximum depth {Limits.MaximumDepth}.");
                return;
            }

            if (++VisitedNodes > Limits.MaximumNodes)
            {
                Add(NativeWireDraftIssueCode.ResourceLimitExceeded, node, $"{node.Path}: draft exceeds maximum node count {Limits.MaximumNodes}.");
                return;
            }

            if (node.ValueState == NativeWireDraftValueState.RequiredUnset && !(expandRequiredObject && node is NativeWireObjectDraftNode))
            {
                if (node.IsRequired)
                {
                    var reason = node.Descriptor.Annotations.GetValueOrDefault("x-default-unavailable-reason");
                    Add(NativeWireDraftIssueCode.RequiredValueUnset, node, $"{node.Path}: required value is unset{(string.IsNullOrWhiteSpace(reason) ? "." : $"; {reason}")}");
                }

                return;
            }

            switch (node)
            {
                case NativeWireArrayDraftNode array:
                    ValidateArray(array, depth);
                    break;
                case NativeWireUnionDraftNode union:
                    if (union.SelectedValue is null)
                    {
                        Add(NativeWireDraftIssueCode.RequiredValueUnset, union, $"{union.Path}: select an explicit union value.");
                    }
                    else
                    {
                        Validate(union.SelectedValue, depth + 1, expandRequiredObject: true);
                    }

                    break;
                case NativeWireNullableDraftNode nullable when !nullable.IsNull && nullable.Value is null:
                    Add(NativeWireDraftIssueCode.RequiredValueUnset, nullable, $"{nullable.Path}: present nullable value has no typed value.");
                    break;
                case NativeWireNullableDraftNode nullable when !nullable.IsNull:
                    Validate(nullable.Value!, depth + 1, expandRequiredObject: true);
                    break;
                case NativeWireFormLinkOrIndexDraftNode formLinkOrIndex:
                    if (!formLinkOrIndex.IsIndexNull)
                    {
                        var indexDescriptor = formLinkOrIndex.Descriptor.Properties.FirstOrDefault(property => string.Equals(property.Name, "index", StringComparison.Ordinal))?.Descriptor;
                        if (indexDescriptor is null)
                        {
                            Add(NativeWireDraftIssueCode.SchemaResolutionFailed, formLinkOrIndex, $"{formLinkOrIndex.Path}.index: FormLinkOrIndex schema is missing the index descriptor.");
                        }
                        else
                        {
                            ValidateIntegerText(formLinkOrIndex, formLinkOrIndex.IndexText, indexDescriptor.Minimum, indexDescriptor.Maximum);
                        }
                    }

                    Validate(formLinkOrIndex.Link, depth + 1);
                    break;
                case NativeWireFormLinkDraftNode formLink:
                    ValidateFormLink(formLink);
                    break;
                case NativeWireFloatBitsDraftNode floatBits:
                    ValidateChildren(floatBits, depth);
                    ValidateFloatBits(floatBits);
                    break;
                case NativeWireByteArrayDraftNode bytes:
                    ValidateChildren(bytes, depth);
                    ValidateBytes(bytes);
                    break;
                case NativeWireArray2DDraftNode array2D:
                    ValidateChildren(array2D, depth);
                    ValidateArray2D(array2D);
                    break;
                case NativeWireObjectDraftNode objectNode:
                    ValidateChildren(objectNode, depth);
                    break;
                case NativeWireIntegerDraftNode integer:
                    ValidateIntegerText(integer, integer.Text, integer.Descriptor.Minimum, integer.Descriptor.Maximum);
                    break;
                case NativeWireStringDraftNode text:
                    ValidateString(text);
                    break;
            }
        }

        /// <summary>Validates every direct child of one object node.</summary>
        private void ValidateChildren(NativeWireObjectDraftNode objectNode, int depth)
        {
            foreach (var property in objectNode.Properties)
            {
                Validate(property.Node, depth + 1);
            }
        }

        /// <summary>Validates array bounds, every item, and optional unique-property semantics.</summary>
        private void ValidateArray(NativeWireArrayDraftNode array, int depth)
        {
            var maximum = Math.Min(Limits.MaximumArrayElements, array.Descriptor.MaximumItems ?? int.MaxValue);
            if (array.Items.Count > maximum)
            {
                Add(NativeWireDraftIssueCode.ResourceLimitExceeded, array, $"{array.Path}: array contains {array.Items.Count} elements, exceeding maximum {maximum}.");
            }

            foreach (var item in array.Items)
            {
                Validate(item, depth + 1, expandRequiredObject: true);
            }

            var uniqueProperty = array.Descriptor.Annotations.GetValueOrDefault("x-unique-property");
            if (!string.IsNullOrWhiteSpace(uniqueProperty))
            {
                var values = new HashSet<string>(StringComparer.Ordinal);
                foreach (var item in array.Items.OfType<NativeWireObjectDraftNode>())
                {
                    var property = item.FindProperty(uniqueProperty);
                    if (property is null || property.ValueState == NativeWireDraftValueState.RequiredUnset)
                    {
                        continue;
                    }

                    var canonical = NativeWireDraftJsonWriter.WriteDetached(property, CancellationToken).GetRawText();
                    if (!values.Add(canonical))
                    {
                        Add(NativeWireDraftIssueCode.CollectionConstraintViolated, property, $"{property.Path}: duplicate value violates unique '{uniqueProperty}' collection key.");
                    }
                }
            }
        }

        /// <summary>Validates canonical integer text and inclusive schema bounds.</summary>
        private void ValidateIntegerText(NativeWireDraftNode node, string text, string? minimum, string? maximum)
        {
            if (!BigInteger.TryParse(text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value) || !string.Equals(value.ToString(CultureInfo.InvariantCulture), text, StringComparison.Ordinal))
            {
                Add(NativeWireDraftIssueCode.InvalidValue, node, $"{node.Path}: integer must use canonical decimal text without exponent, separators, or redundant leading characters.");
                return;
            }

            if (minimum is not null && BigInteger.TryParse(minimum, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var lower) && value < lower)
            {
                Add(NativeWireDraftIssueCode.NumericValueOutOfRange, node, $"{node.Path}: integer {text} is below minimum {minimum}.");
            }

            if (maximum is not null && BigInteger.TryParse(maximum, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var upper) && value > upper)
            {
                Add(NativeWireDraftIssueCode.NumericValueOutOfRange, node, $"{node.Path}: integer {text} exceeds maximum {maximum}.");
            }
        }

        /// <summary>Validates one bounded string, exact JSON-number projection, and schema pattern.</summary>
        private void ValidateString(NativeWireStringDraftNode text)
        {
            if (text.Value.Length > Limits.MaximumStringLength)
            {
                Add(NativeWireDraftIssueCode.ResourceLimitExceeded, text, $"{text.Path}: string length {text.Value.Length} exceeds operation maximum {Limits.MaximumStringLength}.");
            }

            if (text.Descriptor.MinimumLength.HasValue && text.Value.Length < text.Descriptor.MinimumLength.Value)
            {
                Add(NativeWireDraftIssueCode.StringConstraintViolated, text, $"{text.Path}: string length {text.Value.Length} is below minimum {text.Descriptor.MinimumLength.Value}.");
            }

            if (text.Descriptor.MaximumLength.HasValue && text.Value.Length > text.Descriptor.MaximumLength.Value)
            {
                Add(NativeWireDraftIssueCode.StringConstraintViolated, text, $"{text.Path}: string length {text.Value.Length} exceeds maximum {text.Descriptor.MaximumLength.Value}.");
            }

            if (text.Descriptor.Annotations.ContainsKey("x-presentation-json-number"))
            {
                try
                {
                    using var document = JsonDocument.Parse(text.Value);
                    if (document.RootElement.ValueKind != JsonValueKind.Number)
                    {
                        Add(NativeWireDraftIssueCode.InvalidValue, text, $"{text.Path}: value must be one exact JSON number token.");
                    }
                }
                catch (JsonException)
                {
                    Add(NativeWireDraftIssueCode.InvalidValue, text, $"{text.Path}: value must be one exact JSON number token.");
                }
            }

            if (text.Descriptor.Pattern is not null)
            {
                try
                {
                    if (!Regex.IsMatch(text.Value, text.Descriptor.Pattern, RegexOptions.CultureInvariant, PatternTimeout))
                    {
                        Add(NativeWireDraftIssueCode.StringConstraintViolated, text, $"{text.Path}: string does not match required pattern {text.Descriptor.Pattern}.");
                    }
                }
                catch (ArgumentException)
                {
                    Add(NativeWireDraftIssueCode.SchemaResolutionFailed, text, $"{text.Path}: catalog contains an invalid regular-expression pattern.");
                }
                catch (RegexMatchTimeoutException)
                {
                    Add(NativeWireDraftIssueCode.ResourceLimitExceeded, text, $"{text.Path}: pattern validation exceeded its bounded timeout.");
                }
            }
        }

        /// <summary>Validates FormLink null identity without normalizing JSON null and canonical Null.</summary>
        private void ValidateFormLink(NativeWireFormLinkDraftNode formLink)
        {
            if (formLink.IsNull)
            {
                if (formLink.FormKey is not null && !string.Equals(formLink.FormKey, "Null", StringComparison.Ordinal))
                {
                    Add(NativeWireDraftIssueCode.InvalidValue, formLink, $"{formLink.Path}: an explicit null FormLink requires JSON null or canonical FormKey 'Null'.");
                }
            }
            else if (string.IsNullOrWhiteSpace(formLink.FormKey) || string.Equals(formLink.FormKey, "Null", StringComparison.Ordinal))
            {
                Add(NativeWireDraftIssueCode.InvalidValue, formLink, $"{formLink.Path}: a non-null FormLink requires a non-null canonical FormKey.");
            }
        }

        /// <summary>Validates Base64, decoded length, and redundant declared length before allocation.</summary>
        private void ValidateBytes(NativeWireByteArrayDraftNode bytes)
        {
            var base64 = bytes.Value;
            var maximumDecodedLength = ((long)base64.Length + 3L) / 4L * 3L;
            if (maximumDecodedLength > Limits.MaximumDecodedByteLength + 2L)
            {
                Add(NativeWireDraftIssueCode.ResourceLimitExceeded, bytes, $"{bytes.Path}.base64: encoded value can exceed decoded-byte maximum {Limits.MaximumDecodedByteLength}.");
                return;
            }

            var buffer = new byte[Math.Min((int)maximumDecodedLength, Limits.MaximumDecodedByteLength)];
            if (!Convert.TryFromBase64String(base64, buffer, out var written))
            {
                Add(NativeWireDraftIssueCode.InvalidValue, bytes, $"{bytes.Path}.base64: value is not valid Base64.");
                return;
            }

            if (bytes.Length is not null && BigInteger.TryParse(bytes.Length.Text, NumberStyles.None, CultureInfo.InvariantCulture, out var declared) && declared != written)
            {
                Add(NativeWireDraftIssueCode.InvalidValue, bytes.Length, $"{bytes.Length.Path}: declared byte length {declared} does not match decoded length {written}.");
            }
        }

        /// <summary>Validates authoritative float bits against redundant invariant text and optional finite JSON number.</summary>
        private void ValidateFloatBits(NativeWireFloatBitsDraftNode floatBits)
        {
            if (floatBits.FindProperty("bits") is not NativeWireStringDraftNode bits || floatBits.FindProperty("text") is not NativeWireStringDraftNode text)
            {
                Add(NativeWireDraftIssueCode.SchemaResolutionFailed, floatBits, $"{floatBits.Path}: float-bit draft requires text and bits properties.");
                return;
            }

            var number = floatBits.FindProperty("number") as NativeWireStringDraftNode;
            if (bits.Value.Length == 10 && uint.TryParse(bits.Value.AsSpan(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var singleBits))
            {
                var value = BitConverter.UInt32BitsToSingle(singleBits);
                ValidateFloatProjection(floatBits, text, number, value.ToString("R", CultureInfo.InvariantCulture), float.IsFinite(value), BitConverter.SingleToUInt32Bits(value) == singleBits, value);
            }
            else if (bits.Value.Length == 18 && ulong.TryParse(bits.Value.AsSpan(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var doubleBits))
            {
                var value = BitConverter.UInt64BitsToDouble(doubleBits);
                ValidateFloatProjection(floatBits, text, number, value.ToString("R", CultureInfo.InvariantCulture), double.IsFinite(value), BitConverter.DoubleToUInt64Bits(value) == doubleBits, value);
            }
            else
            {
                Add(NativeWireDraftIssueCode.InvalidValue, bits, $"{bits.Path}: bits must be exact uppercase 0x-prefixed 8- or 16-digit hexadecimal text.");
            }
        }

        /// <summary>Validates redundant float text and number values against authoritative bits.</summary>
        private void ValidateFloatProjection<T>(NativeWireFloatBitsDraftNode owner, NativeWireStringDraftNode text, NativeWireStringDraftNode? number, string expectedText, bool finite, bool bitsRoundTrip, T exactValue) where T : struct, IFormattable
        {
            if (!bitsRoundTrip || !string.Equals(text.Value, expectedText, StringComparison.Ordinal))
            {
                Add(NativeWireDraftIssueCode.InvalidValue, text, $"{text.Path}: invariant text must match authoritative bits as '{expectedText}'.");
            }

            var numberPresent = number is not null && number.ValueState != NativeWireDraftValueState.RequiredUnset;
            if (finite && !numberPresent)
            {
                Add(NativeWireDraftIssueCode.RequiredValueUnset, owner, $"{owner.Path}.number: finite authoritative bits require a matching JSON number projection.");
            }
            else if (!finite && numberPresent)
            {
                Add(NativeWireDraftIssueCode.InvalidValue, number!, $"{number!.Path}: non-finite authoritative bits must omit the JSON number projection.");
            }
            else if (finite && numberPresent)
            {
                var expected = exactValue.ToString("R", CultureInfo.InvariantCulture);
                if (!string.Equals(number!.Value, expected, StringComparison.Ordinal) && !NumericJsonEquals(number.Value, expected))
                {
                    Add(NativeWireDraftIssueCode.InvalidValue, number, $"{number.Path}: JSON number must match authoritative bits value {expected}.");
                }
            }
        }

        /// <summary>Compares two finite JSON number spellings through decimal or double value semantics.</summary>
        private static bool NumericJsonEquals(string left, string right)
        {
            return decimal.TryParse(left, NumberStyles.Float, CultureInfo.InvariantCulture, out var leftDecimal) && decimal.TryParse(right, NumberStyles.Float, CultureInfo.InvariantCulture, out var rightDecimal)
                ? leftDecimal == rightDecimal
                : double.TryParse(left, NumberStyles.Float, CultureInfo.InvariantCulture, out var leftDouble) && double.TryParse(right, NumberStyles.Float, CultureInfo.InvariantCulture, out var rightDouble) && leftDouble.Equals(rightDouble);
        }

        /// <summary>Validates row count, width, and row boundaries for a two-dimensional array.</summary>
        private void ValidateArray2D(NativeWireArray2DDraftNode array2D)
        {
            if (array2D.FindProperty("width") is not NativeWireIntegerDraftNode width || array2D.FindProperty("height") is not NativeWireIntegerDraftNode height || array2D.FindProperty("rows") is not NativeWireArrayDraftNode rows ||
                !int.TryParse(width.Text, NumberStyles.None, CultureInfo.InvariantCulture, out var expectedWidth) || !int.TryParse(height.Text, NumberStyles.None, CultureInfo.InvariantCulture, out var expectedHeight))
            {
                Add(NativeWireDraftIssueCode.InvalidValue, array2D, $"{array2D.Path}: two-dimensional array requires canonical width, height, and rows values.");
                return;
            }

            if (rows.Items.Count != expectedHeight)
            {
                Add(NativeWireDraftIssueCode.CollectionConstraintViolated, rows, $"{rows.Path}: row count {rows.Items.Count} does not match height {expectedHeight}.");
            }

            for (var index = 0; index < rows.Items.Count; index++)
            {
                if (rows.Items[index] is not NativeWireArrayDraftNode row || row.Items.Count != expectedWidth)
                {
                    Add(NativeWireDraftIssueCode.CollectionConstraintViolated, rows.Items[index], $"{rows.Items[index].Path}: row width must equal {expectedWidth}.");
                }
            }
        }

        /// <summary>Adds one exact node-bound issue.</summary>
        private void Add(NativeWireDraftIssueCode code, NativeWireDraftNode node, string message)
        {
            MutableIssues.Add(new NativeWireDraftIssue(code, node.Path, message, node));
        }
    }
}

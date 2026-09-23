namespace CreationsForge.Engine.Records;

/// <summary>Compares closed record values without flattening collection, translation, or object semantics.</summary>
internal static class RecordValueComparer
{
    /// <summary>Tests two record values for exact structural equality.</summary>
    /// <param name="left">The left value.</param>
    /// <param name="right">The right value.</param>
    /// <returns><see langword="true"/> when both values have equal type and content.</returns>
    public static bool Equals(RecordValue left, RecordValue right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left.Kind != right.Kind)
        {
            return false;
        }

        return (left, right) switch
        {
            (RecordValue.NullRecordValue, RecordValue.NullRecordValue) => true,
            (RecordValue.StringRecordValue leftValue, RecordValue.StringRecordValue rightValue) => leftValue.Value == rightValue.Value,
            (RecordValue.BooleanRecordValue leftValue, RecordValue.BooleanRecordValue rightValue) => leftValue.Value == rightValue.Value,
            (RecordValue.SignedIntegerRecordValue leftValue, RecordValue.SignedIntegerRecordValue rightValue) => leftValue.Value == rightValue.Value,
            (RecordValue.UnsignedIntegerRecordValue leftValue, RecordValue.UnsignedIntegerRecordValue rightValue) => leftValue.Value == rightValue.Value,
            (RecordValue.FloatingPointRecordValue leftValue, RecordValue.FloatingPointRecordValue rightValue) => leftValue.Value.Equals(rightValue.Value),
            (RecordValue.EnumRecordValue leftValue, RecordValue.EnumRecordValue rightValue) => leftValue.Value == rightValue.Value,
            (RecordValue.FormLinkRecordValue leftValue, RecordValue.FormLinkRecordValue rightValue) => leftValue.FormKey == rightValue.FormKey,
            (RecordValue.ListRecordValue leftValue, RecordValue.ListRecordValue rightValue) => SequenceEquals(leftValue.Values, rightValue.Values),
            (RecordValue.TranslatedStringRecordValue leftValue, RecordValue.TranslatedStringRecordValue rightValue) => TranslatedEquals(leftValue, rightValue),
            (RecordValue.ObjectRecordValue leftValue, RecordValue.ObjectRecordValue rightValue) => ObjectEquals(leftValue, rightValue),
            _ => false,
        };
    }

    private static bool SequenceEquals(IReadOnlyList<RecordValue> left, IReadOnlyList<RecordValue> right)
    {
        return left.Count == right.Count && left.Zip(right).All(pair => Equals(pair.First, pair.Second));
    }

    private static bool TranslatedEquals(
        RecordValue.TranslatedStringRecordValue left,
        RecordValue.TranslatedStringRecordValue right)
    {
        return left.TargetLanguage == right.TargetLanguage
            && left.Values.Count == right.Values.Count
            && left.Values.All(pair => right.Values.TryGetValue(pair.Key, out var value) && value == pair.Value);
    }

    private static bool ObjectEquals(RecordValue.ObjectRecordValue left, RecordValue.ObjectRecordValue right)
    {
        return left.Alternative == right.Alternative
            && left.Fields.Count == right.Fields.Count
            && left.Fields.All(pair => right.Fields.TryGetValue(pair.Key, out var value) && Equals(pair.Value, value));
    }
}

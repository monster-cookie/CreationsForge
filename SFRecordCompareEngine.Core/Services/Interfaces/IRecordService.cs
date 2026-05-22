namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IRecordService
{
    /// <summary>
    ///     Resolve a raw or normalized record reference value to display text from SQLite record metadata.
    /// </summary>
    /// <param name="referenceValue">The reference value, typically a FormKey/FormLink string.</param>
    /// <returns>The EditorID when available, the normalized reference when unresolved, or null for empty input.</returns>
    string? ResolveReferenceDisplayValue(string referenceValue);
}

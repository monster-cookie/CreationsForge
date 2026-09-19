namespace CreationsForge.ViewModels;

/// <summary>Identifies the field used to order rows within each major-record family.</summary>
public enum MajorRecordSortMode
{
    /// <summary>Orders records by local FormID with origin plugin as a stable tie-breaker.</summary>
    FormId,

    /// <summary>Orders records by EditorID, placing records without an EditorID last.</summary>
    EditorId
}

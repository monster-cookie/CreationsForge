namespace CreationsForge.ViewModels;

/// <summary>Identifies the field used to order records inside each plugin and major-record group.</summary>
public enum FormListRecordSortMode
{
    /// <summary>Orders records by their numeric local FormID, matching plugin-editor navigation.</summary>
    FormId,

    /// <summary>Orders records alphabetically by EditorID, placing records without an EditorID last.</summary>
    EditorId
}

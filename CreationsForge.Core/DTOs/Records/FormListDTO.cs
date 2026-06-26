using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.DTOs.Records.Metadata;

namespace CreationsForge.Core.DTOs.Records;

public class FormListDTO : RecordDTO, IHasName
{
    [LocalizedField("Name")]
    public TranslatedStringDTO? Name { get; set; }

    public FormKeyDTO? AddToList { get; set; }

    public IList<FormListItemDTO> Items { get; set; } = new List<FormListItemDTO>();
}

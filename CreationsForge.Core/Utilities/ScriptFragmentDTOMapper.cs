using System.Collections;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Utilities;

public static class ScriptFragmentDTOMapper
{
    public static List<ScriptFragmentDTO> FromScriptFragments(SupportedGame game, ModKeyDTO modKey, string recordType, FormKey formKey, object? scriptFragments, DateTime importedAtUTC)
    {
        var results = new List<ScriptFragmentDTO>();
        if (scriptFragments == null)
        {
            return results;
        }

        AddFragment(results, game, modKey, recordType, formKey, "ScriptFragments", 0, scriptFragments, importedAtUTC);
        return results;
    }

    private static void AddFragment(
        ICollection<ScriptFragmentDTO> results,
        SupportedGame game,
        ModKeyDTO modKey,
        string recordType,
        FormKey formKey,
        string fragmentSlot,
        int fragmentIndex,
        object fragment,
        DateTime importedAtUTC)
    {
        if (fragment is IEnumerable enumerable && fragment is not string)
        {
            foreach (var item in enumerable.Cast<object>().Select((value, index) => new { value, index }))
            {
                AddFragment(results, game, modKey, recordType, formKey, fragmentSlot, item.index, item.value, importedAtUTC);
            }

            return;
        }

        var script = GetPropertyValue(fragment, "Script");
        if (script != null)
        {
            results.Add(CreateFragment(game, modKey, recordType, formKey, fragmentSlot + ".Script", fragmentIndex, script, importedAtUTC));
        }

        var fragments = GetPropertyValue(fragment, "Fragments");
        if (fragments != null)
        {
            AddFragment(results, game, modKey, recordType, formKey, fragmentSlot + ".Fragments", fragmentIndex, fragments, importedAtUTC);
        }

        foreach (var property in fragment.GetType().GetProperties())
        {
            if (property.GetIndexParameters().Length > 0 ||
                !property.CanRead ||
                string.Equals(property.Name, "Script", StringComparison.Ordinal) ||
                string.Equals(property.Name, "Fragments", StringComparison.Ordinal))
            {
                continue;
            }

            var propertyValue = GetPropertyValue(fragment, property.Name);
            if (propertyValue == null || !HasFragmentData(propertyValue))
            {
                continue;
            }

            results.Add(CreateFragment(game, modKey, recordType, formKey, fragmentSlot + "." + property.Name, fragmentIndex, propertyValue, importedAtUTC));
        }
    }

    private static ScriptFragmentDTO CreateFragment(
        SupportedGame game,
        ModKeyDTO modKey,
        string recordType,
        FormKey formKey,
        string fragmentSlot,
        int fragmentIndex,
        object source,
        DateTime importedAtUTC)
    {
        return new ScriptFragmentDTO
        {
            Game = game,
            ModKey = modKey,
            RecordType = recordType,
            FormKey = MapFormKey(formKey),
            FragmentSlot = fragmentSlot,
            FragmentIndex = fragmentIndex,
            MutagenObjectType = source.GetType().Name,
            ScriptName = GetPropertyValue(source, "ScriptName")?.ToString() ?? GetPropertyValue(source, "Name")?.ToString(),
            FragmentName = GetPropertyValue(source, "FragmentName")?.ToString(),
            Unknown2 = GetPropertyValue(source, "Unknown2") is { } unknown2 ? Convert.ToInt32(unknown2) : null,
            ExtraBindDataVersion = GetPropertyValue(source, "ExtraBindDataVersion") is { } version ? Convert.ToInt32(version) : null,
            ImportedAtUTC = importedAtUTC
        };
    }

    private static bool HasFragmentData(object source)
    {
        return GetPropertyValue(source, "ScriptName") != null ||
            GetPropertyValue(source, "Name") != null ||
            GetPropertyValue(source, "FragmentName") != null ||
            GetPropertyValue(source, "Unknown2") != null ||
            GetPropertyValue(source, "ExtraBindDataVersion") != null;
    }

    private static object? GetPropertyValue(object source, string propertyName)
    {
        return source.GetType().GetProperty(propertyName)?.GetValue(source);
    }

    private static FormKeyDTO MapFormKey(FormKey formKey)
    {
        return new FormKeyDTO
        {
            ModKey = ModKeyDTOMapper.FromModKey(formKey.ModKey),
            Id = formKey.ID
        };
    }
}

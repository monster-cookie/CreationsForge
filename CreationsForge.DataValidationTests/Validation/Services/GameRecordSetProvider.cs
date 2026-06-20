using Autofac;
using CreationsForge.Bootstrap.Composition;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;

namespace CreationsForge.DataValidationTests.Validation.Services;

public class GameRecordSetProvider
{
    private static readonly Lazy<IContainer> Container = new(() => AutofacConfigurator.Configure());

    public RecordDTO GetRecord(SupportedGame game, string recordType, string rawFormKey)
    {
        var expectedFormKey = ParseFormKey(rawFormKey);
        using var scope = Container.Value.BeginLifetimeScope();
        var record = GetRecords(scope, game, recordType, expectedFormKey)
            .FirstOrDefault(candidate => FormKeysMatch(candidate.FormKey, expectedFormKey) &&
                                         string.Equals(candidate.ModKey.FileName, expectedFormKey.ModKey.FileName, StringComparison.OrdinalIgnoreCase));

        return record ?? throw new InvalidOperationException(
            $"Unable to find record '{rawFormKey}' for record type '{recordType}' in game '{game}'.");
    }

    private static IEnumerable<RecordDTO> GetRecords(ILifetimeScope scope, SupportedGame game, string recordType, FormKeyDTO formKey)
    {
        return recordType switch
        {
            "AVIF" => scope.Resolve<IActorValueInformationRepository>().GetByFormKey(game, formKey),
            "BOOK" => scope.Resolve<IBookRepository>().GetByFormKey(game, formKey),
            "CLAS" => scope.Resolve<IClassRepository>().GetByFormKey(game, formKey),
            "CONT" => scope.Resolve<IContainerRepository>().GetByFormKey(game, formKey),
            "CNDF" => scope.Resolve<IConditionFormRepository>().GetByFormKey(game, formKey),
            "COBJ" => scope.Resolve<IConstructibleObjectRepository>().GetByFormKey(game, formKey),
            "DOOR" => scope.Resolve<IDoorRepository>().GetByFormKey(game, formKey),
            "FACT" => scope.Resolve<IFactionRepository>().GetByFormKey(game, formKey),
            "FLST" => scope.Resolve<IFormListRepository>().GetByFormKey(game, formKey),
            "GMST" => scope.Resolve<IGameSettingRepository>().GetByFormKey(game, formKey),
            "GLOB" => scope.Resolve<IGlobalRepository>().GetByFormKey(game, formKey),
            "KYWD" => scope.Resolve<IKeywordRepository>().GetByFormKey(game, formKey),
            "MGEF" => scope.Resolve<IMagicEffectRepository>().GetByFormKey(game, formKey),
            "MISC" => scope.Resolve<IMiscObjectRepository>().GetByFormKey(game, formKey),
            "NPC_" => scope.Resolve<INPCRepository>().GetByFormKey(game, formKey),
            "PERK" => scope.Resolve<IPerkRepository>().GetByFormKey(game, formKey),
            "STAT" => scope.Resolve<IStaticRepository>().GetByFormKey(game, formKey),
            "TERM" => scope.Resolve<ITerminalRepository>().GetByFormKey(game, formKey),
            _ => throw new InvalidOperationException($"Unsupported record type '{recordType}'.")
        };
    }

    private static FormKeyDTO ParseFormKey(string rawFormKey)
    {
        var separatorIndex = rawFormKey.IndexOf(':', StringComparison.Ordinal);
        if (separatorIndex <= 0 || separatorIndex >= rawFormKey.Length - 1)
        {
            throw new FormatException($"Invalid Spriggit FormKey '{rawFormKey}'.");
        }

        var fileName = rawFormKey[(separatorIndex + 1)..];
        return new FormKeyDTO
        {
            Id = Convert.ToUInt32(rawFormKey[..separatorIndex], 16),
            ModKey = new ModKeyDTO
            {
                FileName = fileName,
                Name = Path.GetFileNameWithoutExtension(fileName),
                Type = 0
            }
        };
    }

    private static bool FormKeysMatch(FormKeyDTO left, FormKeyDTO right)
    {
        return left.Id == right.Id &&
               string.Equals(left.ModKey.FileName, right.ModKey.FileName, StringComparison.OrdinalIgnoreCase);
    }
}

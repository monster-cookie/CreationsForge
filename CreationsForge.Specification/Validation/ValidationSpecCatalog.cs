using CreationsForge.Specification.Validation.Specs.ActorValueInformation;
using CreationsForge.Specification.Validation.Specs.Book;
using CreationsForge.Specification.Validation.Specs.Class;
using CreationsForge.Specification.Validation.Specs.ConditionForm;
using CreationsForge.Specification.Validation.Specs.ConstructibleObject;
using CreationsForge.Specification.Validation.Specs.Container;
using CreationsForge.Specification.Validation.Specs.Door;
using CreationsForge.Specification.Validation.Specs.Faction;
using CreationsForge.Specification.Validation.Specs.FormList;
using CreationsForge.Specification.Validation.Specs.GameSetting;
using CreationsForge.Specification.Validation.Specs.Global;
using CreationsForge.Specification.Validation.Specs.Keyword;
using CreationsForge.Specification.Validation.Specs.MagicEffect;
using CreationsForge.Specification.Validation.Specs.MiscItem;
using CreationsForge.Specification.Validation.Specs.NPC;
using CreationsForge.Specification.Validation.Specs.Perk;
using CreationsForge.Specification.Validation.Specs.Static;
using CreationsForge.Specification.Validation.Specs.Terminal;

namespace CreationsForge.Specification.Validation;

/// <summary>
/// Exposes the Spriggit validation sample specifications declared by the production specification library.
/// </summary>
public static class ValidationSpecCatalog
{
    /// <summary>
    /// Gets every validation specification in deterministic record-family order.
    /// </summary>
    public static IReadOnlyList<ValidationSpec> All { get; } =
    [
        .. GetActorValueInformationSpecs(),
        .. GetBookSpecs(),
        .. GetClassSpecs(),
        .. GetConditionFormSpecs(),
        .. GetConstructibleObjectSpecs(),
        .. GetContainerSpecs(),
        .. GetDoorSpecs(),
        .. GetFactionSpecs(),
        .. GetFormListSpecs(),
        .. GetGameSettingSpecs(),
        .. GetGlobalSpecs(),
        .. GetKeywordSpecs(),
        .. GetMagicEffectSpecs(),
        .. GetMiscItemSpecs(),
        .. GetNPCSpecs(),
        .. GetPerkSpecs(),
        .. GetStaticSpecs(),
        .. GetTerminalSpecs()
    ];

    private static IReadOnlyList<ValidationSpec> GetActorValueInformationSpecs()
    {
        return InvokePublicFactories(typeof(ActorValueInformationValidationSpecs));
    }

    private static IReadOnlyList<ValidationSpec> GetBookSpecs()
    {
        return InvokePublicFactories(typeof(BookValidationSpecs));
    }

    private static IReadOnlyList<ValidationSpec> GetClassSpecs()
    {
        return InvokePublicFactories(typeof(ClassValidationSpecs));
    }

    private static IReadOnlyList<ValidationSpec> GetConditionFormSpecs()
    {
        return InvokePublicFactories(typeof(ConditionFormValidationSpecs));
    }

    private static IReadOnlyList<ValidationSpec> GetConstructibleObjectSpecs()
    {
        return InvokePublicFactories(typeof(ConstructibleObjectValidationSpecs));
    }

    private static IReadOnlyList<ValidationSpec> GetContainerSpecs()
    {
        return InvokePublicFactories(typeof(ContainerValidationSpecs));
    }

    private static IReadOnlyList<ValidationSpec> GetDoorSpecs()
    {
        return InvokePublicFactories(typeof(DoorValidationSpecs));
    }

    private static IReadOnlyList<ValidationSpec> GetFactionSpecs()
    {
        return InvokePublicFactories(typeof(FactionValidationSpecs));
    }

    private static IReadOnlyList<ValidationSpec> GetFormListSpecs()
    {
        return InvokePublicFactories(typeof(FormListValidationSpecs));
    }

    private static IReadOnlyList<ValidationSpec> GetGameSettingSpecs()
    {
        return InvokePublicFactories(typeof(GameSettingValidationSpecs));
    }

    private static IReadOnlyList<ValidationSpec> GetGlobalSpecs()
    {
        return InvokePublicFactories(typeof(GlobalValidationSpecs));
    }

    private static IReadOnlyList<ValidationSpec> GetKeywordSpecs()
    {
        return InvokePublicFactories(typeof(KeywordValidationSpecs));
    }

    private static IReadOnlyList<ValidationSpec> GetMagicEffectSpecs()
    {
        return InvokePublicFactories(typeof(MagicEffectValidationSpecs));
    }

    private static IReadOnlyList<ValidationSpec> GetMiscItemSpecs()
    {
        return InvokePublicFactories(typeof(MiscItemValidationSpecs));
    }

    private static IReadOnlyList<ValidationSpec> GetNPCSpecs()
    {
        return InvokePublicFactories(typeof(NPCValidationSpecs));
    }

    private static IReadOnlyList<ValidationSpec> GetPerkSpecs()
    {
        return InvokePublicFactories(typeof(PerkValidationSpecs));
    }

    private static IReadOnlyList<ValidationSpec> GetStaticSpecs()
    {
        return InvokePublicFactories(typeof(StaticValidationSpecs));
    }

    private static IReadOnlyList<ValidationSpec> GetTerminalSpecs()
    {
        return InvokePublicFactories(typeof(TerminalValidationSpecs));
    }

    private static IReadOnlyList<ValidationSpec> InvokePublicFactories(Type specificationType)
    {
        return specificationType
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly)
            .Where(method => method.ReturnType == typeof(ValidationSpec) && method.GetParameters().Length == 0)
            .OrderBy(method => method.Name, StringComparer.Ordinal)
            .Select(method => (ValidationSpec)method.Invoke(null, [])!)
            .ToList();
    }
}

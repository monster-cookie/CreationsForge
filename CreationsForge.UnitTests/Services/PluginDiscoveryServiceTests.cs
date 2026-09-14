using CreationsForge.Core.Services;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

/// <summary>Verifies installed-plugin dependency ordering before GUI workspace activation.</summary>
public sealed class PluginDiscoveryServiceTests
{
    /// <summary>Verifies a transitive Bethesda master is placed before the user plugin that declares it.</summary>
    [Fact]
    public void TryGetRequiredMastersInLoadOrder_WithTransitiveMaster_OrdersMasterBeforeDependent()
    {
        var starfield = ModKey.FromNameAndExtension("Starfield.esm");
        var sfbgs007 = ModKey.FromNameAndExtension("sfbgs007.esm");
        var dynamicScenes = ModKey.FromNameAndExtension("Venworks-DynamicScenesEngine.esm");
        var encounters = ModKey.FromNameAndExtension("Venworks-EncountersOverhaul.esm");
        var otherMaster = ModKey.FromNameAndExtension("Venworks-Core.esm");
        IReadOnlyDictionary<ModKey, IReadOnlyList<ModKey>> masterKeys =
            new Dictionary<ModKey, IReadOnlyList<ModKey>>
            {
                [starfield] = [],
                [sfbgs007] = [starfield],
                [dynamicScenes] = [sfbgs007],
                [otherMaster] = [starfield],
                [encounters] = [dynamicScenes, otherMaster]
            };

        var succeeded = PluginDiscoveryService.TryGetRequiredMastersInLoadOrder(
            encounters,
            masterKeys,
            out var orderedMasters);

        succeeded.ShouldBeTrue();
        orderedMasters.ShouldBe([starfield, sfbgs007, dynamicScenes, otherMaster]);
    }
}

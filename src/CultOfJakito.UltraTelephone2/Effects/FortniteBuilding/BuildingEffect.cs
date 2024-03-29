using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2.Effects.FortniteBuilding;

[HarmonyPatch]
[RegisterChaosEffect]
public class BuildingEffect : ChaosEffect
{
    [Configgable("Chaos/Effects", "Fortnite Building")]
    private static ConfigToggle s_enabled = new(true);
    private static bool s_currentlyActive;

    public override void BeginEffect(UniRandom random)
    {
        s_currentlyActive = true;
        NewMovement.Instance.gameObject.AddComponent<BuildingControls>();
    }

    public override void Dispose()
    {
        s_currentlyActive = false;
        Destroy(NewMovement.Instance.gameObject.GetComponent<BuildingControls>());
    }

    public override int GetEffectCost() => 5;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
}

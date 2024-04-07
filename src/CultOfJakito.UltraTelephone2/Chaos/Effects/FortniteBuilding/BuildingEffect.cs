using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.FortniteBuilding;

[HarmonyPatch]
[RegisterChaosEffect]
public class BuildingEffect : ChaosEffect
{
    [Configgable("Chaos/Effects/Fortnite Building", "Fortnite Building")]
    private static ConfigToggle s_enabled = new(true);
    public static bool CurrentlyActive { get; private set; }

    public override void BeginEffect(UniRandom random)
    {
        CurrentlyActive = true;
        NewMovement.Instance.gameObject.AddComponent<BuildingControls>();
        BuildingHud.Create();
    }

    public override void Dispose()
    {
        CurrentlyActive = false;
        Destroy(NewMovement.Instance.gameObject.GetComponent<BuildingControls>());
    }

    protected override void OnDestroy() => CurrentlyActive = false;


    public override int GetEffectCost() => 3;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
}

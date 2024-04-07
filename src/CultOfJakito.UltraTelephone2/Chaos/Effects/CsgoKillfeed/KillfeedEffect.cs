using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.CsgoKillfeed;

[HarmonyPatch]
[RegisterChaosEffect]
public class KillfeedEffect : ChaosEffect
{
    [Configgable("Chaos/Effects", "CSGO Killfeed")]
    private static ConfigToggle s_enabled = new(true);
    private static bool s_currentlyActive;

    public override void BeginEffect(UniRandom random)
    {
        s_currentlyActive = true;
        Instantiate(UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/CSGO Killfeed/Killfeed.prefab"), CanvasController.Instance.transform);
    }

    public override void Dispose()
    {
        s_currentlyActive = false;
    }

    protected override void OnDestroy() => s_currentlyActive = false;


    public override int GetEffectCost() => 3;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

    [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.DeliverDamage)), HarmonyPrefix]
    private static void SetState(EnemyIdentifier __instance, ref bool __state)
    {
        __state = __instance.dead;
    }

    [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.DeliverDamage)), HarmonyPostfix]
    private static void CreateKillFeed(EnemyIdentifier __instance, ref bool __state, GameObject sourceWeapon)
    {
        if (s_currentlyActive && !__state && __instance.dead && sourceWeapon != null)
        {
            Killfeed.Instance.CreateKillfeedEntry(__instance, sourceWeapon);
        }
    }
}

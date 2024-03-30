using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public class RealisticExplosions : ChaosEffect
{
    [Configgable("Chaos/Effects", "Realistic Explosions")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_currentlyActive;

    private static float s_timeSinceLastTick;

    private static AudioClip Sound => UT2Assets.GetAsset<AudioClip>("Assets/Telephone 2/Misc/Sounds/Explosion.mp3");

    public override void BeginEffect(UniRandom random)
    {
        s_currentlyActive = true;
    }

    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 1;

    [HarmonyPatch(typeof(Explosion), nameof(Explosion.Start)), HarmonyPostfix]
    public static void SoundReplacement(Explosion __instance)
    {
        if (!s_currentlyActive && !s_enabled.Value)
            return;
        __instance.GetComponent<AudioSource>().PlayOneShot(Sound);
    }

    protected override void OnDestroy() => s_currentlyActive = false;
}

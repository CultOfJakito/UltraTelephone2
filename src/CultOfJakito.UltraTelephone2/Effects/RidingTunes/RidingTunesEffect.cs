using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Zed
{
    [RegisterChaosEffect]
    [HarmonyPatch]
    public class RidingTunes : ChaosEffect
    {
        [Configgable("Chaos/Rocket Riding Tunes", "Rocket Riding Music")]
        private static ConfigToggle s_enabled = new(true);

        private static bool s_currentlyActive = false;

        public override void BeginEffect(UniRandom random) => s_currentlyActive = true;

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && GeneralSettings.EnableCopyrightedMusic.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost() => 1;

        public override void Dispose()
        {
            s_currentlyActive = false;
            base.Dispose();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Grenade), nameof(Grenade.PlayerRideStart))]
        public static void Riding(Grenade __instance)
        {
            if (!s_currentlyActive || !GeneralSettings.EnableCopyrightedMusic.Value)
                return;

            if (!__instance.TryGetComponent(out MusicalRocket music))
                music = __instance.gameObject.AddComponent<MusicalRocket>();

            music.Play();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Grenade), nameof(Grenade.PlayerRideEnd))]
        public static void RidingEnd(Grenade __instance)
        {
            if (!s_currentlyActive)
                return;

            if (__instance.TryGetComponent(out MusicalRocket music))
            {
                music.Pause();
            }
        }
    }
}

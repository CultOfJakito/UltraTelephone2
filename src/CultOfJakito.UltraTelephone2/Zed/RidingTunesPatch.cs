using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Zed
{
    [PatchThis("CultOfJakito.UltraTelephone2.Zed.RidingTunesPatch")]
    public class RidingTunesPatch
    {

        [Configgable("ZedDev/Patches", "Rocket Riding Music")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [HarmonyPatch(typeof(Grenade), "PlayerRideStart")]
        [HarmonyPostfix]
        public static void Riding(Grenade __instance)
        {
            if (!s_enabled.Value)
                return;

            AudioClip audio = UT2Assets.ZedBundle.LoadAsset<AudioClip>("ridingtunes");
            if(__instance.GetComponentsInChildren<Marker>().Any(s => s.Name == "PlaySound"))
            {
                AudioSource audioSource = __instance.GetComponentsInChildren<Marker>().Where(s => s.Name == "PlaySound").First().gameObject.GetComponent<AudioSource>();   
                audioSource.UnPause();
            }
            else
            {
                audio?.PlaySound(parent: __instance.transform, keepSource: true, loop: true);
            }
        }

        [HarmonyPatch(typeof(Grenade), "PlayerRideEnd")]
        [HarmonyPostfix]
        public static void RidingEnd(Grenade __instance)
        {
            if (!s_enabled.Value)
                return;

            if(__instance.GetComponentsInChildren<Marker>().Any(s => s.Name == "PlaySound"))
            {
                AudioSource audioSource = __instance.GetComponentsInChildren<Marker>().Where(s => s.Name == "PlaySound").First().gameObject.GetComponent<AudioSource>();   
                audioSource.Pause();
            }
        }
    }
}

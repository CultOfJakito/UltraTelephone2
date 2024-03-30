using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Patches;

[HarmonyPatch]
public class VineboomMauricePatch
{
    [Configgable("Patches", "Maurice Vine Boom")]
    private static ConfigToggle s_enabled = new ConfigToggle(true);

    [HarmonyPatch(typeof(EnemyIdentifier), "OnEnable")]
    [HarmonyPostfix]
    public static void PlaySound(EnemyIdentifier __instance)
    {
        if (!s_enabled.Value)
            return;

        if (!__instance.GetComponent<SpiderBody>())
            return;

        AudioClip vineboom = UT2Assets.GetAsset<AudioClip>("Assets/Telephone 2/Misc/Sounds/vineboom.mp3");

        if (vineboom != null)
        {
            Debug.Log("Playing vineboom");
            vineboom.PlaySound(position: __instance.transform.position);
        }
        else
        {
            Debug.LogError("Vineboom clip not found");
        }
    }
}

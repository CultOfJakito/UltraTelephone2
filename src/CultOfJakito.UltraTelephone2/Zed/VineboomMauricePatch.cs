using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Zed;

[PatchThis("CultOfJakito.UltraTelephone2.Zed.VineboomMauricePatch")]
public class VineboomMauricePatch
{
    [Configgable("Zed/Patches", "Vine boom maurice")]
    private static ConfigToggle s_enabled = new ConfigToggle(true);

    [HarmonyPatch(typeof(EnemyIdentifier), "OnEnable")]
    [HarmonyPostfix]
    public static void PlaySound(EnemyIdentifier __instance)
    {
        if (!s_enabled.Value)
            return;

        if(!__instance.GetComponent<SpiderBody>()) return;
        AudioClip vineboom = UT2Assets.ZedBundle.LoadAsset<AudioClip>("vineboom");

        if(vineboom != null)
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

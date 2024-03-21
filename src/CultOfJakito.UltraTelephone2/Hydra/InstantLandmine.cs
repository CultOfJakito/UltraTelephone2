using Configgy;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2.Hydra;

[HarmonyPatch]
public static class InstantLandmine
{
    [Configgable("Hydra/Patches", "Realistic Landmines")]
    private static ConfigToggle s_enabled = new(true);


    [HarmonyPatch(typeof(Landmine), nameof(Landmine.Activate))]
    [HarmonyPostfix]
    public static void OnActivate(Landmine __instance)
    {
        if (!s_enabled.Value)
            return;

        //Reflection is being dumb so idfc cry about it
        __instance.Invoke("Explode", 0f);
    }
}

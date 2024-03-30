using Configgy;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2.Patches;

[HarmonyPatch]
public static class InstantLandmine
{
    [Configgable("Patches", "Realistic Landmines")]
    private static ConfigToggle s_enabled = new(true);


    [HarmonyPatch(typeof(Landmine), nameof(Landmine.Activate))]
    [HarmonyPostfix]
    public static void OnActivate(Landmine __instance)
    {
        if (!s_enabled.Value)
            return;

        __instance.Explode();
    }
}

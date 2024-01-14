using Configgy;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2.Hydra.FakePBank.Patches;

[HarmonyPatch]
public static class InstantLandmine
{
    [Configgable("Hydra/Tweaks", "Realistic Landmines")]
    private static ConfigToggle s_enabled = new(true);


    [HarmonyPatch(typeof(Landmine), nameof(Landmine.Activate))]
    [HarmonyPostfix]
    public static void OnActivate(Landmine instance, bool activated)
    {
        if (!s_enabled.Value)
        {
            return;
        }

        //Reflection is being dumb so idfc cry about it
        instance.Invoke("Explode", 0f);
    }
}

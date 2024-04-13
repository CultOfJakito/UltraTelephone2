using CultOfJakito.UltraTelephone2.Placeholders;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2.Patches
{
    [HarmonyPatch]
    public static class BookPlaceholderParser
    {

        [HarmonyPatch(typeof(ScanningStuff), nameof(ScanningStuff.ScanBook)), HarmonyPrefix]
        [HarmonyPriority(100)]
        private static void OnScanBook(ScanningStuff __instance, ref string text)
        {
            text = PlaceholderHelper.ReplacePlaceholders(text);
        }
    }
}

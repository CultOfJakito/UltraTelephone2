using CultOfJakito.UltraTelephone2.Assets;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Patches;

[HarmonyPatch]
public class TextDestruction
{
    private static Font[] s_fonts =
    {
        UT2Assets.GetAsset<Font>("Assets/Telephone 2/Fonts/ComicSans.ttf"),
        UT2Assets.GetAsset<Font>("Assets/Telephone 2/Fonts/HeartlessFont.ttf"),
        UT2Assets.GetAsset<Font>("Assets/Telephone 2/Fonts/Papyrus.ttf"),
        UT2Assets.GetAsset<Font>("Assets/Telephone 2/Fonts/Times New Roman.ttf"),
        UT2Assets.GetAsset<Font>("Assets/Telephone 2/Fonts/Impact.ttf")
    };

    private static TMP_FontAsset[] s_tmpFonts =
    {
        UT2Assets.GetAsset<TMP_FontAsset>("Assets/Telephone 2/Fonts/TMP/ComicSans.asset"),
        UT2Assets.GetAsset<TMP_FontAsset>("Assets/Telephone 2/Fonts/TMP/HeartlessFont.asset"),
        UT2Assets.GetAsset<TMP_FontAsset>("Assets/Telephone 2/Fonts/TMP/Papyrus.asset"),
        UT2Assets.GetAsset<TMP_FontAsset>("Assets/Telephone 2/Fonts/TMP/Times New Roman.asset"),
        UT2Assets.GetAsset<TMP_FontAsset>("Assets/Telephone 2/Fonts/TMP/Impact.asset")
    };

    private static List<UnityEngine.Object> s_checkedTexts = new();
    private static UniRandom s_rand = UniRandom.SessionNext();

    public static void Initialize()
    {
        InGameCheck.OnLevelChanged += s => s_checkedTexts.Clear();
    }

    private static string Uwufy(string str) => str.ToLower().Replace("r", "w").Replace("l", "w").Replace("<cowow", "<color") + (s_rand.Chance(0.25f) ? " :3" : string.Empty);

    [HarmonyPatch(typeof(TextMeshProUGUI), nameof(TextMeshProUGUI.OnEnable)), HarmonyPostfix]
    private static void SwapTmp(TextMeshProUGUI __instance)
    {
        if (s_checkedTexts.Contains(__instance))
        {
            return;
        }

        if (s_rand.Chance(0.25f))
        {
            __instance.font = s_rand.SelectRandom(s_tmpFonts);
        }

        if (s_rand.Chance(0.25f))
        {
            __instance.text = Uwufy(__instance.text);
        }

        s_checkedTexts.Add(__instance);
    }

    [HarmonyPatch(typeof(Text), nameof(Text.OnEnable)), HarmonyPostfix]
    private static void SwapText(Text __instance)
    {
        if (s_checkedTexts.Contains(__instance))
        {
            return;
        }

        if (s_rand.Chance(0.25f))
        {
            __instance.font = s_rand.SelectRandom(s_fonts);
        }

        if (s_rand.Chance(0.25f))
        {
            __instance.text = Uwufy(__instance.text);
        }

        s_checkedTexts.Add(__instance);
    }
}

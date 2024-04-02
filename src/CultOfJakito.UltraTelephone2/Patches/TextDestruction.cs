using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Patches;

[HarmonyPatch]
public class TextDestruction
{
    [Configgable("Patches/Text Destruction", "Font Changer")]
    private static bool s_fontChangeEnabled = true;
    [Configgable("Patches/Text Destruction", "Uwufier")]
    private static bool s_uwufierEnabled = true;

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

    private static string Uwufy(string str)
    {
        string result = string.Empty;
        bool inTag = false;

        foreach (char character in str)
        {
            if (!inTag)
            {
                result += character.ToString().ToLower() is "r" or "l" ? 'w' : character;
                if (character == '<')
                {
                    inTag = true;
                }
                continue;
            }

            result += character;
            if (character == '>')
            {
                inTag = false;
            }
        }

        return result.ToLower() + (s_rand.Chance(0.25f) ? " " + s_rand.SelectRandom([":3", ";3", "^_^", ">_<", ">//<"]) : string.Empty);
    }

    [HarmonyPatch(typeof(TextMeshProUGUI), nameof(TextMeshProUGUI.OnEnable)), HarmonyPostfix]
    private static void SwapTmp(TextMeshProUGUI __instance)
    {
        if (s_checkedTexts.Contains(__instance))
        {
            return;
        }

        if (s_rand.Chance(0.25f) && s_fontChangeEnabled)
        {
            __instance.font = s_rand.SelectRandom(s_tmpFonts);
            __instance.enableWordWrapping = true;
        }

        if (s_rand.Chance(0.25f) && s_uwufierEnabled)
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

        if (s_rand.Chance(0.25f) && s_fontChangeEnabled)
        {
            __instance.font = s_rand.SelectRandom(s_fonts);
            __instance.resizeTextForBestFit = true;
        }

        if (s_rand.Chance(0.25f) && s_uwufierEnabled)
        {
            __instance.text = Uwufy(__instance.text);
        }

        s_checkedTexts.Add(__instance);
    }
}

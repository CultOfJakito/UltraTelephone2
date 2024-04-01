using Configgy;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Patches
{

    [HarmonyPatch]
    public static class PartyColoredMenu
    {

        [Configgable("Patches", "Party Colored Menus")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [HarmonyPatch(typeof(CanvasController), nameof(CanvasController.Awake)), HarmonyPostfix]
        [HarmonyPriority(200)]
        private static void OnAwake(CanvasController __instance)
        {
            if (!s_enabled.Value)
                return;

            PartifyMenu(__instance.GetComponent<RectTransform>());
        }

        [HarmonyPatch(typeof(ShopZone), nameof(ShopZone.Start)), HarmonyPostfix]
        [HarmonyPriority(200)]
        private static void OnStart(ShopZone __instance)
        {
            if (!s_enabled.Value)
                return;

            if (__instance.shopCanvas == null)
                return;

            //No casino, the colors are important for gameplay
            if (SceneHelper.CurrentScene == "CASINO")
                return;

            PartifyMenu(__instance.shopCanvas.GetComponent<RectTransform>());
        }


        private static void PartifyMenu(RectTransform root)
        {
            UniRandom random = new UniRandom(new SeedBuilder()
                .WithGlobalSeed()
                .WithSceneName()
                .WithSeed(nameof(PartyColoredMenu))
                .WithSeed(root.name));

            foreach (TextMeshProUGUI tmp in root.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                tmp.color = random.Color(1f, 1f, 1f);
            }

            foreach (Slider slider in root.GetComponentsInChildren<Slider>(true))
            {
                if (slider.targetGraphic != null)
                {
                    slider.colors = GenerateColorBlock(random);
                }

                slider.onValueChanged.AddListener((value) =>
                {
                    slider.colors = GenerateColorBlock(random);
                });
            }

            foreach (Toggle toggle in root.GetComponentsInChildren<Toggle>(true))
            {
                if (toggle.targetGraphic != null)
                {
                    toggle.colors = GenerateColorBlock(random);
                }

                toggle.onValueChanged.AddListener((value) =>
                {
                    toggle.colors = GenerateColorBlock(random);
                });
            }


            foreach (Button button in root.GetComponentsInChildren<Button>(true))
            {
                if (button.targetGraphic != null)
                {
                    button.colors = GenerateColorBlock(random);
                }

                TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    if (random.Bool())
                    {
                        Color buttonColor = button.colors.normalColor;
                        Color opposite = new Color(1f - buttonColor.r, 1f - buttonColor.g, 1f - buttonColor.b);
                        text.color = opposite;
                    }
                    else
                    {
                        text.color = random.Color(1f, 1f, 1f);
                    }
                }

                button.onClick.AddListener(() =>
                {
                    button.colors = GenerateColorBlock(random);

                    if(text != null)
                    {
                        if (random.Bool())
                        {
                            Color buttonColor = button.colors.normalColor;
                            Color opposite = new Color(1f - buttonColor.r, 1f - buttonColor.g, 1f - buttonColor.b);
                            text.color = opposite;
                        }
                        else
                        {
                            text.color = random.Color(1f, 1f, 1f);
                        }
                    }
                });
            }
        }

        private static ColorBlock GenerateColorBlock(UniRandom random)
        {
            ColorBlock colorBlock = new ColorBlock();
            colorBlock.normalColor = random.Color(1f,0.5f,1f);
            colorBlock.highlightedColor = random.Color(1f,1f,1f);
            colorBlock.pressedColor = colorBlock.normalColor * 0.7f;
            colorBlock.disabledColor = colorBlock.normalColor * 0.5f;
            colorBlock.colorMultiplier = 1f;
            return colorBlock;
        }
    }
}

using Configgy;
using CultOfJakito.UltraTelephone2.Resources;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Zed;

public class MinecraftBookPatch
{
    [Configgable("ZedDev/Patches", "Minecraft Book")]
    private static ConfigToggle s_enabled = new(true);

    private static Sprite? s_minecraftBookSprite;
    public static void Init()
    {
        Texture2D texture = new Texture2D(146, 180);
        texture.LoadImage(Resource1.book);
        texture.filterMode = FilterMode.Point;
        s_minecraftBookSprite = Sprite.Create(texture, new Rect(0, 0, 146, 180), new Vector2(0.5f, 0.5f));
        SceneManager.sceneLoaded += Apply;
    }

    public static void Apply(Scene scene, LoadSceneMode mode)
    {
        if (!s_enabled.Value)
            return;

        Transform scanning = CanvasController.Instance?.transform.Find("ScanningStuff");

        if (scanning != null)
        {
            Image img = scanning.GetComponent<ScanningStuff>().readingPanel.GetComponent<Image>();
            img.rectTransform.SetAnchor(AnchorPresets.MiddleCenter, 0, 0);
            img.rectTransform.SetPivot(PivotPresets.MiddleCenter);
            img.rectTransform.sizeDelta = new Vector2(146 * 3f, 180 * 3f);
            img.sprite = s_minecraftBookSprite;
            img.color = new Color(1, 1, 1, 1f);
            img.rectTransform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0f);

            RectTransform panel = img.rectTransform.GetChild(0).GetComponent<RectTransform>();
            panel.SetAnchor(AnchorPresets.StretchAll);
            panel.SetRect(new Rect4(10, 20, 20, -20));

            ScrollRect scrollRect = img.rectTransform.GetComponentInChildren<ScrollRect>();
            foreach (Image image in scrollRect.verticalScrollbar.GetComponentsInChildren<Image>())
            {
                image.color = Color.clear;
            }

            TMP_Text text = scanning.GetComponent<ScanningStuff>().readingText;
            text.color = Color.black;
            text.fontSize = 20;
        }
    }
}

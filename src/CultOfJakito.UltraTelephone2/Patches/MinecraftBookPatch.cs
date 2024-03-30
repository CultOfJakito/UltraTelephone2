using System.Reflection;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Patches;

public class MinecraftBookPatch
{
    [Configgable("Patches", "Minecraft Book")]
    private static ConfigToggle s_enabled = new(true);
    private static Sprite s_minecraftBookSprite;

    public static void Init()
    {
        SceneManager.sceneLoaded += Apply;
    }

    //Publicization is not working for me, so I'm just gonna do a heckin reflection
    private static FieldInfo s_readingPanelField = typeof(ScanningStuff).GetField("readingPanel", BindingFlags.NonPublic | BindingFlags.Instance);
    private static FieldInfo s_readingTextField = typeof(ScanningStuff).GetField("readingText", BindingFlags.NonPublic | BindingFlags.Instance);

    private static GameObject GetReadingPanel(ScanningStuff instance)
    {
        return (GameObject)s_readingPanelField.GetValue(instance);
    }

    private static TMP_Text GetReadingText(ScanningStuff instance)
    {
        return (TMP_Text)s_readingTextField.GetValue(instance);
    }

    public static void Apply(Scene scene, LoadSceneMode mode)
    {
        if (!s_enabled.Value)
            return;

        if (!InGameCheck.InLevel())
            return;

        ScanningStuff scanningStuff = ScanningStuff.Instance;
        if (scanningStuff == null)
            return;

        s_minecraftBookSprite ??= UT2Assets.GetAsset<Sprite>("Assets/Telephone 2/MC Book/book.png");

        Image img = GetReadingPanel(scanningStuff).GetComponent<Image>();
        img.rectTransform.SetAnchor(AnchorPresets.MiddleCenter, 0, 0);
        img.rectTransform.SetPivot(PivotPresets.MiddleCenter);
        img.rectTransform.sizeDelta = new Vector2(146 * 3f, 180 * 3f);
        img.sprite = s_minecraftBookSprite;
        img.color = new Color(1, 1, 1, 1f);
        img.rectTransform.GetChild(0).GetComponent<Image>().color = Color.clear;

        RectTransform panel = img.rectTransform.GetChild(0).GetComponent<RectTransform>();
        panel.SetAnchor(AnchorPresets.StretchAll);
        panel.SetRect(new Rect4(10, 20, 20, -20));

        ScrollRect scrollRect = img.rectTransform.GetComponentInChildren<ScrollRect>();
        foreach (Image image in scrollRect.verticalScrollbar.GetComponentsInChildren<Image>())
        {
            image.color = Color.clear;
        }

        TMP_Text text = GetReadingText(scanningStuff);
        text.color = Color.black;
        text.fontSize = 20;
    }
}

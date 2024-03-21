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

    private static Sprite? minecraftBookSprite;
    public static void Init()
    {
        Texture2D texture = new Texture2D(146, 180);
        texture.LoadImage(Resource1.book);
        texture.filterMode = FilterMode.Point;
        minecraftBookSprite = Sprite.Create(texture, new Rect(0, 0, 146, 180), new Vector2(0.5f, 0.5f));
        SceneManager.sceneLoaded += Apply;
    }

    public static void Apply(Scene scene, LoadSceneMode mode)
    {
        if (!s_enabled.Value)
            return;

        GameObject canvas = scene.GetRootGameObjects().Where(go => go.name == "Canvas").FirstOrDefault();
        if (canvas != null)
        {
            Transform scanning = canvas.transform.Find("ScanningStuff");
            if (scanning != null)
            {
                //scanning.GetComponent<ScanningStuff>().readingPanel.GetComponent<Image>();
                Image img = Traverse.Create(scanning.GetComponent<ScanningStuff>()).Field<GameObject>("readingPanel").Value.GetComponent<Image>();
                img.rectTransform.SetAnchor(AnchorPresets.MiddleCenter, 0, 0);
                img.rectTransform.SetPivot(PivotPresets.MiddleCenter);
                img.rectTransform.sizeDelta = new Vector2(146 * 3f, 180 * 3f);
                img.sprite = minecraftBookSprite;
                img.color = new Color(1, 1, 1, 1f);
                img.rectTransform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0f);
                RectTransform panel = img.rectTransform.GetChild(0).GetComponent<RectTransform>();
                panel.SetAnchor(AnchorPresets.StretchAll);
                panel.SetRect(new Rect4(10, 20, 20, -20));
                ScrollRect scrollRect = img.rectTransform.GetComponentInChildren<ScrollRect>();
                scrollRect.verticalScrollbar.GetComponentsInChildren<Image>().ForEach(i => i.color = new Color(1, 1, 1, 0));
                TMP_Text text = Traverse.Create(scanning.GetComponent<ScanningStuff>()).Field<TMP_Text>("readingText").Value;
                text.color = new Color(0, 0, 0, 1);
                text.fontSize = 20;

            }
        }
    }
}

using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Patches
{
    [HarmonyPatch]
    public static class WackyCursor
    {
        private static UniRandom s_rng;

        [Configgable("Fun", "Wacky Cursor")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        private static CursorDatabase s_cursorDatabase;

        public static void Init()
        {
            s_rng = new UniRandom(new SeedBuilder().WithGlobalSeed()
                .WithSceneName()
                .WithSeed(nameof(WackyCursor)));

            s_cursorDatabase = HydraAssets.CursorDatabase;

            s_enabled.OnValueChanged += (enabled) =>
            {
                if (enabled)
                    RandomizeCursor();
                else
                    ResetCursor();
            };

            if (s_enabled.Value)
                RandomizeCursor();
        }

        public static void RandomizeCursor()
        {
            if (!s_enabled.Value)
                return;

            CursorAsset cursorAsset = s_rng.SelectRandom(s_cursorDatabase.Cursors);
            Cursor.SetCursor(cursorAsset.Texture, cursorAsset.Hotspot, CursorMode.Auto);
        }

        public static void ResetCursor()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        [HarmonyPatch(typeof(CanvasController), nameof(CanvasController.Awake)), HarmonyPostfix]
        [HarmonyPriority(201)]
        private static void OnAwake(CanvasController __instance)
        {
            AddListenersToChangeCursor(__instance.GetComponent<RectTransform>());
        }

        private static void AddListenersToChangeCursor(RectTransform root)
        {
            UniRandom random = new UniRandom(new SeedBuilder()
                .WithGlobalSeed()
                .WithSceneName()
                .WithSeed(nameof(WackyCursor)));

            foreach (Toggle toggle in root.GetComponentsInChildren<Toggle>(true))
            {
                toggle.onValueChanged.AddListener((value) =>
                {
                    RandomizeCursor();
                });
            }


            foreach (Button button in root.GetComponentsInChildren<Button>(true))
            {
                button.onClick.AddListener(() =>
                {
                    RandomizeCursor();
                });
            }

        }
    }

    public class CursorDatabase : ScriptableObject
    {
        public CursorAsset[] Cursors;
    }

    public class CursorAsset : ScriptableObject
    {
        public Texture2D Texture;
        public Vector2 Hotspot;
    }
}

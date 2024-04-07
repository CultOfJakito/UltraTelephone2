using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Fun;

[HarmonyPatch]
public class CrueltySquadBorder : MonoSingleton<CrueltySquadBorder>
{
    [Configgable("Fun", "Cruelty Squad Border")]
    private static ConfigToggle s_enabled = new(true);
    private static UniRandom s_rand;

    public Image Image;
    public Sprite[] Sprites;

    [HarmonyPatch(typeof(CanvasController), nameof(CanvasController.Awake)), HarmonyPostfix]
    private static void CreateJumpscare()
    {
        if (!s_enabled.Value || instance != null)
        {
            return;
        }

        Instantiate(UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Cruelty Squad/CrueltySquad.prefab"));
    }

    private void Start()
    {
        s_rand ??= UniRandom.SessionNext();
        Image.sprite = s_rand.SelectRandom(Sprites);
    }
}

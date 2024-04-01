using System.Collections;
using CultOfJakito.UltraTelephone2.Assets;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.FortniteBuilding;

[HarmonyPatch]
public class BuildingHud : MonoSingleton<BuildingHud>
{
    public Image[] Outlines;

    public static void Create()
    {
        Instantiate(UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Fortnite Builds/Fortnite HUD.prefab"), CanvasController.Instance.transform);
    }

    public void SelectOutline(BuildTypes build)
    {
        foreach (Image image in Outlines)
        {
            StartCoroutine(Transition(image, image == Outlines[(int)build] ? Color.white : new(1, 1, 1, 0.2f)));
        }
    }

    private IEnumerator Transition(Image image, Color colour)
    {
        while (image.color != colour)
        {
            image.color = Vector4.MoveTowards(image.color, colour, Time.deltaTime * 5);
            yield return null;
        }
    }
}

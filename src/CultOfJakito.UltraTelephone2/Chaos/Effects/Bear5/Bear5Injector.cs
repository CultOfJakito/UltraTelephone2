using CultOfJakito.UltraTelephone2.Assets;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.Bear5;

[HarmonyPatch(typeof(SpawnMenu))]
public static class Bear5Injector
{
    private static bool injected;

    [HarmonyPatch("Awake"), HarmonyPrefix]
    private static void Start(ref SpawnableObjectsDatabase ___objects)
    {
        //Only add our content once, since the ScriptableObject's data will persist between scene loads.
        if (injected)
            return;

        SpawnableObject bear5Object = UT2Assets.GetAsset<SpawnableObject>("Assets/Telephone 2/Bear5/Bear5Spawnable.asset");

        SpawnableObject[] enemies = ___objects.enemies;
        SpawnableObject[] newEnemies = new SpawnableObject[enemies.Length + 1];
        Array.Copy(enemies, newEnemies, enemies.Length);
        newEnemies[newEnemies.Length - 1] = bear5Object;
        ___objects.enemies = newEnemies;

        injected = true;
    }

    [HarmonyPatch(nameof(SpawnMenu.RebuildIcons)), HarmonyPostfix]
    private static void AddEnemyIcon(ref Dictionary<string, Sprite> ___spriteIcons)
    {
        ___spriteIcons.Add("ut2.icons.bear5", UT2Assets.GetAsset<Sprite>("Assets/Telephone 2/Bear5/BEAR5Icon.png"));
    }


}

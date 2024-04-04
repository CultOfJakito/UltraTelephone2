using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.MemePaintings;

[RegisterChaosEffect]
public sealed class MemePaintings : ChaosEffect
{
    private static string[] s_paintingMaterials = [
        "BingChilling",
        "BreadCup",
        "Dont",
        "GordonFreeman",
        "Loss",
        "SleepyJoe",
        "True",
        "Weedworks",
    ];

    // Extremely fucked up but I can't find a better way to do this
    private static HashSet<string> s_originalMaterialNames = [
        "Default",
        "Ferryman",
        "FerrymanGabriel",
        "FerrymanGabrielDestroyed",
        "FerrymanRocket",
        "KingMinos",
        "KingSisyphus",
    ];

    [Configgable("Chaos/Effects", "Meme Paintings")]
    private static ConfigToggle s_enabled = new ConfigToggle(true);
    public override void BeginEffect(UniRandom random)
    {
        s_effectActive = true;

        /*
        OriginalPaintingMaterialData materialData = UT2Assets.GetAsset<OriginalPaintingMaterialData>("Assets/Telephone 2/MemePaintings/OriginalMaterialData.asset");
        if(materialData == null)
        {
            Debug.LogError("ROT");
            return;
        }
        */

        Debug.Log("Querying every single mesh renderer; this might take a while");
        foreach(MeshRenderer meshRenderer in SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<MeshRenderer>()))
        {
            if(meshRenderer.gameObject.hideFlags.HasFlag(HideFlags.HideAndDontSave))
            {
                continue;
            }
            if(meshRenderer.gameObject.GetComponentInParent<NewMovement>() != null)
            {
                continue;
            }
            if(meshRenderer.gameObject.GetComponentInParent<EnemyIdentifier>() != null)
            {
                continue;
            }

            for(int i=0; i<meshRenderer.materials.Length; i++)
            {
                string name = meshRenderer.materials[i].name;
                if(name.EndsWith(" (Instance)"))
                {
                    name = name.Substring(0, name.Length - " (Instance)".Length);
                }
                if (s_originalMaterialNames.Contains(name))
                {
                    Debug.Log($"Found {meshRenderer.gameObject} ({i})");

                    string key = $"Assets/Telephone 2/MemePaintings/Materials/{random.SelectRandomFromSet(s_paintingMaterials)}.mat";

                    meshRenderer.sharedMaterials = Enumerable.Repeat(UT2Assets.GetAsset<Material>(key), meshRenderer.materials.Length).ToArray();
                }
            }
        }
    }

    private static bool s_effectActive = false;

    public override int GetEffectCost() => 1;
    protected override void OnDestroy() {
        s_effectActive = false;
    }

    public override bool CanBeginEffect(ChaosSessionContext ctx) {
        if(!s_enabled.Value)
        {
            return false;
        }

        // This should be every level with a painting in it, this is from memory so I might me wrong
        return ctx.LevelName.EndsWith("4-3")
            || ctx.LevelName.EndsWith("4-4")
            || ctx.LevelName.EndsWith("5-2")
            || ctx.LevelName.EndsWith("5-3");
    }
}

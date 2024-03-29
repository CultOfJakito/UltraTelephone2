using System;
using System.IO;
using System.Reflection;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CultOfJakito.UltraTelephone2.Assets;

[HarmonyPatch]
public static class AddressableManager
{
    private static bool s_dontSanitizeScenes;
    public static string CatalogPath => Path.Combine(UT2Paths.InternalAssetsFolder, "catalog_wbp.json");
    public static string ModDataPath => Path.Combine(UT2Paths.InternalAssetsFolder, "data.json");
    public static string AssetPath => UT2Paths.InternalAssetsFolder;

    public static void LoadCatalog()
    {
        Addressables.LoadContentCatalogAsync(CatalogPath, false).WaitForCompletion();
    }

    public static void LoadSceneUnsanitzed(string path)
    {
        s_dontSanitizeScenes = true;

        try
        {
            SceneHelper.LoadScene(path);
        }
        catch (Exception ex)
        {
            // i hate using trycatch but if this isnt set back to false, every unmodded scene load will fail
            Debug.LogError(ex.ToString());
        }

        s_dontSanitizeScenes = false;
    }

    [HarmonyPatch(typeof(SceneHelper), nameof(SceneHelper.SanitizeLevelPath)), HarmonyPrefix]
    private static bool PreventSanitization(string scene, ref string __result)
    {
        if (s_dontSanitizeScenes)
        {
            __result = scene;
            return false;
        }

        return true;
    }
}


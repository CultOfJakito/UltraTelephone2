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
        ValidateFiles();
        Addressables.LoadContentCatalogAsync(CatalogPath, false).WaitForCompletion();
    }

    public static void LoadSceneUnsanitzed(string path)
    {
        s_dontSanitizeScenes = true;

        try
        {
            SceneHelper.LoadScene(path);
            SceneHelper.CurrentScene = Path.GetFileNameWithoutExtension(path);
        }
        catch (Exception ex)
        {
            // i hate using trycatch but if this isnt set back to false, every unmodded scene load will fail
            Debug.LogError(ex.ToString());
        }

        s_dontSanitizeScenes = false;
    }

    public static void ValidateFiles()
    {
        if(!Directory.Exists(AssetPath))
            Directory.CreateDirectory(AssetPath);

        //UNPACK THE BUILT-IN ASSETS
        ValidateFile(Path.Combine(AssetPath, "catalog_wbp.hash"), Properties.Resources.catalog_wbp);
        ValidateFile(Path.Combine(AssetPath, "catalog_wbp.json"), Properties.Resources.catalog_wbp1);
        ValidateFile(Path.Combine(AssetPath, "shader_unitybuiltinshaders.bundle"), Properties.Resources.shader_unitybuiltinshaders);
        ValidateFile(Path.Combine(AssetPath, "telephone2_assets_all.bundle"), Properties.Resources.telephone2_assets_all);
        ValidateFile(Path.Combine(AssetPath, "telephone2_scenes_all.bundle"), Properties.Resources.telephone2_scenes_all);
        ValidateFile(Path.Combine(AssetPath, "ultratelephone2_monoscripts.bundle"), Properties.Resources.ultratelephone2_monoscripts);
    }

    private static void ValidateFile(string filePath, byte[] data)
    {
        if (File.Exists(filePath))
            return;

        File.WriteAllBytes(filePath, data);
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


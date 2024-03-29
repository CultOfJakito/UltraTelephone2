using System;
using System.Collections.Generic;
using System.Text;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CultOfJakito.UltraTelephone2.Assets
{
    public static class UT2Assets
    {
        private static Dictionary<string, UnityEngine.Object> _loadedAssets = new();

        //ez
        //private static AssetLoader monoscripts = new AssetLoader(Properties.Resources.ultratelephone2_monoscripts);
        //private static AssetLoader backupLoader = new AssetLoader(Properties.Resources.telephone2_assets_all);

        public static T GetAsset<T>(string assetName) where T : UnityEngine.Object
        {
            //return backupLoader.GetAsset<T>(assetName);

            if (_loadedAssets.ContainsKey(assetName))
            {
                return (T)_loadedAssets[assetName];
            }



            T asset = Addressables.LoadAssetAsync<T>(assetName).WaitForCompletion();

            if (asset == null)
            {
                Debug.LogError($"{assetName} of type {typeof(T)} not found in Assetbundle.");
                return null;
            }

            _loadedAssets.Add(assetName, asset);
            return asset;
        }


        internal static void ValidateAssetIntegrity()
        {
            if (!Directory.Exists(UT2Paths.DataFolder))
                Directory.CreateDirectory(UT2Paths.DataFolder);

            if (!Directory.Exists(UT2Paths.InternalAssetsFolder))
                Directory.CreateDirectory(UT2Paths.InternalAssetsFolder);

            ValidateAssets(UT2Paths.InternalAssetsFolder);
        }

        internal static void ValidateAssets(string folder)
        {
            CheckFile(Path.Combine(folder,"catalog_wbp.hash"), Properties.Resources.catalog_wbp_hash);
            CheckFile(Path.Combine(folder,"catalog_wbp.json"), Properties.Resources.catalog_wbp_json);
            CheckFile(Path.Combine(folder,"shader_unitybuiltinshaders.bundle"), Properties.Resources.shader_unitybuiltinshaders);
            CheckFile(Path.Combine(folder,"telephone2_assets_all.bundle"), Properties.Resources.telephone2_assets_all);
            CheckFile(Path.Combine(folder,"telephone2_scenes_all.bundle"), Properties.Resources.telephone2_scenes_all);
            CheckFile(Path.Combine(folder,"ultratelephone2_monoscripts.bundle"), Properties.Resources.ultratelephone2_monoscripts);
        }

        //Check the assets existence
        internal static void CheckFile(string path, byte[] internalFile)
        {
            if (!File.Exists(path) || new FileInfo(path).Length != internalFile.Length)
            {
                File.WriteAllBytes(path, internalFile);
            }
        }
    }
}

using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CultOfJakito.UltraTelephone2.Assets
{
    public static class UT2Assets
    {
        private static Dictionary<string, UnityEngine.Object> _loadedAssets = new();

        //Uncomment in case of emergency :3c
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

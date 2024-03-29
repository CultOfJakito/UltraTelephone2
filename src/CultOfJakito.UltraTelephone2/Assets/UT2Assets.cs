using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CultOfJakito.UltraTelephone2.Assets
{
    public static class UT2Assets
    {
        private static Dictionary<string, UnityEngine.Object> _loadedAssets = new();

        public static T GetAsset<T>(string assetName) where T : UnityEngine.Object
        {
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
    }
}

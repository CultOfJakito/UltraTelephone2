using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Assets;

public class AssetLoader
{
    public AssetBundle Bundle { get; }
    private Dictionary<string, UnityEngine.Object> _loadedAssets = new();

    public AssetLoader(AssetBundle bundle) => Bundle = bundle;
    public AssetLoader(byte[] bundleBytes) => Bundle = AssetBundle.LoadFromMemory(bundleBytes);
    public AssetLoader(string filePath) => Bundle = AssetBundle.LoadFromFile(filePath);

    public T LoadAsset<T>(string assetName) where T : UnityEngine.Object
    {
        if (_loadedAssets.ContainsKey(assetName))
        {
            return (T)_loadedAssets[assetName];
        }

        T asset = Bundle.LoadAsset<T>(assetName);

        if (asset == null)
        {
            Debug.LogError($"{assetName} of type {typeof(T)} not found in Assetbundle.");
            return null;
        }

        _loadedAssets.Add(assetName, asset);
        return asset;
    }

    public T[] LoadAllAssets<T>() where T : UnityEngine.Object
    {
        T[] assets = Bundle.LoadAllAssets<T>();

        foreach (T asset in assets)
        {
            if (asset == null)
            {
                continue;
            }

            if (!_loadedAssets.ContainsKey(asset.name))
            {
                _loadedAssets.Add(asset.name, asset);
            }
        }

        return assets;
    }

    public void Unload(bool unloadAllLoadedObjects = true) => Bundle.Unload(unloadAllLoadedObjects);
}

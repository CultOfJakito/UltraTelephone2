using CultOfJakito.UltraTelephone2.Data;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Zed;

public static class ZedResources
{
    public static bool Initialized { get; private set; }
    private static AssetBundle? _zedBundle;
    private static Dictionary<string, UnityEngine.Object> _cache = new();
    public static void Init()
    {
        FileInfo bundle = new(Paths.GetBundleFilePath("zed"));
        if(bundle.Exists)
        {
            _zedBundle = AssetBundle.LoadFromFile(bundle.FullName);
            Initialized = true;
            Debug.Log("Zed bundle loaded");
        }
        else
        {
            Debug.LogError("Zed bundle not found");
        }
    }
    public static T? Load<T>(string name) where T : UnityEngine.Object
    {
        if(Initialized)
        {
            return _zedBundle?.LoadAsset<T>(name) ?? null;
        }
        else
        {
            Debug.LogError("Zed bundle not initialized");
            return null;
        }
    }
    public static T? GetCached<T>(string name) where T : UnityEngine.Object
    {
        if(_cache.ContainsKey(name))
        {
            return _cache[name] as T;
        }
        else
        {
            T? obj = Load<T>(name);
            if(obj != null)
            {
                _cache[name] = obj;
            }
            return obj;
        }
    }
}
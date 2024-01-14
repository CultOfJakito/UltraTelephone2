using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CultOfJakito.UltraTelephone2.Assets;

public static class UkPrefabs
{
    public static UkAsset<GameObject> MinosPrime { get; private set; } = new("Assets/Prefabs/Enemies/MinosPrime.prefab");
    public static UkAsset<GameObject> MindFlayer { get; private set; } = new("Assets/Prefabs/Enemies/Mindflayer.prefab");
    public static UkAsset<GameObject> MinosPrimeExplosion { get; private set; } = new("Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Minos Prime.prefab");
    public static UkAsset<GameObject> Idol { get; private set; } = new("Assets/Prefabs/Enemies/Idol.prefab");
    public static UkAsset<GameObject> ShopTerminal { get; private set; } = new("Assets/Prefabs/Levels/Shop.prefab");
}

public class UkAsset<T> where T : UnityEngine.Object
{
    private readonly string _assetAddress;
    public UkAsset(string assetAddress) => _assetAddress = assetAddress;

    public T GetObject() => Addressables.LoadAssetAsync<T>(_assetAddress).WaitForCompletion();

    public void LoadObjectAsync(Action<AsyncOperationStatus, T> onComplete) =>
        Addressables.LoadAssetAsync<T>(_assetAddress).Completed += r => onComplete.Invoke(r.Status, r.Result);
}

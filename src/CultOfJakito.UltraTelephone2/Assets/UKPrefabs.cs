using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CultOfJakito.UltraTelephone2
{
    public static class UKPrefabs
    {
        public static UKAsset<GameObject> MinosPrime { get; private set; } = new UKAsset<GameObject>("Assets/Prefabs/Enemies/MinosPrime.prefab");
        public static UKAsset<GameObject> MindFlayer { get; private set; } = new UKAsset<GameObject>("Assets/Prefabs/Enemies/Mindflayer.prefab");
        public static UKAsset<GameObject> MinosPrimeExplosion { get; private set; } = new UKAsset<GameObject>("Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Minos Prime.prefab");
        public static UKAsset<GameObject> Idol { get; private set; } = new UKAsset<GameObject>("Assets/Prefabs/Enemies/Idol.prefab");
        public static UKAsset<GameObject> ShopTerminal { get; private set; } = new UKAsset<GameObject>("Assets/Prefabs/Levels/Shop.prefab");
    }

    public class UKAsset<T> where T : UnityEngine.Object
    {
        private readonly string assetAddress;
        public UKAsset(string assetAddress)
        {
            this.assetAddress = assetAddress;
        }

        public T GetObject()
        {
            return Addressables.LoadAssetAsync<T>(assetAddress).WaitForCompletion();
        }

        public void LoadObjectAsync(Action<AsyncOperationStatus, T> onComplete)
        {
            Addressables.LoadAssetAsync<T>(assetAddress).Completed += (r) =>
            {
                onComplete.Invoke(r.Status, r.Result);
            };
        }
    }
}

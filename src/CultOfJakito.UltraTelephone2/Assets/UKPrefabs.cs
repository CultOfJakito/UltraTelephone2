using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CultOfJakito.UltraTelephone2.Assets;

public static class UkPrefabs
{
    public static UKAsset<GameObject> MinosPrime { get; private set; } = new("Assets/Prefabs/Enemies/MinosPrime.prefab");
    public static UKAsset<GameObject> MindFlayer { get; private set; } = new("Assets/Prefabs/Enemies/Mindflayer.prefab");
    public static UKAsset<GameObject> MinosPrimeExplosion { get; private set; } = new("Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Minos Prime.prefab");
    public static UKAsset<GameObject> Idol { get; private set; } = new("Assets/Prefabs/Enemies/Idol.prefab");
    public static UKAsset<GameObject> ShopTerminal { get; private set; } = new("Assets/Prefabs/Levels/Shop.prefab");
    public static UKAsset<TMP_FontAsset> VCRFont { get; private set; } = new("Assets/Fonts/VCR_OSD_MONO_1.asset");
    public static UKAsset<GameObject> BreakParticleMetal { get; private set; } = new("Assets/Particles/Breaks/BreakParticleMetal.prefab");
    public static UKAsset<Material> BubbleMaterial { get; private set; } = new("Assets/Materials/Sprites/Bubble.mat");
    public static UKAsset<AudioClip> BubbleLoopClip { get; private set; } = new("Assets/Sounds/Environment/WaterBubblesLoop.wav");
    public static UKAsset<Sprite> WhiteUI { get; private set; } = new("Assets/Textures/WhiteUI.png");
    public static AudioMixerGroup MainMixer => AudioMixerController.Instance.allSound.outputAudioMixerGroup;
}

public class UKAsset<T> where T : UnityEngine.Object
{
    private readonly string _assetAddress;
    public UKAsset(string assetAddress) => _assetAddress = assetAddress;

    public T GetObject() => Addressables.LoadAssetAsync<T>(_assetAddress).WaitForCompletion();

    public void LoadObjectAsync(Action<AsyncOperationStatus, T> onComplete) =>
        Addressables.LoadAssetAsync<T>(_assetAddress).Completed += r => onComplete.Invoke(r.Status, r.Result);
}

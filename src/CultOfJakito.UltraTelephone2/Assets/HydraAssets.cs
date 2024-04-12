using CultOfJakito.UltraTelephone2.Patches;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Assets
{
    public static class HydraAssets
    {
        public static AudioClip BeeAudioLoop => UT2Assets.GetAsset<AudioClip>("Assets/Telephone 2/Misc/Sounds/bee_loop.ogg");
        public static AudioClip BrokenShopMusic => UT2Assets.GetAsset<AudioClip>("Assets/Telephone 2/Misc/Sounds/brokenshopmusic.wav");
        public static GameObject GooglyEye => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Google Eyes/googlyeyemesh.fbx");
        public static Sprite UT2Banner => UT2Assets.GetAsset<Sprite>("Assets/Telephone 2/Misc/Textures/UltraTelephone2Header.png");
        public static Texture2D RoachCarving => UT2Assets.GetAsset<Texture2D>("Assets/Telephone 2/Misc/Textures/roachcarving.png");
        public static GameObject BuyLevelAccessBarricade => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Misc/Prefabs/BuyLevelAccess.prefab");
        public static GameObject OutOfOrderShopSign => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Misc/Prefabs/OutOfOrderShopSign.prefab");
        public static AudioClip SplatSound => UT2Assets.GetAsset<AudioClip>("Assets/Telephone 2/Misc/Sounds/CartoonSplat.ogg");
        public static GameObject Nametag => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Misc/Prefabs/Nametag.prefab");
        public static GameObject MoneyHUD => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Misc/Prefabs/MoneyHUD.prefab");
        public static AudioClip ImBlueBreak => UT2Assets.GetAsset<AudioClip>("Assets/Telephone 2/Misc/Music/ImBlueBreak.wav");
        public static AudioClip ImBlueCalm => UT2Assets.GetAsset<AudioClip>("Assets/Telephone 2/Misc/Music/ImBlueCalm.wav");
        public static AudioClip FogHorn => UT2Assets.GetAsset<AudioClip>("Assets/Telephone 2/Misc/Sounds/foghorn.ogg");
        public static GameObject Coin => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/UT1/TelephoneMod/hydrabundle/Prefab/CollectableCoin.prefab");
        public static GameObject CoinBlue => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/UT1/TelephoneMod/hydrabundle/Prefab/CollectableCoinBlue.prefab");
        public static GameObject CoinRed => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/UT1/TelephoneMod/hydrabundle/Prefab/CollectableCoinRed.prefab");
        public static GameObject CoinYellow => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/UT1/TelephoneMod/hydrabundle/Prefab/CollectableCoinYellow.prefab");
        public static GameObject CoinBlack => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/UT1/TelephoneMod/hydrabundle/Prefab/CollectableCoinBlack.prefab");
        public static GameObject CoinDiamond => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/UT1/TelephoneMod/hydrabundle/Prefab/CollectableCoinDiamond.prefab");
        public static GameObject CoinRing => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Currencies/RingCoin.prefab");
        public static GameObject CoinCollectFX => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/UT1/TelephoneMod/hydrabundle/Prefab/CollectableCoinFX.prefab");
        public static GameObject CaptchaManager => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Misc/Prefabs/CaptchaManager.prefab");

        public static GameObject HoneyBunModel => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/HoneyBun/honeybun.fbx");
        public static GameObject RocketFishModel => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/RocketFish/rocketfish.fbx");

        public static Texture2D HideousKojimaTexture => UT2Assets.GetAsset<Texture2D>("Assets/Telephone 2/Textures/HideousKojima/hideouskojima_0.png");
        public static Texture2D HideousKojimaEnragedTexture => UT2Assets.GetAsset<Texture2D>("Assets/Telephone 2/Textures/HideousKojima/hideouskojima_1.png");
        public static GameObject Glungus => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Misc/Prefabs/Glungus/Glungus.prefab");

        public static AudioClip MannequinSkitterNoise => UT2Assets.GetAsset<AudioClip>("Assets/Telephone 2/Misc/Sounds/KrabsWalk.ogg");
        public static GameObject StickyHandModel => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Misc/Prefabs/StickyHand/stickyhand.fbx");
        public static Material StickyHandMaterial => UT2Assets.GetAsset<Material>("Assets/Telephone 2/Misc/Prefabs/StickyHand/stickyHandmat.mat");
        public static GameObject StickyHandClink => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Misc/Prefabs/StickyHand/StickyHandClink.prefab");

        public static GameObject Cheb => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Misc/Prefabs/Cheb/CHEB.prefab");

        public static CursorDatabase CursorDatabase => UT2Assets.GetAsset<CursorDatabase>("Assets/Telephone 2/Misc/ScriptableObjects/CursorDatabase.asset");
    }
}

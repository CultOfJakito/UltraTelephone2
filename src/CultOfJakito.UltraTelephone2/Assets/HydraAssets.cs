using System;
using System.Collections.Generic;
using System.Text;
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
        public static GameObject Coin => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/UT1/TelephoneMod/hydrabundle/Prefab/CollectableCoin.prefab");
        public static GameObject CoinBlue => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/UT1/TelephoneMod/hydrabundle/Prefab/CollectableCoinBlue.prefab");
        public static GameObject CoinRed => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/UT1/TelephoneMod/hydrabundle/Prefab/CollectableCoinRed.prefab");
        public static GameObject CoinYellow => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/UT1/TelephoneMod/hydrabundle/Prefab/CollectableCoinYellow.prefab");
        public static GameObject CoinBlack => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/UT1/TelephoneMod/hydrabundle/Prefab/CollectableCoinBlack.prefab");
        public static GameObject CoinDiamond => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/UT1/TelephoneMod/hydrabundle/Prefab/CollectableCoinDiamond.prefab");
        public static GameObject CoinCollectFX => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/UT1/TelephoneMod/hydrabundle/Prefab/CollectableCoinFX.prefab");
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Assets
{
    public static class HydraAssets
    {
        public static AudioClip BeeAudioLoop => UT2Assets.GetAsset<AudioClip>("Assets/Telephone 2/Misc Sounds/bee_loop.ogg");
        public static GameObject GooglyEye => UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Google Eyes/googlyeyemesh.fbx");
        public static Sprite UT2Banner => UT2Assets.GetAsset<Sprite>("Assets/Telephone 2/Misc/Textures/UltraTelephone2Header.png");
    }
}

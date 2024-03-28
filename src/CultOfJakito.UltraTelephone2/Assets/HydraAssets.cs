using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Assets
{
    public static class HydraAssets
    {
        public static AudioClip BeeAudioLoop => UT2Assets.HydraBundle.LoadAsset<AudioClip>("bee_loop");
        public static GameObject GooglyEye => UT2Assets.HydraBundle.LoadAsset<GameObject>("googlyeyemesh");
        public static Sprite UT2Banner => UT2Assets.HydraBundle.LoadAsset<Sprite>("UltraTelephone2Header");
    }
}

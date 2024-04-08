using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [RegisterChaosEffect]
    [HarmonyPatch]
    public class CoinPlushy : ChaosEffect
    {
        [Configgable("Chaos/Effects", "Coin Plushies")]
        private static ConfigToggle s_enabled = new(true);

        private static bool s_effectActive;

        private static List<GameObject> _plushiePrefabs = null;
        private static UniRandom s_random;

        private static Collider playerCollider;

        public override void BeginEffect(UniRandom random)
        {
            _plushiePrefabs ??= new List<GameObject>()
            {
                UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Dev Plushies/Plushie Prefabs/ZelzmiyDevPlushie.prefab"),
                UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Dev Plushies/Plushie Prefabs/HydraDevPlushie.prefab"),
                UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Dev Plushies/Plushie Prefabs/WaffleDevPlushie.prefab"),
                UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Dev Plushies/Plushie Prefabs/ZedDevPlushie.prefab"),
                UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Dev Plushies/Plushie Prefabs/GlitchyDevPlushie.prefab"),
            };
            s_random = random;
            playerCollider = NewMovement.Instance.playerCollider;
            s_effectActive = true;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost() => 1;

        [HarmonyPatch(typeof(Coin), nameof(Coin.Start)), HarmonyPostfix]
        private static void ChangeCoinToPlushie(Coin __instance)
        {
            if (!s_enabled.Value || !s_effectActive)
                return;

            // 30% plushy chance
            if (s_random.Chance(0.7f))
               return;

            GameObject plushie = s_random.SelectRandom(_plushiePrefabs);
            GameObject plush = Instantiate(plushie, __instance.transform.position, __instance.transform.rotation);

            foreach (var col in plush.GetComponentsInChildren<Collider>())
            {
               Physics.IgnoreCollision(playerCollider, col);
            }

            plush.GetComponent<Rigidbody>().AddForce(
                CameraController.instance.transform.forward * 20 + Vector3.up * 15f + (NewMovement.Instance.ridingRocket ?
                NewMovement.Instance.ridingRocket.rb.velocity :
                NewMovement.Instance.rb.velocity) + Vector3.zero,
                ForceMode.VelocityChange);


            Destroy(__instance.gameObject);
        }

        protected override void OnDestroy() => s_effectActive = false;
    }
}

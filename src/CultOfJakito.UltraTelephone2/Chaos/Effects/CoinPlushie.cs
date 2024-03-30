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

        public override void BeginEffect(UniRandom random)
        {
            Console.WriteLine("Starting Coin Plushies");
            _plushiePrefabs ??= new List<GameObject>()
            {
                UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Dev Plushies/Plushie Prefabs/zelzmiy Niko Plush.prefab"),
                UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Dev Plushies/Plushie Prefabs/HydraDevPlushie.prefab")
            };
            Console.WriteLine("plushie count " + _plushiePrefabs.Count);
            s_random = random;
            s_effectActive = true;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost() => 1;

        [HarmonyPatch(typeof(Coin), nameof(Coin.Start)), HarmonyPostfix]
        private static void ChangeCoinToPlushie(Coin __instance)
        {
            if (!s_enabled.Value || !s_effectActive)
                return;

            // 10% plushy chance
            if (s_random.Chance(0.9f))
                return;

            GameObject plushie = s_random.SelectRandomList(_plushiePrefabs);
            GameObject plush = Instantiate(plushie, __instance.transform.position, __instance.transform.rotation);
            plush.GetComponent<Rigidbody>().AddForce(
                CameraController.instance.transform.forward * 20 + Vector3.up * 15f + (NewMovement.Instance.ridingRocket ?
                NewMovement.Instance.ridingRocket.rb.velocity :
                NewMovement.Instance.rb.velocity) + Vector3.zero,
                ForceMode.VelocityChange);

            Console.WriteLine(__instance.GetComponent<Rigidbody>().velocity);

            Destroy(__instance.gameObject);

        }
    }
}

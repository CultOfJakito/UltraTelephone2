using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    public class SlightlyShrinkTerminal : ChaosEffect
    {
        private static bool s_effectActive = false;

        [Configgable("Hydra/Chaos", "Change Terminal Size")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [Configgable("Hydra/Chaos", "Terminal Size Variance")]
        private static FloatSlider s_variance = new FloatSlider(0.3f, 0f, 0.8f);

        [HarmonyPatch(typeof(ShopZone), "Start"), HarmonyPostfix]
        public static void OnStart(ShopZone __instance)
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            //Horrible and disgusting but I have to circumvent meshcombine somehow
            GameObject cube = __instance.transform.Find("Cube").gameObject;
            cube.SetActive(false);

            GameObject terminalPrefab = UKPrefabs.ShopTerminal.GetObject();

            GameObject terminalCopy = GameObject.Instantiate(terminalPrefab);

            terminalCopy.transform.SetParent(__instance.transform.parent, true);
            terminalCopy.transform.position = __instance.transform.position;
            terminalCopy.transform.rotation = __instance.transform.rotation;
            terminalCopy.transform.localScale = __instance.transform.localScale;
            GameObject terminalMesh = terminalCopy.transform.Find("Cube").gameObject;

            terminalMesh.transform.SetParent(__instance.transform, true);
            GameObject.DestroyImmediate(terminalCopy);

            terminalMesh.layer = cube.layer;
            terminalMesh.tag = cube.tag;

            MeshRenderer sourceMeshRenderer = cube.GetComponent<MeshRenderer>();
            MeshRenderer targetMeshRenderer = terminalMesh.GetComponent<MeshRenderer>();

            //Praying this works.
            targetMeshRenderer.materials = sourceMeshRenderer.materials;

            float randomValue = (float) s_rng.NextDouble();
            randomValue = (randomValue - 0.5f) * 2f;
            randomValue *= s_variance.Value;
            randomValue = 1-randomValue;
            __instance.transform.localScale *= randomValue;
        }

        private static System.Random s_rng;

        public override void BeginEffect(System.Random random)
        {
            s_rng = random;
            s_effectActive = true;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx)
        {
            if (!s_enabled.Value)
                return false;

            return base.CanBeginEffect(ctx);
        }

        public override int GetEffectCost()
        {
            return 1;
        }

        private void OnDestroy()
        {
            s_effectActive = false;
        }
    }
}

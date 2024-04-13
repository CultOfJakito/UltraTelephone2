using Configgy;

using CultOfJakito.UltraTelephone2.DependencyInjection;

using HarmonyLib;

using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public sealed class ForcedPerspective : ChaosEffect
{
    [Configgable("Chaos/Effects", "Forced Perspective")]
    private static readonly ConfigToggle s_enabled = new(true);

    private static bool s_active = false;
    public override void BeginEffect(UniRandom random) => s_active = true;
    protected override void OnDestroy() => s_active = false;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Drone), "Awake")]
    static void ApplyForcedPerspective(EnemyIdentifier __instance)
    {
        if (!s_active || !s_enabled.value)
        {
            return;
        }

        // Lack of EnemyType check is intentional.
        // Drone is just shorthand for "a handful of flying enemies"
        __instance.gameObject.AddComponent<ForcedPerspectiveComponent>();
    }

    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

    public override int GetEffectCost() => 1;

    private sealed class ForcedPerspectiveComponent : MonoBehaviour
    {
        private Vector3 _baseScale;
        private float _baseDistance;

        public void Awake()
        {
            _baseScale = gameObject.transform.localScale;
            _baseDistance = ComputeDistance();
        }

        public void Update()
        {
            float scaleFactor = ComputeDistance() / _baseDistance;
            scaleFactor = Mathf.Clamp(scaleFactor, 0.05f, 100f);
            transform.localScale = scaleFactor * _baseScale;
        }

        private float ComputeDistance()
        {
            Vector3 camPos = Camera.main.transform.position;
            Vector3 ownPos = transform.position;
            return (ownPos - camPos).magnitude;
        }
    }
}


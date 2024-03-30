using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra;

[RegisterChaosEffect]
[HarmonyPatch]
public class DoorStuck : ChaosEffect
{
    [Configgable("Chaos/Effects", "Door Stuck")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_effectActive;
    private static UniRandom s_random;

    public override void BeginEffect(UniRandom rand)
    {
        s_random = rand;
        s_effectActive = true;
    }

    [HarmonyPatch(typeof(Door), nameof(Door.Open))]
    [HarmonyPostfix]
    public static void OnDoorOpen(Door __instance)
    {
        if (!s_effectActive || !s_enabled.Value)
        {
            return;
        }

        if (__instance.TryGetComponent(out DoorJammer jammer))
        {
            return;
        }

        jammer = __instance.gameObject.AddComponent<DoorJammer>();
        jammer.Door = __instance;
        jammer.JamOnOpen = true;
        jammer.JamOnPercent = s_random.Float() / 5f;
        jammer.UnjamAfterSeconds = s_random.Float() * 5f + 3f;
    }

    [HarmonyPatch(typeof(BigDoor), nameof(BigDoor.Open))]
    [HarmonyPostfix]
    public static void OnBigDoorOpen(BigDoor __instance)
    {
        if (!s_effectActive || !s_enabled.Value)
        {
            return;
        }

        if (__instance.TryGetComponent(out DoorJammer jammer))
        {
            return;
        }

        jammer = __instance.gameObject.AddComponent<DoorJammer>();
        jammer.BigDoor = __instance;
        jammer.JamOnOpen = true;
        jammer.JamOnPercent = (float)s_random.NextDouble() / 5f;
        jammer.UnjamAfterSeconds = (float)s_random.NextDouble() * 5f + 3f;
    }

    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 4;
    protected override void OnDestroy() => s_effectActive = false;
}

public class DoorJammer : MonoBehaviour
{
    public Door Door;
    public BigDoor BigDoor;
    public float JamOnPercent;
    public bool JamOnOpen;
    public float UnjamAfterSeconds = 5f;

    private bool _didJam;

    private void Start()
    {
        if (Door != null)
        {
            Door.onFullyOpened.AddListener(() => { _didJam = false; });
        }

        if (BigDoor != null)
        {
            Door door = BigDoor.GetComponentInParent<Door>();
            if (door != null)
            {
                door.onFullyOpened.AddListener(() => { _didJam = false; });
            }
        }
    }

    private void Update()
    {
        DoorUpdate();
        BigDoorUpdate();
    }

    private void BigDoorUpdate()
    {
        if (BigDoor == null || _didJam)
        {
            return;
        }

        if (JamOnOpen == BigDoor.open)
        {
            Vector3 rot = BigDoor.transform.localEulerAngles;
            float valueTraveled = JamOnOpen ? MathUtils.InverseLerpVector3(BigDoor.origRotation.eulerAngles, BigDoor.openRotation, rot) :
                MathUtils.InverseLerpVector3(BigDoor.openRotation, BigDoor.origRotation.eulerAngles, rot);

            if (valueTraveled > JamOnPercent)
            {
                Jam();
            }
        }
    }

    private void DoorUpdate()
    {
        if (Door == null || _didJam)
        {
            return;
        }

        if (JamOnOpen == Door.open)
        {
            Vector3 pos = Door.transform.localPosition;
            float valueTraveled = JamOnOpen ? MathUtils.InverseLerpVector3(Door.closedPos, Door.openPos, pos) :
                MathUtils.InverseLerpVector3(Door.openPos, Door.closedPos, pos);

            if (valueTraveled > JamOnPercent)
            {
                Jam();
            }
        }
    }

    private void Jam()
    {
        //Door stuck.
        _didJam = true;

        if (Door != null)
        {
            Door.enabled = false;
        }

        if (BigDoor != null)
        {
            BigDoor.enabled = false;
        }

        Invoke(nameof(ReleaseJam), UnjamAfterSeconds);
    }

    private void ReleaseJam()
    {
        if (Door != null)
        {
            Door.enabled = true;
        }

        if (BigDoor != null)
        {
            BigDoor.enabled = true;
        }
    }
}

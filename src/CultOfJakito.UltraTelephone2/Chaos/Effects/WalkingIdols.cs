using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.AI;

namespace CultOfJakito.UltraTelephone2.Hydra;

[HarmonyPatch]
[RegisterChaosEffect]
public class WalkingIdols : ChaosEffect
{
    private static bool s_effectActive;

    [Configgable("Chaos/Effects/Walking Idols", "Walking Idols")]
    private static ConfigToggle s_enabled = new(true);

    [Configgable("Chaos/Effects/Walking Idols", "Walking Idol Speed")]
    public static FloatSlider S_WalkSpeed = new(40f, 0.1f, 200f);

    [HarmonyPatch(typeof(Idol), "Start")]
    [HarmonyPostfix]
    public static void OnStart(Idol __instance)
    {
        if (!s_effectActive || !s_enabled.Value)
        {
            return;
        }

       __instance.gameObject.AddComponent<IdolWalker>();
    }

    private static UniRandom s_rng;

    public override void BeginEffect(UniRandom random)
    {
        s_rng = random;
        s_effectActive = true;
    }

    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 1;
    private void OnDestroy() => s_effectActive = false;
}


public class IdolWalker : MonoBehaviour
{
    private NavMeshAgent agent;

    private float timeUntilChangeDirection = 0f;

    private UniRandom rng;
    private float walkSpeed;
    private Vector3 targetPoint;

    private Transform model;

    //TODO add footstep sounds or skittering sounds

    private void Start()
    {
        walkSpeed = WalkingIdols.S_WalkSpeed.Value;
        rng = new UniRandom(UltraTelephoneTwo.Instance.Random.Next());
        agent = gameObject.AddComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
        agent.angularSpeed = 6000f;
        agent.acceleration = 2000f;

        model = transform.GetChild(0);
    }

    private void Update()
    {
        timeUntilChangeDirection = Mathf.Max(0f, timeUntilChangeDirection - Time.deltaTime);

        if(agent.remainingDistance < 2f || timeUntilChangeDirection <= 0f)
        {
            timeUntilChangeDirection = rng.Float() * 4f;
            targetPoint = RandomNearbyPoint();
        }

        agent.SetDestination(targetPoint);

        if (model)
        {
            Vector3 desiredDirection = agent.desiredVelocity.normalized;
            desiredDirection.y = 0f;
            if(desiredDirection != Vector3.zero)
            {
                model.forward = desiredDirection;
            }
        }
    }

    private Vector3 RandomNearbyPoint()
    {
        Vector3 randomDirection = rng.InsideUnitSphere() * 16f;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 pos = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
        {
            pos  = hit.position;
        }
        else
        {
            pos = transform.position+((transform.position - CameraController.Instance.transform.position)*-20f);
        }

        return pos;
    }
}

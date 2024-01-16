using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using ULTRAKILL;
using UnityEngine;

[RegisterChaosEffect]
public class CannonBallBounce : ChaosEffect
{
    [Configgable("ZedDev", "Enable cannonball bounce")]
    public static ConfigToggle Enabled = new ConfigToggle(true);

    public static bool CanBounce = false;
    public override void BeginEffect(System.Random random)
    {
        if(!Enabled.Value) CanBounce = false;
        else CanBounce = true;
    }
    public override bool CanBeginEffect(ChaosSessionContext ctx)
    {
        if (!Enabled.Value)
            return false;

        return base.CanBeginEffect(ctx);
    }
    public override int GetEffectCost()
    {
        return 1;
    }
}

public class BouncyCannonball : MonoBehaviour
{
    public float RemainingTime = 5f;
    public Rigidbody rb;
    SphereCollider sc;
    public void Update()
    {
        RemainingTime -= Time.deltaTime;
        if(RemainingTime <= 0) sc.material = null;
    }
    void Start()
    {
        sc = gameObject.AddComponent<SphereCollider>();
        sc.radius = 0.8f;
        sc.material = BouncyCannonballPatch.Bouncy;
    }

    private bool hurtPlayer = false;

    private void OnTriggerEnter(Collider other)
    {
        //I think this is broken rn but it would be funny.
        return;
        if (hurtPlayer || RemainingTime == 5f)
            return;

        if (other.CompareTag("Player") && RemainingTime < 4.5f)
        {
            hurtPlayer = true;
            NewMovement.Instance.GetHurt(40, true);

        }
    }
}

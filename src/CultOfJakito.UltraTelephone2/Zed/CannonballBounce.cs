using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Zed;

[RegisterChaosEffect]
public class CannonBallBounce : ChaosEffect
{
    [Configgable("ZedDev", "Enable cannonball bounce")]
    public static ConfigToggle Enabled = new(true);

    public static bool CanBounce;

    public override void BeginEffect(UniRandom random) => CanBounce = Enabled.Value;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => Enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 1;
}

public class BouncyCannonball : MonoBehaviour
{
    public float RemainingTime = 5f;
    public Rigidbody Rb;
    private SphereCollider _sc;

    public void Update()
    {
        RemainingTime -= Time.deltaTime;
        if (RemainingTime <= 0)
        {
            _sc.material = null;
        }
    }

    private void Start()
    {
        _sc = gameObject.AddComponent<SphereCollider>();
        _sc.radius = 0.8f;
        _sc.material = BouncyCannonballPatch.Bouncy;
    }

    private bool _hurtPlayer;

    private void OnTriggerEnter(Collider other)
    {
        //I think this is broken rn but it would be funny.
        return;
        if (_hurtPlayer || RemainingTime == 5f)
        {
            return;
        }

        if (other.CompareTag("Player") && RemainingTime < 4.5f)
        {
            _hurtPlayer = true;
            NewMovement.Instance.GetHurt(40, true);
        }
    }
}

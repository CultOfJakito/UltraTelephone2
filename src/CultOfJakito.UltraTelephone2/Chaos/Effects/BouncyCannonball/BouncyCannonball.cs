using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.BouncyCannonball;

public class BouncyCannonball : MonoBehaviour
{
    private static readonly PhysicMaterial s_bouncy = new("Bouncy")
    {
        bounciness = 1f,
        dynamicFriction = 0f,
        staticFriction = 0f,
        frictionCombine = PhysicMaterialCombine.Minimum,
        bounceCombine = PhysicMaterialCombine.Maximum
    };

    public float RemainingTime = 5f;
    private SphereCollider _sc;

    private void Start()
    {
        _sc = gameObject.AddComponent<SphereCollider>();
        _sc.radius = 0.8f;
        _sc.material = s_bouncy;
    }

    public void Update()
    {
        RemainingTime -= Time.deltaTime;
        if (RemainingTime <= 0)
        {
            _sc.material = null;
        }
    }
}

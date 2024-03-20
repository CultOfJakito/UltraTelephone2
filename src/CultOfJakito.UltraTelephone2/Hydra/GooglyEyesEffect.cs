using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra;

[RegisterChaosEffect]
public class GooglyEyesEffect : ChaosEffect
{
    private static bool s_effectActive;
    private static UniRandom s_random;

    public override void BeginEffect(UniRandom rand)
    {
        s_random = rand;
        s_effectActive = true;
    }

    public override int GetEffectCost() => 1;
    private void OnDestroy() => s_effectActive = false;
}

public class GooglyEye : MonoBehaviour
{
    private Vector2 _velocity;
    private Vector2 _position;
    private float _radius;
    private Vector3 _lastPosition;


    private void Awake()
    {
        _lastPosition = transform.position;
        _velocity = new Vector2(0, 0);
    }

    private void Update()
    {
    }

    private void Sol1()
    {
        Vector3 positionalDelta = transform.position - _lastPosition;
        Vector3 normal = transform.forward;

        Vector3 localizedVelocity = transform.InverseTransformDirection(positionalDelta);
        localizedVelocity.z = 0;
        _velocity += new Vector2(localizedVelocity.x, localizedVelocity.y);

        float gravity = Physics.gravity.y;

        if (_position.magnitude >= _radius)
        {
            Vector2 tangent = Vector2.Perpendicular(_position);
            Vector2 tangentVelo = tangent.normalized * gravity;
        }

        _velocity += new Vector2(0, Physics.gravity.y * Time.deltaTime);
    }

    private float _energy;
    private float _energyDecay;
    private void Sol2() => _energy -= Time.deltaTime * _energyDecay;

    //USE SINE WAVE TO MOVE THE EYE
    //SINE OVER TIME DECREASING ENERGY LOSES MOMENTUM
    private void LateUpdate() => _lastPosition = transform.position;
}

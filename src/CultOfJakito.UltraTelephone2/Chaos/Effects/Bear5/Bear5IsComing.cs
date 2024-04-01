using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Events;
using UnityEngine;
using UnityEngine.AI;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.Bear5;

[RegisterChaosEffect]
public sealed class Bear5IsComing : ChaosEffect
{
    [Configgable("Chaos/Effects", "Bear5")]
    private static ConfigToggle s_enabled = new(true);
    private static bool s_isEffectRunning = false;

    private GameObject _bear5;

    private float _startTimer;
    private bool _hasSpawnedBear5;

    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

    public override void BeginEffect(UniRandom random) {
        if(!s_enabled.Value)
        {
            return;
        }

        s_isEffectRunning = true;

        _startTimer = random.Range(50f, 120f);
        Debug.Log($"BEAR5 IS COMING IN {_startTimer}");

        GameEvents.OnPlayerRespawn += OnPlayerRespawn;
    }
    public override int GetEffectCost() => 4;
    protected override void OnDestroy() {
        GameEvents.OnPlayerRespawn -= OnPlayerRespawn;

        if(_bear5 != null)
        {
            Destroy(_bear5);
        }

        s_isEffectRunning = false;
    }

    private void Update()
    {
        if(_hasSpawnedBear5 || !s_enabled.Value || !s_isEffectRunning)
        {
            return;
        }

        _startTimer -= Time.deltaTime;
        if(_startTimer <= 0f)
        {
            SpawnBear5();
        }
    }

    private void SpawnBear5()
    {
        if(_hasSpawnedBear5 || !s_enabled.Value || !s_isEffectRunning)
        {
            return;
        }

        Debug.Log("BEAR5 IS HERE");

        try
        {
            NavMesh.SamplePosition(Vector3.zero, out NavMeshHit hit, float.PositiveInfinity, NavMesh.AllAreas);
            _bear5 = Instantiate<GameObject>(UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Bear5/BEAR5.prefab"), hit.position, Quaternion.identity);
            HudMessageReceiver.Instance.SendHudMessage("<color=#00f>BEAR5 IS COMING</color>");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to spawn Bear5: {e}");
        }
        _hasSpawnedBear5 = true;
    }

    private void OnPlayerRespawn(PlayerRespawnEvent e)
    {
        if(UnityEngine.Random.Range(0f, 1f) <= 0.1f)
        {
            //SpawnBear5();
        }
    }
}

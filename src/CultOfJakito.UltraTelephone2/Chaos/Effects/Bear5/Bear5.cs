using UnityEngine;
using UnityEngine.AI;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.Bear5;

public class Bear5 : MonoBehaviour
{
    private EnemyIdentifier _enemyIdentifier;
    private NavMeshAgent _navMeshAgent;
    private float _giveUpTimer = 3f;
    private bool _isGivingUp = false;
    private bool _isTweaking = false;

    private EnemyTarget _lastTarget;

    private Vector3 _hidingSpot;

    private void Awake()
    {
        _enemyIdentifier = GetComponent<EnemyIdentifier>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _hidingSpot = transform.position;
    }

    private void FixedUpdate()
    {
        if(UnityEngine.Random.Range(0f, 1000f) <= 0.001f)
        {
            _isTweaking = true;
        }
    }

    private void Update()
    {
        if(_enemyIdentifier == null || _enemyIdentifier.dead)
        {
            Destroy(gameObject);
            return;
        }

        if(_enemyIdentifier.target == null)
        {
            _isTweaking = false;
            _giveUpTimer = 3f;
            _lastTarget = _enemyIdentifier.target;
        }
        if(_isTweaking)
        {
            TeleportToTarget();
            return;
        }

        if(_navMeshAgent.pathStatus is NavMeshPathStatus.PathPartial or NavMeshPathStatus.PathInvalid
            || _navMeshAgent.velocity.sqrMagnitude <= 1f
            || Vector3.Distance(transform.position, _enemyIdentifier.target.position) >= 50f)
        {
            _giveUpTimer -= Time.deltaTime;
        } else
        {
            _giveUpTimer = 3f;
        }

        if(_giveUpTimer <= 0f)
        {
            if(_enemyIdentifier.target != null)
            {
                TeleportToTarget();
                return;
            }
            _isGivingUp = true;
        }

        NavMeshHit hit = default;
        bool hasTarget = _enemyIdentifier.target != null && NavMesh.SamplePosition(_enemyIdentifier.target.position, out hit, 20f, NavMesh.AllAreas);
        if(hasTarget)
        {
            _isGivingUp = false;
        }
        if (!hasTarget || _isGivingUp)
        {
            _navMeshAgent.SetDestination(_hidingSpot);
            return;
        }

        _isGivingUp = false;
        _navMeshAgent.SetDestination(hit.position);
    }

    private void TeleportToTarget()
    {
        Vector3 position = _enemyIdentifier.target.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)) * 25f;
        if(!NavMesh.SamplePosition(position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            return;
        }
        _navMeshAgent.Warp(hit.position);
        _giveUpTimer = 3f;
    }
}

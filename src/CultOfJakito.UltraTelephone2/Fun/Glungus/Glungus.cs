using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;
using UnityEngine.AI;

namespace CultOfJakito.UltraTelephone2.Fun.Glungus
{
    public class Glungus : MonoBehaviour
    {
        public Animator animator;
        public NavMeshAgent agent;

        float minDistanceToPlayer = 3f;
        float maxDistanceToPlayer = 6f;
        float hardMaxDistanceToPlayer = 40f;
        private static readonly int speedHash = Animator.StringToHash("Speed");

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>(true);
            agent = GetComponent<NavMeshAgent>();

            UniRandom rand = new UniRandom(new SeedBuilder().WithGlobalSeed().WithSceneName());

            agent.stoppingDistance = minDistanceToPlayer + rand.Range(-0.25f,0.25f);
            agent.speed = rand.Range(6f, 12f);
        }

        public void UpdateGlungus(GlungusManager boss)
        {
            Vector3 position = transform.position;
            float distance = Vector3.Distance(boss.PlayerPosition, position);

            if(distance > hardMaxDistanceToPlayer)
            {
                agent.Warp(boss.StandableSpot);
                return;
            }

            if(distance > maxDistanceToPlayer)
            {
                agent.SetDestination(boss.PlayerPosition);
            }else if(distance < minDistanceToPlayer)
            {
                agent.SetDestination(transform.position);
            }

            float speed = agent.velocity.magnitude;
            float normalized = (speed/agent.speed);
            float s = (normalized > 0.3f) ? 1f : 0f;
            animator.SetFloat(speedHash, s);
        }

        public void Kill()
        {
            GetComponent<Breakable>().Break();
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}

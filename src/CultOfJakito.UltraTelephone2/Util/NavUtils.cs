using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace CultOfJakito.UltraTelephone2.Util
{
    public static class NavUtils
    {
        public static bool TryGetRandomPointOnNavMesh(out NavMeshHit hit)
        {
            hit = new NavMeshHit();
            UniRandom rand = UniRandom.CreateFullRandom();
            int sampleAttempts = 100;

            for (int i = 0; i < sampleAttempts; i++)
            {
                Vector3 randPos = new Vector3(rand.Next(-100, 100), 0, rand.Next(-100, 100));
                if (NavMesh.SamplePosition(randPos, out hit, 50f, NavMesh.AllAreas))
                    return true;
            }

            return false;
        }
    }
}

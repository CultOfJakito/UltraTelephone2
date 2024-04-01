using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.AI;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [RegisterChaosEffect]
    public class ChebInvasion : ChaosEffect
    {
        [Configgable("Chaos/Effects", "Cheb Pylons")]
        private static ConfigToggle s_enabled = new(true);

        private static UniRandom s_rng;

        public override void BeginEffect(UniRandom random)
        {
            s_rng = random;

            this.StartCoroutine(ChebInvasionRoutine());
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx)
        {
            if (!s_enabled.Value || !base.CanBeginEffect(ctx))
                return false;

            return true;
        }
        public override int GetEffectCost() => 4;
        protected override void OnDestroy() { }

        float cellSize = 3f;
        int cellSideCount = 1000;
        int maxCellsPerTick = 10;

        private IEnumerator ChebInvasionRoutine()
        {
            Vector3 min = new Vector3(-cellSize * (float)cellSideCount / 2f, 90f, -cellSize * (float)cellSideCount / 2f);
            Vector3 max = new Vector3(cellSize * (float)cellSideCount / 2f, 90f, cellSize * (float)cellSideCount / 2f);

            Vector3 GetCellPosition(int x, int z)
            {
                return new Vector3(min.x + x * cellSize, min.y, min.z + z * cellSize);
            }

            int cells = cellSideCount * cellSideCount;
            int cellsDone = 0;

            while (cellsDone < cells)
            {
                int cellsThisTick = Math.Min(maxCellsPerTick, cells - cellsDone);

                for (int i = 0; i < cellsThisTick; i++)
                {
                    int x = cellsDone % cellSideCount;
                    int z = cellsDone / cellSideCount;

                    Vector3 pos = GetCellPosition(x, z);

                    if(NavMesh.SamplePosition(pos, out NavMeshHit hit, 50f, -1))
                    {
                        if (s_rng.Chance(0.1f))
                        {
                            GameObject.Instantiate(HydraAssets.Cheb, hit.position + Vector3.up*0.7f, Quaternion.identity);
                        }
                    }

                    cellsDone++;
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}

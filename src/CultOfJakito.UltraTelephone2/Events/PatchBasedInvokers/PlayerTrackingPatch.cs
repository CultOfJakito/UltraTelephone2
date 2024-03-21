using CultOfJakito.UltraTelephone2.Events;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ULTRASTATS.Patches
{
    [HarmonyPatch]
    public static class PlayerTrackingPatch
    {

        [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.GetHurt)), HarmonyPrefix]
        public static void PreGetHurt(NewMovement __instance, out PlayerHurtState __state)
        {
            __state = new PlayerHurtState()
            {
                Health = __instance.hp,
                Dead = __instance.dead
            };
        }

        [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.GetHurt)), HarmonyPostfix]
        public static void PostGetHurt(NewMovement __instance, PlayerHurtState __state)
        {
            int health = __instance.hp;
            int damage = __state.Health - health;
            if (damage > 0)
            {
                UKGameEventRegistry.RaiseEvent(new PlayerHurtEvent(__instance, damage));
            }

            if (!__state.Dead && __instance.dead)
            {
                UKGameEventRegistry.RaiseEvent(new PlayerDeathEvent(__instance));
            }
        }

        public class PlayerHurtState
        {
            public int Health;
            public bool Dead;
        }

        [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.GetHealth)), HarmonyPrefix]
        public static void PreGetHealth(NewMovement __instance, out int __state)
        {
            __state = __instance.hp;
        }

        [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.GetHealth)), HarmonyPostfix]
        public static void PostGetHealth(NewMovement __instance, bool silent, int __state)
        {
            //Silent usually means the player's health was reset, not healed.
            if (silent)
                return;

            int health = __instance.hp;
            int healthRegained = health - __state;

            if (healthRegained > 0)
            {
                UKGameEventRegistry.RaiseEvent(new PlayerHealEvent(__instance, healthRegained));
            }
        }

        //[HarmonyPatch(typeof(PlayerAnimations), nameof(PlayerAnimations.Footstep)), HarmonyPostfix]
        //public static void OnFootstep()
        //{

        //}

        [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.ForceAntiHP)), HarmonyPrefix]
        public static void PreForceAntiHP(NewMovement __instance, out float __state)
        {
            __state = __instance.antiHp;
        }

        [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.ForceAntiHP)), HarmonyPostfix]
        public static void PostForceAntiHP(NewMovement __instance, float __state)
        {
            float anti = __instance.antiHp;
            float antiGained = anti - __state;

            if (antiGained > 0)
            {
                UKGameEventRegistry.RaiseEvent(new PlayerAntiHealEvent(__instance, (int)antiGained));
            }
        }

        [HarmonyPatch(typeof(Punch), nameof(Punch.Parry)), HarmonyPostfix]
        public static void PostTryParryProjecilte(Punch __instance)
        {
            UKGameEventRegistry.RaiseEvent(new PlayerParryEvent(NewMovement.Instance, __instance));
        }

        private static bool s_diedToRespawn;
        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.Restart))]
        [HarmonyPostfix]
        public static void Postfix()
        {
            UKGameEventRegistry.RaiseEvent(new PlayerRespawnEvent(NewMovement.Instance, !s_diedToRespawn));
            s_diedToRespawn = false;
        }

        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.Restart))]
        [HarmonyPrefix]
        public static void Prefix() => s_diedToRespawn = NewMovement.Instance.hp <= 0;
    }
}

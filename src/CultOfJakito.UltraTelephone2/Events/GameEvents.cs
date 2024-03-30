using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.Events
{
    public static class GameEvents
    {
        /// <summary>
        /// Invoked when the player dies.
        /// </summary>
        public static Action OnPlayerDeath;

        /// <summary>
        /// Invoked when the player respawns.
        /// </summary>
        public static Action<PlayerRespawnEvent> OnPlayerRespawn;

        /// <summary>
        /// Invoked when the player heals.
        /// </summary>
        public static Action<PlayerHealEvent> OnPlayerHeal;

        /// <summary>
        /// Invoked when the player takes damage.
        /// </summary>
        public static Action<PlayerHurtEvent> OnPlayerHurt;

        /// <summary>
        /// Invoked when anti-hp is given
        /// </summary>
        public static Action<PlayerAntiHealEvent> OnPlayerAntiHeal;

        /// <summary>
        /// Invoked after the player parries something.
        /// </summary>
        public static Action OnParry;


        /// <summary>
        /// Invoked when the level state changes. This includes the level starting, and ending.
        /// </summary>
        public static Action<LevelStateChangeEvent> OnLevelStateChange;

        /// <summary>
        /// Invoked when an enemy dies... obviously.
        /// </summary>
        public static Action<EnemyDeathEvent> OnEnemyDeath;


    }
}

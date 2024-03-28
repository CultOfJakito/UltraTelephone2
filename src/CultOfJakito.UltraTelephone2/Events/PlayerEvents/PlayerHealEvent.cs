using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.Events
{
    public class PlayerHealEvent : PlayerEvent
    {
        public int HealthGained { get; }
        public PlayerHealEvent(NewMovement player, int healthGained) : base(player)
        {
            HealthGained = healthGained;
        }
    }
}

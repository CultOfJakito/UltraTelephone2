using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.Events
{
    public class PlayerAntiHealEvent : PlayerEvent
    {
        public int AntiHealthGained { get; }
        public PlayerAntiHealEvent(NewMovement player, int antiGained) : base(player)
        {
            AntiHealthGained = antiGained;
        }
    }
}

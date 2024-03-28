using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.Events
{
    public class PlayerHurtEvent : PlayerEvent
    {
        public int Damage { get; }

        public PlayerHurtEvent(NewMovement player, int damage) : base(player)
        {
            this.Damage = damage;
        }
    }
}

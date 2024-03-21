using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.Events
{
    public class PlayerRespawnEvent : PlayerEvent
    {
        public bool IsManualRespawn { get; }
        public PlayerRespawnEvent(NewMovement player, bool manualRespawn) : base(player)
        {
            IsManualRespawn = manualRespawn;
        }
    }
}

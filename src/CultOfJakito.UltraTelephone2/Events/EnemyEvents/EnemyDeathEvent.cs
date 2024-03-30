using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.Events
{
    public class EnemyDeathEvent : EnemyEvent
    {
        public string Hitter { get; }

        public EnemyDeathEvent(EnemyIdentifier eid, string hitter) : base(eid)
        {
            Hitter = hitter;
        }
    }
}

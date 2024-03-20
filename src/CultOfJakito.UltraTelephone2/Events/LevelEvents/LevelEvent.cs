using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.Events
{
    public class LevelEvent : UKGameEvent
    {
        public string LevelName { get; }

        public LevelEvent(string levelName)
        {
            LevelName = levelName;
        }
    }
}

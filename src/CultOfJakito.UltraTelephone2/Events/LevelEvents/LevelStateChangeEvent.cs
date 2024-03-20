using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.Events
{
    public class LevelStateChangeEvent : LevelEvent
    {
        public bool IsPlaying { get; }
        public LevelStateChangeEvent(bool state, string levelName) : base(levelName)
        {
            IsPlaying = state;
        }
    }
}

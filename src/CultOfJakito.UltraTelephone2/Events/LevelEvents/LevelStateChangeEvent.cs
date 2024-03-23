using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.Events
{
    public class LevelStateChangeEvent : LevelEvent
    {
        /// <summary>
        /// If the stats manager timer is running.
        /// </summary>
        public bool IsPlaying { get; }
        public LevelStateChangeEvent(bool state, string levelName) : base(levelName)
        {
            IsPlaying = state;
        }
    }
}

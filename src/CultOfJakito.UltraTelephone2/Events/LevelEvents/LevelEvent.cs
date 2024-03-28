using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.Events
{
    public class LevelEvent : UKGameEvent
    {

        /// <summary>
        /// The name of the level... duh.
        /// </summary>
        public string LevelName { get; }

        public LevelEvent(string levelName)
        {
            LevelName = levelName;
        }
    }
}

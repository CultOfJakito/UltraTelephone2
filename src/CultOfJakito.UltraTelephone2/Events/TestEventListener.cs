using System;
using System.Collections.Generic;
using System.Text;
using CultOfJakito.UltraTelephone2.Events;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.LevelSpecific
{
    public class TestEventListener : IEventListener
    {
        [EventListener]
        public void OnLevelStart(LevelStateChangeEvent e)
        {
            Debug.Log($"Level {(e.IsPlaying ? "Started" : "Ended")}: " + e.LevelName);
        }
    }
}

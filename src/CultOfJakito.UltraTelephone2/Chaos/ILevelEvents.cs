using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2
{
    public interface ILevelEvents
    {
        public void OnLevelStarted(string levelName);
        public void OnLevelComplete(string levelName);
    }
}

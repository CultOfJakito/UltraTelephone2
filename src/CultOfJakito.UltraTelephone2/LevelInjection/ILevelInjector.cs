using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.LevelInjection
{
    public interface ILevelInjector
    {
        public void OnLevelLoaded(string sceneName);
    }
}

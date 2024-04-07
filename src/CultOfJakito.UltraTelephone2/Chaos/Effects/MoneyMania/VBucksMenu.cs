using System;
using System.Collections.Generic;
using System.Text;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.LevelInjection;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.MoneyMania
{
    [RegisterLevelInjector]
    class VBucksMenu : ILevelInjector
    {

        private GameObject _vboinksScreen = UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Currencies/Vboinks/Vbucks Buy Screen.prefab");

        public void OnLevelLoaded(string sceneName)
        {
            if (sceneName.Equals("Main Menu"))
            {
                GameObject.Instantiate(_vboinksScreen, CanvasController.instance.transform);
            }
        }
    }
}

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

        private GameObject _vboinksScreen;

        public void OnLevelLoaded(string sceneName)
        {
            if (sceneName.Equals("Main Menu"))
            {
                _vboinksScreen = UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Currencies/Vboinks/VbucksBuyScreen.prefab");

                if((bool)_vboinksScreen)
                    GameObject.Instantiate(_vboinksScreen, CanvasController.instance.transform);
            }
        }
    }
}

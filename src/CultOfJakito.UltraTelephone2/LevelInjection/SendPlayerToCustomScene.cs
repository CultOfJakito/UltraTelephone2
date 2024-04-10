using System;
using System.Collections.Generic;
using System.Text;
using CultOfJakito.UltraTelephone2.Assets;
using GameConsole.Commands;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.LevelInjection
{
    public class SendPlayerToCustomScene : MonoBehaviour
    {
        [SerializeField] private string _path;

        public void SendToScene()
        {
            AddressableManager.LoadSceneUnsanitzed(_path);
        }

    }
}

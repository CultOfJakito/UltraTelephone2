using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.LevelInjection.Level_5_1
{
    [RegisterLevelInjector]
    public class ImBlueDaBaDee : MonoBehaviour, ILevelInjector
    {
        public void OnLevelLoaded(string sceneName)
        {
            if (sceneName != "Level 5-1" || !GeneralSettings.EnableCopyrightedMusic.Value)
            {
                UnityEngine.Object.Destroy(this);
                return;
            }
        }

        private void Start()
        {
            MusicManager.Instance.cleanTheme.clip = HydraAssets.ImBlueCalm;
            MusicManager.Instance.battleTheme.clip = HydraAssets.ImBlueBreak;
            MusicManager.Instance.bossTheme.clip = HydraAssets.ImBlueBreak;

            foreach (MusicChanger changer in Locator.LocateComponentsOfType<MusicChanger>())
            {
                changer.battle = HydraAssets.ImBlueBreak;
                changer.clean = HydraAssets.ImBlueCalm;
                changer.boss = HydraAssets.ImBlueBreak;
            }
        }
    }
}

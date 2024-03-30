using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.LevelInjection.Level_5_1
{
    [RegisterLevelInjector]
    public class ImBlueDaBaDee : MonoBehaviour, ILevelInjector
    {
        [Configgable("Patches/Level", "Im Blue Da Ba Dee")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        //So like... the actual music used here I "remixed" a bit and aren't technically the songs.
        //However, the original songs are copyrighted as far as I know and I don't know if the mixes are transformative enough
        //to be considered fair use. So I'm just going to be safe and include into the copyrighted toggle. womp womp :/

        public void OnLevelLoaded(string sceneName)
        {
            if (sceneName != "Level 5-1" || !GeneralSettings.EnableCopyrightedMusic.Value || !s_enabled.Value)
            {
                UnityEngine.Object.Destroy(this);
                return;
            }
        }

        private Stack<Action> undoStack;

        private void Start()
        {
            undoStack = new Stack<Action>();

            AudioClip cleanDefault = MusicManager.Instance.cleanTheme.clip;
            AudioClip battleDefault = MusicManager.Instance.battleTheme.clip;
            AudioClip bossDefault = MusicManager.Instance.bossTheme.clip;

            undoStack.Push(() =>
            {
                MusicManager.Instance.cleanTheme.clip = cleanDefault;
                MusicManager.Instance.battleTheme.clip = battleDefault;
                MusicManager.Instance.bossTheme.clip = bossDefault;
            });


            MusicManager.Instance.cleanTheme.clip = HydraAssets.ImBlueCalm;
            MusicManager.Instance.battleTheme.clip = HydraAssets.ImBlueBreak;
            MusicManager.Instance.bossTheme.clip = HydraAssets.ImBlueBreak;

            foreach (MusicChanger changer in Locator.LocateComponentsOfType<MusicChanger>())
            {
                AudioClip cClean = changer.clean;
                AudioClip cBattle = changer.battle;
                AudioClip cBoss = changer.boss;

                undoStack.Push(() =>
                {
                    if(changer != null)
                    {
                        changer.clean = cClean;
                        changer.battle = cBattle;
                        changer.boss = cBoss;
                    }
                });

                changer.battle = HydraAssets.ImBlueBreak;
                changer.clean = HydraAssets.ImBlueCalm;
                changer.boss = HydraAssets.ImBlueBreak;
            }

            GeneralSettings.EnableCopyrightedMusic.OnValueChanged += SetEnabled;
            s_enabled.OnValueChanged += SetEnabled;
        }


        private void SetEnabled(bool enabled)
        {
            enabled = GeneralSettings.EnableCopyrightedMusic.Value && s_enabled.Value;

            if (enabled)
                return;

            while (undoStack.Count > 0)
                undoStack.Pop()?.Invoke();

            Destroy(this);
        }

        private void OnDestroy()
        {
            s_enabled.OnValueChanged -= SetEnabled;
            GeneralSettings.EnableCopyrightedMusic.OnValueChanged -= SetEnabled;
        }
    }
}

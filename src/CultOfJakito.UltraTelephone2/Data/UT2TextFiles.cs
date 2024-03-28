using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Data
{
    public static class UT2TextFiles
    {
        [Configgable("General", "Reload Text Files")]
        private static ConfigButton s_reloadFiles = new ConfigButton(ReloadFiles, "Reload Text Files");

        public static TextListFile S_BossBarPrefixesFile { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "bossBar.prefixes.txt"), Properties.Resources.bossBar_prefixes);
        public static TextListFile S_BossBarSuffixesFile { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "bossBar.suffixes.txt"), Properties.Resources.bossBar_suffixes);
        public static TextListFile S_SplashTextsFile { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "splashes.txt"), Properties.Resources.splashes);
        public static TextListFile S_CantReadWordsFile { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "cantReadWords.txt"), Properties.Resources.cantReadWords);

        public static void ReloadFiles()
        {
            S_BossBarPrefixesFile.Load();
            S_BossBarSuffixesFile.Load();
            S_SplashTextsFile.Load();
            S_CantReadWordsFile.Load();
        }
    }
}

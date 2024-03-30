using Configgy;
using CultOfJakito.UltraTelephone2.Util;

namespace CultOfJakito.UltraTelephone2.Data
{
    public static class UT2TextFiles
    {
        [Configgable("Extras/Advanced", "Reload Text Files")]
        private static ConfigButton s_reloadFiles = new ConfigButton(ReloadFiles, "Reload Text Files");

        public static TextListFile S_BossBarPrefixesFile { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "bossBar.prefixes.txt"), Properties.Resources.bossBar_prefixes);
        public static TextListFile S_BossBarSuffixesFile { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "bossBar.suffixes.txt"), Properties.Resources.bossBar_suffixes);
        public static TextListFile S_SplashTextsFile { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "splashes.txt"), Properties.Resources.splashes);
        public static TextListFile S_CantReadWordsFile { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "cantReadWords.txt"), Properties.Resources.cantReadWords);
        public static TextListFile S_PonderPrompts { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "ponder.txt"), Properties.Resources.ponder);
        public static TextListFile S_ShiteTipsFile { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "shiteTips.txt"), Properties.Resources.shiteTips);
        public static TextListFile S_EnemyNames { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "enemyNames.txt"), Properties.Resources.enemyNames);
        public static TextListFile S_WordList10k { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "wordlist.10000.txt"), Properties.Resources.wordlist_10000);

        public static void ReloadFiles()
        {
            S_BossBarPrefixesFile.Load();
            S_BossBarSuffixesFile.Load();
            S_SplashTextsFile.Load();
            S_CantReadWordsFile.Load();
            S_PonderPrompts.Load();
            S_ShiteTipsFile.Load();
            S_EnemyNames.Load();
            S_WordList10k.Load();
        }
    }
}

using Configgy;
using CultOfJakito.UltraTelephone2.Util;

namespace CultOfJakito.UltraTelephone2.Data
{
    public static class UT2TextFiles
    {
        [Configgable("Extras/Advanced", "Reload Text Files")]
        private static ConfigButton s_reloadFiles = new ConfigButton(ReloadFiles, "Reload Text Files");

        public static TextListFile BossBarPrefixesFile { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "bossBar.prefixes.txt"), Properties.Resources.bossBar_prefixes);
        public static TextListFile BossBarSuffixesFile { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "bossBar.suffixes.txt"), Properties.Resources.bossBar_suffixes);
        public static TextListFile SplashTextsFile { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "splashes.txt"), Properties.Resources.splashes);
        public static TextListFile CantReadWordsFile { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "cantReadWords.txt"), Properties.Resources.cantReadWords);
        public static TextListFile PonderPromptsFile { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "ponder.txt"), Properties.Resources.ponder);
        public static TextListFile ShiteTipsFile { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "shiteTips.txt"), Properties.Resources.shiteTips);
        public static TextListFile EnemyNamesFile { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "enemyNames.txt"), Properties.Resources.enemyNames);
        public static TextListFile WordList10kFile { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "wordlist.10000.txt"), Properties.Resources.wordlist_10000);
        public static TextListFile WindowTitlesFile { get; private set; } = new TextListFile(Path.Combine(UT2Paths.TextFolder, "windowTitles.txt"), Properties.Resources.windowTitles);

        public static void ReloadFiles()
        {
            BossBarPrefixesFile.Load();
            BossBarSuffixesFile.Load();
            SplashTextsFile.Load();
            CantReadWordsFile.Load();
            PonderPromptsFile.Load();
            ShiteTipsFile.Load();
            EnemyNamesFile.Load();
            WordList10kFile.Load();
            WindowTitlesFile.Load();
        }
    }
}

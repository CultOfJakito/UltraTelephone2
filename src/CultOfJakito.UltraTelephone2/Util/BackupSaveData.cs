namespace CultOfJakito.UltraTelephone2.Util
{
    public static class BackupSaveData
    {
        private static string saveGameFolder => GameProgressSaver.BaseSavePath;
        private static string backupFolder => Path.Combine(UT2Paths.ModFolder, "SAVES_BACKUP");

        public static void EnsureBackupExists()
        {
            if (Directory.Exists(backupFolder))
                return;

            UnityEngine.Debug.Log("UT2: No backup data found... backing up player data.");

            Directory.CreateDirectory(backupFolder);

            foreach (string filePath in Directory.GetFiles(saveGameFolder,"*", SearchOption.AllDirectories))
            {
                //get the relative path
                string relativePath = filePath.Substring(saveGameFolder.Length+1, filePath.Length-(saveGameFolder.Length+1));
                string backupPath = Path.Combine(backupFolder, relativePath);

                if(!Directory.Exists(Path.GetDirectoryName(backupPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(backupPath));

                File.Copy(filePath, backupPath);
            }

            UnityEngine.Debug.Log("UT2: Backup save data created.");
        }
    }
}

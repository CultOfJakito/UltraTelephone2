using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2
{
    public static class UT2Data
    {
        private static UT2SaveData saveData;

        public static UT2SaveData SaveData
        {
            get
            {
                if(saveData == null)
                {
                    Load();
                }
                return saveData;
            }
        }

        private static void Load()
        {
            if(!Directory.Exists(Path.GetDirectoryName(saveDataPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(saveDataPath));

            if(!File.Exists(saveDataPath))
            {
                saveData = new UT2SaveData();
                return;
            }

            string json = File.ReadAllText(saveDataPath);
            try
            {
                saveData = JsonConvert.DeserializeObject<UT2SaveData>(json);
                Debug.Log("UT2: Loaded saved data.");

            }catch(Exception e)
            {
                Debug.LogError("UT2: Failed to load saved data.");
                Debug.LogException(e);
                saveData = new UT2SaveData();
            }
        }

        private static string saveDataPath => Paths.DataFilePath;

        public static void Save()
        {
            if (saveData == null)
                return;

            if (!Directory.Exists(Path.GetDirectoryName(saveDataPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(saveDataPath));

            string json = JsonConvert.SerializeObject(saveData);
            File.WriteAllText(saveDataPath, json);
            Debug.Log("Saved UT2 data");
        }
    }

    [Serializable]
    public class UT2SaveData
    {
        public bool initializedPAmount;
        public long fakePAmount;
        public int lastRealPAmount;
    }
}

﻿using Newtonsoft.Json;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Data;

public static class Ut2Data
{
    private static Ut2SaveData s_saveData;

    public static Ut2SaveData SaveData
    {
        get
        {
            if (s_saveData == null)
            {
                Load();
            }

            return s_saveData;
        }
    }

    private static void Load()
    {
        if (!Directory.Exists(Path.GetDirectoryName(SaveDataPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SaveDataPath));
        }

        if (!File.Exists(SaveDataPath))
        {
            s_saveData = new Ut2SaveData();
            return;
        }

        string json = File.ReadAllText(SaveDataPath);
        try
        {
            s_saveData = JsonConvert.DeserializeObject<Ut2SaveData>(json);
            Debug.Log("UT2: Loaded saved data.");
        }
        catch (Exception e)
        {
            Debug.LogError("UT2: Failed to load saved data.");
            Debug.LogException(e);
            s_saveData = new Ut2SaveData();
        }
    }

    private static string SaveDataPath => Paths.DataFilePath;

    public static void Save()
    {
        if (s_saveData == null)
        {
            return;
        }

        if (!Directory.Exists(Path.GetDirectoryName(SaveDataPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SaveDataPath));
        }

        string json = JsonConvert.SerializeObject(s_saveData);
        File.WriteAllText(SaveDataPath, json);
        Debug.Log("Saved UT2 data");
    }
}

[Serializable]
public class Ut2SaveData
{
    public bool InitializedPAmount;
    public long FakePAmount;
    public int LastRealPAmount;
}

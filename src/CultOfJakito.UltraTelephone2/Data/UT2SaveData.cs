using CultOfJakito.UltraTelephone2.Fun.EA;
using CultOfJakito.UltraTelephone2.Util;
using Newtonsoft.Json;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Data;

public static class UT2SaveData
{
    private static Ut2SaveData s_saveData;

    private static string SaveDataPath => Path.Combine(UT2Paths.DataFolder, "saveData.json");

    public static bool IsDirty { get; private set; }
    public static void MarkDirty()
    {
        IsDirty = true;
    }

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

    public static void Load()
    {
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


    public static void Save()
    {
        if (s_saveData == null)
            return;

        if (!Directory.Exists(Path.GetDirectoryName(SaveDataPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SaveDataPath));
        }

        string json = JsonConvert.SerializeObject(s_saveData, Formatting.Indented);
        File.WriteAllText(SaveDataPath, json);
        IsDirty = false;
        Debug.Log("Saved UT2 data");
    }
}

[Serializable]
public class Ut2SaveData
{
    public bool InitializedPAmount;
    public long FakePAmount;
    public int LastRealPAmount;

    public bool BeenToCasino;

    public List<BuyableReceipt> purchases;

    //CurrencyChaos
    public int Rings; 
    public int Vbucks; // TODO; add to main menu buying them
    public int Blood; 
    public int MetalScraps; 
    public int Trophies; 
    public int Gunpowder; 
    public int Fish; 
    public int Plushies; 
    public int MarketCoins;
}

using System.Reflection;
using HarmonyLib;

public class PatchThis : Attribute
{
    public string ID;
    public PatchThis(string identifier)
    {
        ID = identifier;
    }
}
public static class Patches
{
    public static Dictionary<string, Harmony> AllPatches = new();
    public static void PatchAll()
    {
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.GetCustomAttribute<PatchThis>() is PatchThis patch)
            {
                if (!AllPatches.ContainsKey(patch.ID))
                {
                    AllPatches[patch.ID] = new Harmony(patch.ID);
                }
                AllPatches[patch.ID].PatchAll(type);
            }
        }
    }
}
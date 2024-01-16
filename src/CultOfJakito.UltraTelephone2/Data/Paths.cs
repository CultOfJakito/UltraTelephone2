using System.Reflection;

namespace CultOfJakito.UltraTelephone2.Data;

public static class Paths
{
    private const string BundleFolderName = "Bundles";
    private const string DataFileName = "saveData.json";
    private const string DataFolderName = "Data";

    public static string BundleFolder => Path.Combine(ModFolder, BundleFolderName);
    public static string ModFolder => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    public static string DataFilePath => Path.Combine(DataFolder, DataFileName);
    public static string DataFolder => Path.Combine(ModFolder, DataFolderName);

    public static string GetBundleFilePath(string bundleFileName)
    {
        if (!Directory.Exists(BundleFolder))
        {
            Directory.CreateDirectory(BundleFolder);
        }

        return Path.Combine(BundleFolder, bundleFileName);
    }

    public static void ValidateFolders()
    {
        if (!Directory.Exists(BundleFolder))
        {
            Directory.CreateDirectory(BundleFolder);
        }

        if (!Directory.Exists(DataFolder))
        {
            Directory.CreateDirectory(DataFolder);
        }
    }
}

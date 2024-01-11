using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CultOfJakito.UltraTelephone2
{
    public static class Paths
    {

        const string BUNDLE_FOLDER_NAME = "Bundles";
        const string DATA_FILE_NAME = "saveData.json";

        public static string BundleFolder => Path.Combine(ModFolder, BUNDLE_FOLDER_NAME);
        public static string ModFolder => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string DataFilePath => Path.Combine(DataFolder, DATA_FILE_NAME);
        public static string DataFolder => Path.Combine(ModFolder, "Data");
        public static string GetBundleFilePath(string bundleFileName)
        {
            if(!Directory.Exists(BundleFolder))
                Directory.CreateDirectory(BundleFolder);

            return Path.Combine(BundleFolder, bundleFileName);
        }
    }
}

using System.IO.Compression;
using AngryLevelLoader.Containers;
using AngryLevelLoader.DataTypes;
using AngryLevelLoader.Managers;
using Newtonsoft.Json;
using RudeLevelScript;

namespace CultOfJakito.UltraTelephone2.Hydra
{
    public static class AngryInjector
    {
        private static Dictionary<string, AngryBundleContainer> loadedBundles = new Dictionary<string, AngryBundleContainer>();

        //This doesnt work atm. Do not use.
        public static void LoadUTLevel(string name)
        {
            AngryBundleContainer bundle = null;
            LevelContainer levelContainer = null;
            RudeLevelData levelData = null;
            string levelPath = null;

            foreach (var b in loadedBundles.Values)
            {
                if (b.levels.TryGetValue(name, out levelContainer))
                {
                    bundle = b;
                    levelData = b.GetAllLevelData().Where(x=>x.levelName == name).FirstOrDefault();
                    break;
                }
            }

            if (levelContainer == null)
                throw new Exception("Level container could not be found.");

            if (bundle == null)
                throw new Exception($"No bundle containing {name} could be found.");
            
            if (levelData == null)
                throw new Exception("Level data name does not match level name, or level data is missing.");

            AngrySceneManager.LoadLevel(bundle, levelContainer, levelData, levelPath);
        }   

        public static void PreloadBundle(byte[] bytes)
        {
            AngryBundleData data = FileToBundleData(bytes);
            AngryBundleContainer bundleContainer = data.ToAngryBundle();
            loadedBundles.Add(bundleContainer.bundleData.bundleName, bundleContainer);
        }

        private static AngryBundleContainer ToAngryBundle(this AngryBundleData data)
        {
            AngryBundleContainer bundle = new AngryBundleContainer(data.bundleDataPath, data);
            return bundle;
        }

        private static AngryBundleData FileToBundleData(byte[] angryFile)
        {
            //Angry doesnt have a way to load from memory, so most of this is copied from AngryLevelLoader
            //EternalUnion wrote the code this is based on.
            using (MemoryStream ms = new MemoryStream(angryFile))
            {
                using (ZipArchive zip = new ZipArchive(ms))
                {
                    var entry = zip.GetEntry("data.json");
                    if (entry == null)
                        throw new Exception("No data.json found in injected angry bundle");

                    using (StreamReader dataReader = new StreamReader(entry.Open()))
                        return JsonConvert.DeserializeObject<AngryBundleData>(dataReader.ReadToEnd());
                }
            }
        }
    }
}

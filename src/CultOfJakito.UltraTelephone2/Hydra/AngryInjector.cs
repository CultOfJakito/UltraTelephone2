using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using AngryLevelLoader;
using AngryLevelLoader.Containers;
using AngryLevelLoader.DataTypes;
using AngryLevelLoader.Managers;
using Newtonsoft.Json;

namespace CultOfJakito.UltraTelephone2.Hydra
{
    public static class AngryInjector
    {
        public static void LoadUTLevel(string levelName)
        {
            //AngrySceneManager.LoadLevel(,);
        }

        private static Dictionary<string, AngryBundleContainer> loadedBundles = new Dictionary<string, AngryBundleContainer>();

        public static void LoadUTLevel(string name, byte[] angryFile)
        {
            
        }   

        public static void PreloadBundle(byte[] bytes)
        {
            AngryBundleData data = FileToBundleData(bytes);
            AngryBundleContainer bundleContainer = data.ToAngryBundle();
            loadedBundles.Add(bundleContainer.bundleData.bundleName, bundleContainer);
            //AngrySceneManager.LoadLevel(bundleContainer, levelContainer, levelData, levelName);
        }

        private static void PreloadLevel()
        {

        }


        private static AngryBundleContainer ToAngryBundle(this AngryBundleData data)
        {
            AngryBundleContainer bundle = new AngryBundleContainer(data.bundleDataPath, data);
            //AngryLevelLoader.Plugin.angryBundles[data.bundleGuid] = bundle;
            bundle.UpdateOrder();
            bundle.UpdateScenes(false, true);

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

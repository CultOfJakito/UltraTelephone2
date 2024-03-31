using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Util
{
    public static class TextureHelper
    {
        private static Texture2D[] cachedTextures = new Texture2D[0];
        private static Dictionary<string, Texture2D> cachedTextureMap = new Dictionary<string, Texture2D>();

        private static bool initialized = false;

        public static void LoadTextures(string folderPath)
        {
            cachedTextures = FindTextures(folderPath);
        }

        public static bool TryLoadTexture(string path, out Texture2D tex)
        {
            tex = null;

            if (cachedTextureMap.ContainsKey(path))
            {
                tex = cachedTextureMap[path];
                return true;
            }

            if (!File.Exists(path))
            {
                Debug.LogError("UT2: Invalid texture path: " + path);
                return false;
            }

            byte[] byteArray = null;
            try
            {
                byteArray = File.ReadAllBytes(path);
            }
            catch (System.Exception e)
            {
                Debug.LogError("UT2: Error reading texture file: " + e.Message);
                Debug.LogException(e);
                return false;
            }

            tex = new Texture2D(16, 16);
            if (!tex.LoadImage(byteArray))
            {
                Debug.LogError("UT2: Error loading texture image: " + path);
                return false;
            }

            cachedTextureMap[path] = tex;
            return true;
        }

        public static Texture2D RandomTextureFromCache(UniRandom random)
        {
            if (cachedTextures.Length > 0)
                return random.SelectRandomFromSet(cachedTextures);

            return null;
        }

        private static void ClearTextureCache()
        {
            if (cachedTextures == null)
                return;

            int len = cachedTextures.Length;
            for (int i = 0; i < len; i++)
            {
                if (cachedTextures[i] == null)
                    continue;

                UnityEngine.Object.Destroy(cachedTextures[i]);
            }

            cachedTextures = null;
        }

        public static Texture2D[] FindTextures(string folderPath)
        {
            if(!Directory.Exists(folderPath))
            {
                Debug.LogError("UT2: Invalid folder path: " + folderPath);
                return Array.Empty<Texture2D>();
            }

            List<Texture2D> newTextures = new List<Texture2D>();

            string[] pngs = System.IO.Directory.GetFiles(folderPath, "*.png", SearchOption.AllDirectories);
            string[] jpgs = System.IO.Directory.GetFiles(folderPath, "*.jpg", SearchOption.AllDirectories);

            for (int i = 0; i < pngs.Length; i++)
            {
                if (TryLoadTexture(pngs[i], out Texture2D newTex))
                {
                    newTextures.Add(newTex);
                }
            }

            for (int i = 0; i < jpgs.Length; i++)
            {
                if (TryLoadTexture(jpgs[i], out Texture2D newTex))
                {
                    newTextures.Add(newTex);
                }
            }

            return newTextures.ToArray();
        }
    }
}

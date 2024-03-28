using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CultOfJakito.UltraTelephone2.Util
{
    public static class UT2Paths
    {
        /// <summary>
        /// Where the dll is located.
        /// </summary>
        public static string ModFolder => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// Where data is located
        /// </summary>
        public static string DataFolder => Path.Combine(ModFolder, $"{UltraTelephoneTwo.MOD_NAME}_Data");

        /// <summary>
        /// Textures here!
        /// </summary>
        public static string TextureFolder => Path.Combine(DataFolder, "textures");

        /// <summary>
        /// Audio here!
        /// </summary>
        public static string AudioFolder => Path.Combine(DataFolder, "audio");

        /// <summary>
        /// Text here
        /// </summary>
        public static string TextFolder => Path.Combine(DataFolder, "text");

        public static void EnsureFolders()
        {
            if (!Directory.Exists(DataFolder))
                Directory.CreateDirectory(DataFolder);

            if (!Directory.Exists(TextureFolder))
                Directory.CreateDirectory(TextureFolder);

            if (!Directory.Exists(AudioFolder))
                Directory.CreateDirectory(AudioFolder);

            if (!Directory.Exists(TextFolder))
                Directory.CreateDirectory(TextFolder);
        }
    }
}

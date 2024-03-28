using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.Assets
{
    public static class UT2Assets
    {
        private static AssetLoader zedBundle;
        public static AssetLoader ZedBundle
        {
            get
            {
                zedBundle ??= new AssetLoader(Properties.Resources.zed);
                return zedBundle;
            }
        }

        private static AssetLoader zelzmiyBundle;
        public static AssetLoader ZelzmiyBundle
        {
            get
            {
                zelzmiyBundle ??= new AssetLoader(Properties.Resources.zelzmiy);
                return zelzmiyBundle;
            }
        }


        private static AssetLoader hydraBundle;
        public static AssetLoader HydraBundle
        {
            get
            {
                hydraBundle ??= new AssetLoader(Properties.Resources.hydra);
                return hydraBundle;
            }
        }

        private static AssetLoader ultraTelephoneLegacyBundle;
        public static AssetLoader UltraTelephoneLegacyBundle
        {
            get
            {
                ultraTelephoneLegacyBundle ??= new AssetLoader(Properties.Resources.TelephoneMod);
                return ultraTelephoneLegacyBundle;
            }
        }

        public static void ForceLoad()
        {
            zelzmiyBundle ??= new AssetLoader(Properties.Resources.zelzmiy);
            zedBundle ??= new AssetLoader(Properties.Resources.zed);
            hydraBundle ??= new AssetLoader(Properties.Resources.hydra);
            ultraTelephoneLegacyBundle ??= new AssetLoader(Properties.Resources.TelephoneMod);
        }
    }
}

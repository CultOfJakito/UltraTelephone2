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
                if (zedBundle == null)
                {
                    zedBundle = new AssetLoader(Properties.Resources.zelzmiy);
                }
                return zedBundle;
            }
        }

        private static AssetLoader zelzmiyBundle;
        public static AssetLoader ZelzmiyBundle
        {
            get
            {
                if(zelzmiyBundle == null)
                {
                    zelzmiyBundle = new AssetLoader(Properties.Resources.zelzmiy);
                }
                return zelzmiyBundle;
            }
        }

        public static void ForceLoad()
        {
            if(zelzmiyBundle != null)
                zelzmiyBundle = new AssetLoader(Properties.Resources.zelzmiy);

            if(zedBundle != null)
                zedBundle = new AssetLoader(Properties.Resources.zed);
        }
    }
}

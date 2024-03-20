using System;
using System.Collections.Generic;
using System.Text;
using Configgy;

namespace CultOfJakito.UltraTelephone2
{
    public static class GeneralSettings
    {
        [Configgable("General", description:"If copyrighted music should be enabled or disabled.")]
        public static ConfigToggle EnableCopyrightedMusic = new ConfigToggle(true);
    }
}

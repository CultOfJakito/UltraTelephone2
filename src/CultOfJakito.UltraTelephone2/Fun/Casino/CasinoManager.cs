using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.Fun.Casino
{
    [ConfigureSingleton(SingletonFlags.NoAutoInstance)]
    public class CasinoManager : MonoSingleton<CasinoManager>
    {
        public long Winnings;

    }
}

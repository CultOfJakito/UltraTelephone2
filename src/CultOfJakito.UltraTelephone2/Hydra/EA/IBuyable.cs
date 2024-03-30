using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.Hydra.EA
{
    public interface IBuyable
    {
        public long GetCost();
        public string GetBuyableID();
        public void Buy();
    }
}

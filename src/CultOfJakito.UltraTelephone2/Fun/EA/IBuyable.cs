namespace CultOfJakito.UltraTelephone2.Fun.EA
{
    public interface IBuyable
    {
        public long GetCost();
        public string GetBuyableID();
        public void Buy();
    }
}

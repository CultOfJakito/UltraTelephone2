namespace CultOfJakito.UltraTelephone2.Placeholders.PlaceholderTypes
{
    public class TelephoneDeveloperName : IStringPlaceholder
    {
        public string PlaceholderID => "UT2_DEVELOPER_NAME";
        private UniRandom random;

        public string GetPlaceholderValue()
        {
            return random.SelectRandom(developerNames);
        }

        private string[] developerNames;

        public TelephoneDeveloperName()
        {
            random = UniRandom.CreateFullRandom();
            developerNames = new string[]
            {
                "Hydraxous",
                "zelzmiy",
                "Protract__",
                "Wafflethings",
                "ZedDev",
                "Teamdoodz"
            };
        }
    }
}

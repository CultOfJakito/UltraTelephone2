using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.Placeholders.PlaceholderTypes
{
    public class UltrakillDeveloperName : IStringPlaceholder
    {
        public string PlaceholderID => "UK_DEVELOPER_NAME";

        public string GetPlaceholderValue()
        {
            return random.SelectRandom(developerNames);
        }

        private UniRandom random;
        private string[] developerNames;

        public UltrakillDeveloperName()
        {
            random = UniRandom.CreateFullRandom();
            developerNames = new string[]
            {
                "Hakita",
                "PITR",
                "Heckteck",
                "ActionDawg",
                "Jericho",
                "Francis"
            };

        }
    }
}

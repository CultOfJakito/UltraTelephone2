using System;
using System.Collections.Generic;
using System.Text;
using Configgy;

namespace CultOfJakito.UltraTelephone2.Placeholders.PlaceholderTypes
{
    public class PlayerHardwardName : IStringPlaceholder
    {
        [Configgable("Text", "Hardware Username Usage")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        public string PlaceholderID => "PLAYER_HARDWARE_NAME";

        public string GetPlaceholderValue()
        {
            if (!s_enabled.Value)
                return "Person";

            return Environment.UserName;
        }

    }
}

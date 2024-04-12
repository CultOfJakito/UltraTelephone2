using Configgy;
using Steamworks;

namespace CultOfJakito.UltraTelephone2.Placeholders.PlaceholderTypes
{
    public class PlayerSteamName : IStringPlaceholder
    {
        [Configgable("Text", "Steam Username Usage")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        public string PlaceholderID => "PLAYER_STEAM_NAME";

        public string GetPlaceholderValue()
        {
            if(SteamClient.IsValid && SteamClient.IsLoggedOn && s_enabled.Value)
            {
                return SteamClient.Name;
            }

            return "Person";
        }

    }
}

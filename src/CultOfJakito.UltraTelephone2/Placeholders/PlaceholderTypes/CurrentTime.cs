namespace CultOfJakito.UltraTelephone2.Placeholders.PlaceholderTypes
{
    public class CurrentTime : IStringPlaceholder
    {
        public string PlaceholderID => "CURRENT_TIME";

        public string GetPlaceholderValue()
        {
            return DateTime.Now.ToString("hh:mm t");
        }
    }
}

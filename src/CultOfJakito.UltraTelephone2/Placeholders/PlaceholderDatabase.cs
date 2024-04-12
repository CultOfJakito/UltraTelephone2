namespace CultOfJakito.UltraTelephone2.Placeholders
{
    public class PlaceholderDatabase
    {
        private Dictionary<string, IStringPlaceholder> placeholders = new Dictionary<string, IStringPlaceholder>();

        public void AddPlaceholder(IStringPlaceholder placeholder)
        {
            placeholders.Add(placeholder.PlaceholderID, placeholder);
        }

        public void RemovePlaceholder(string placeholderID)
        {
            placeholders.Remove(placeholderID);
        }

        public string GetPlaceholderValue(string placeholderID)
        {
            if (placeholders.ContainsKey(placeholderID))
                return placeholders[placeholderID].GetPlaceholderValue();
            else
                return string.Empty;
        }

        public bool ContainsPlaceholderID(string placeholderID)
        {
            return placeholders.ContainsKey(placeholderID);
        }

        public bool StringContainsPlaceholder(string value)
        {
            return value.Contains(PlaceholderHelper.PlaceholderStart) && value.Contains(PlaceholderHelper.PlaceholderEnd);
        }

        const string PlaceholderStart = "<ph>";
        const string PlaceholderEnd = "</ph>";

        public string ReplacePlaceholders(string value)
        {
            string copy = value;

            while (StringContainsPlaceholder(copy))
            {
                int start = copy.IndexOf(PlaceholderStart);
                int end = copy.IndexOf(PlaceholderEnd);
                string placeholderID = copy.Substring(start + PlaceholderStart.Length, (end - start) - (PlaceholderEnd.Length-1));
                copy = copy.Replace(PlaceholderStart + placeholderID + PlaceholderEnd, GetPlaceholderValue(placeholderID));
            }

            return copy;
        }

    }
}

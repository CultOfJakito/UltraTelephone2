namespace CultOfJakito.UltraTelephone2.Placeholders.PlaceholderTypes
{
    public class UltraTelephone2DownloadCount : IStringPlaceholder
    {

        static string downloadCount = "???";
        public string PlaceholderID => "UT2_DOWNLOAD_COUNT";

        const string url = "https://thunderstore.io/c/ultrakill/p/CultOfJakito/UltraTelephone2/";

        public UltraTelephone2DownloadCount()
        {
            UpdateDownloadCount();
        }

        public string GetPlaceholderValue()
        {
            UpdateDownloadCount();
            return downloadCount;
        }

        private void UpdateDownloadCount()
        {
            Task.Run(async () =>
            {
                downloadCount = await GetDownloadCount();
            });
        }

        private async Task<string> GetDownloadCount()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string html = await response.Content.ReadAsStringAsync();

                    //Disgusting HTML parsing/scraping tehee
                    const string marker = "<td>Total downloads</td>";
                    int index = html.IndexOf(marker);
                    if(index == -1)
                        return "???";

                    string sub = html.Substring(index + marker.Length, 100);

                    int start = sub.IndexOf("<td>");
                    int end = sub.IndexOf("</td>");

                    if (start != -1 && end != -1)
                        return sub.Substring(start + 4, end - start - 4);
                    else
                        return "???";
                }
                else
                {
                    return "???";
                }
            }
        }

        
    }
}

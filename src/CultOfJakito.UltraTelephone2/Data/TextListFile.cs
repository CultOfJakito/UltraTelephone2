using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Data
{
    public class TextListFile
    {
        public string FilePath { get; }
        public string DefaultText { get; }

        public List<string> TextList { get; private set; }

        public TextListFile(string path, string defaultText)
        {
            FilePath = path;
            DefaultText = defaultText;
        }

        public void Load()
        {
            if (!File.Exists(FilePath))
            {
                File.WriteAllText(FilePath, DefaultText);
                TextList = LinesToList(DefaultText);
                return;
            }

            try
            {
                TextList = ReadFromFile(FilePath);
                if (TextList == null)
                {
                    TextList = LinesToList(DefaultText);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading text file at path {FilePath}");
                Debug.LogException(ex);
                TextList = LinesToList(DefaultText);
                return;
            }
        }

        private List<string> LinesToList(string text)
        {
            return new List<string>(text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.TrimEnd('\n', '\r')));
        }


        private List<string> ReadFromFile(string path)
        {
            return LinesToList(File.ReadAllText(path));
        }

    }
}

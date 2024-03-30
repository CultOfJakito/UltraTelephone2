using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Data
{
    public class TextListFile
    {
        public string Path { get; }
        public string DefaultText { get; }

        public List<string> TextList { get; private set; }

        public TextListFile(string path, string defaultText)
        {
            Path = path;
            DefaultText = defaultText;
        }

        public void Load()
        {
            if (!File.Exists(Path))
            {
                File.WriteAllText(Path, DefaultText);
                TextList = LinesToList(DefaultText);
                return;
            }

            try
            {
                TextList = ReadFromFile(Path);
                if(TextList == null)
                    TextList = LinesToList(DefaultText);
            }
            catch (Exception ex)
            {
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

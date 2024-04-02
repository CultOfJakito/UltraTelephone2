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
            Debug.Log("loading tlf " + Path);
            if (!File.Exists(Path))
            {
                Debug.Log("doesnt exist, setting to dt");
                File.WriteAllText(Path, DefaultText);
                TextList = LinesToList(DefaultText);
                Debug.Log("worked and ret");
                return;
            }

            try
            {
                Debug.Log("reading from file");
                TextList = ReadFromFile(Path);
                if (TextList == null)
                {
                    TextList = LinesToList(DefaultText);
                    Debug.Log("null read, dt");
                }
            }
            catch (Exception ex)
            {
                Debug.Log("exception!");
                Debug.LogException(ex);
                TextList = LinesToList(DefaultText);
                Debug.Log("set to dt, reting");
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

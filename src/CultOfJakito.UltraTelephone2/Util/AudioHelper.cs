using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CultOfJakito.UltraTelephone2.Zed;
using JetBrains.Annotations;
using UltraTelephone;
using UnityEngine;
using UnityEngine.Networking;

namespace CultOfJakito.UltraTelephone2.Util
{
    public static class AudioHelper
    {

        private static Dictionary<string, AudioClip> loadedClips = new Dictionary<string, AudioClip>();
        private static Dictionary<string, AudioClip> loadedClipsByPath = new Dictionary<string, AudioClip>();

        private static CoroutineRunner coroutineRunner;

        private static CoroutineRunner GetCoroutineRunner()
        {
            if (coroutineRunner == null)
            {
                coroutineRunner = new GameObject("AudioHelper").AddComponent<CoroutineRunner>();
                GameObject.DontDestroyOnLoad(coroutineRunner.gameObject);
            }

            return coroutineRunner;
        }

        public static void LoadClips(string folderPath)
        {
            DirectoryInfo info = new DirectoryInfo(folderPath);

            foreach (FileInfo file in info.GetFiles("*.wav", SearchOption.TopDirectoryOnly))
                GetCoroutineRunner().StartCoroutine(LoadClipAsync(file.FullName, AudioType.WAV));
            foreach (FileInfo file in info.GetFiles("*.mp3", SearchOption.TopDirectoryOnly))
                GetCoroutineRunner().StartCoroutine(LoadClipAsync(file.FullName, AudioType.MPEG));
            foreach (FileInfo file in info.GetFiles("*.ogg", SearchOption.TopDirectoryOnly))
                GetCoroutineRunner().StartCoroutine(LoadClipAsync(file.FullName, AudioType.OGGVORBIS));
        }

        public static void LoadClipAtPath(string filePath, Action<bool, AudioClip> onComplete)
        {
            if(loadedClipsByPath.ContainsKey(filePath))
            {
                onComplete?.Invoke(true, loadedClipsByPath[filePath]);
                return;
            }

            GetCoroutineRunner().StartCoroutine(LoadClipAtPathAsync(filePath, onComplete));
        }

        private static IEnumerator LoadClipAtPathAsync(string filePath, Action<bool, AudioClip> onComplete)
        {
            AudioType audioType = AudioType.UNKNOWN;
            string extension = Path.GetExtension(filePath);
            switch (extension)
            {
                case ".wav":
                    audioType = AudioType.WAV;
                    break;
                case ".mp3":
                    audioType = AudioType.MPEG;
                    break;
                case ".ogg":
                    audioType = AudioType.OGGVORBIS;
                    break;
                default:
                    Debug.LogError($"Unsupported audio file type: {extension}");
                    onComplete?.Invoke(false, null);
                    yield break;
            }

            yield return LoadClipAsync(filePath, audioType);

            if (loadedClipsByPath.ContainsKey(filePath))
                onComplete?.Invoke(true, loadedClipsByPath[filePath]);
            else
                onComplete?.Invoke(false, null);
        }

        public static void ClearClipCache()
        {
            foreach (AudioClip clip in loadedClips.Values)
            {
                UnityEngine.Object.Destroy(clip);
            }

            loadedClips.Clear();
        }

        private static IEnumerator LoadClipAsync(string filePath, AudioType audioType)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, audioType))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError)
                {
                    Debug.Log($"Failed to load {audioType} file at path {filePath}");
                    Debug.Log(www.error);
                }
                else
                {
                    AudioClip newAudioClip = DownloadHandlerAudioClip.GetContent(www);
                    if (newAudioClip != null)
                    {
                        if(!loadedClips.ContainsKey(newAudioClip.name))
                            loadedClips.Add(newAudioClip.name, newAudioClip);

                        loadedClipsByPath[filePath] = newAudioClip;
                    }
                }
            }
        }

        public static IEnumerable<AudioClip> GetLoadedClips()
        {
            return loadedClipsByPath.Values;
        }

        public static bool TryGetAudioClipByName(string name, out AudioClip clip)
        {
            clip = null;
            if (!loadedClips.ContainsKey(name))
                return false;

            clip = loadedClips[name];
            return true;
        }
    }
}

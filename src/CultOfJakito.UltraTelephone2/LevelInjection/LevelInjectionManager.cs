using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CultOfJakito.UltraTelephone2.LevelInjection
{
    public class LevelInjectionManager : MonoBehaviour
    {
        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            //Onl run on the active scene
            if (scene != SceneManager.GetActiveScene())
                return;

            GameObject levelInjectionObject = new GameObject("UT2 LevelInjections");

            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (Type type in asm.GetTypes())
            {
                if (type.IsAbstract)
                    continue;

                if (type.GetCustomAttribute<RegisterLevelInjectorAttribute>() == null)
                    continue;

                if (!typeof(ILevelInjector).IsAssignableFrom(type))
                    continue;


                ILevelInjector levelInjectionListener = null;

                if (typeof(MonoBehaviour).IsAssignableFrom(type))
                {
                    levelInjectionListener = levelInjectionObject.AddComponent(type) as ILevelInjector;
                }
                else
                {
                    levelInjectionListener = (ILevelInjector)Activator.CreateInstance(type);
                }

                levelInjectionListener?.OnLevelLoaded(SceneHelper.CurrentScene);
            }
        }

    }
}

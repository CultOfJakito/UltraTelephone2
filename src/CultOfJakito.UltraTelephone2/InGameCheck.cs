using Configgy;
using UnityEngine.SceneManagement;

namespace CultOfJakito.UltraTelephone2;

public static class InGameCheck
{
    private static bool s_initialized;

    public static void Init()
    {
        if (!s_initialized)
        {
            s_initialized = true;
            SceneManager.sceneLoaded += OnSceneLoad;
        }
    }

    /// <summary>
    /// Enumerated version of the Ultrakill scene types
    /// </summary>
    public enum UkLevelType
    {
        Tutorial,
        Intro,
        MainMenu,
        Level,
        Endless,
        Sandbox,
        Credits,
        Custom,
        Intermission,
        Secret,
        PrimeSanctum,
        Unknown
    }

    /// <summary>
    /// Returns the current level type
    /// </summary>
    public static UkLevelType CurrentLevelType = UkLevelType.Intro;

    /// <summary>
    /// Returns the currently active ultrakill scene name.
    /// </summary>
    public static string CurrentSceneName = "";

    /// <summary>
    /// Invoked whenever the current level type is changed.
    /// </summary>
    public static event Action<UkLevelType> OnLevelTypeChanged;

    /// <summary>
    /// Invoked whenever the scene is changed.
    /// </summary>
    public static event Action<string> OnLevelChanged;

    //Perhaps there is a better way to do this.
    private static void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene != SceneManager.GetActiveScene())
        {
            return;
        }

        string sceneName = SceneHelper.CurrentScene;
        UkLevelType newScene = GetUkLevelType(sceneName);

        if (newScene != CurrentLevelType)
        {
            CurrentLevelType = newScene;
            CurrentSceneName = sceneName; //grr
            OnLevelTypeChanged?.Invoke(newScene);
        }

        OnLevelChanged?.Invoke(sceneName);
    }

    //Perhaps there is a better way to do this. Also this will most definitely cause problems in the future if PITR or Hakita rename any scenes.

    /// <summary>
    /// Gets enumerated level type from the name of a scene.
    /// </summary>
    /// <param name="sceneName">Name of the scene</param>
    /// <returns></returns>
    public static UkLevelType GetUkLevelType(string sceneName)
    {
        sceneName = sceneName.Contains("P-") ? "Sanctum" : sceneName;
        sceneName = sceneName.Contains("-S") ? "Secret" : sceneName;
        sceneName = sceneName.Contains("Level") ? "Level" : sceneName;
        sceneName = sceneName.Contains("Intermission") ? "Intermission" : sceneName;

        switch (sceneName)
        {
            case "Main Menu":
                return UkLevelType.MainMenu;
            case "Custom Content":
                return UkLevelType.Custom;
            case "Intro":
                return UkLevelType.Intro;
            case "Endless":
                return UkLevelType.Endless;
            case "uk_construct":
                return UkLevelType.Sandbox;
            case "Intermission":
                return UkLevelType.Intermission;
            case "Level":
                return UkLevelType.Level;
            case "Secret":
                return UkLevelType.Secret;
            case "Sanctum":
                return UkLevelType.PrimeSanctum;
            case "CreditsMuseum2":
                return UkLevelType.Credits;
            case "Tutorial":
                return UkLevelType.Tutorial;
            default:
                return UkLevelType.Unknown;
        }
    }

    [Configgable("Extras/Advanced", "Force InGame Check True")]
    private static ConfigToggle s_cfgForceInLevelCheckTrue = new(false);

    /// <summary>
    /// Returns true if the current scene is playable.
    /// This will return false for all secret levels.
    /// </summary>
    /// <returns></returns>
    public static bool InLevel()
    {
        if (s_cfgForceInLevelCheckTrue.Value)
        {
            return true;
        }

        switch (CurrentLevelType)
        {
            case UkLevelType.MainMenu:
            case UkLevelType.Intro:
            case UkLevelType.Intermission:
            case UkLevelType.Secret:
            case UkLevelType.Tutorial:
                return false;
            default:
                return true;
        }
    }

    public static bool PlayingLevel()
    {
        if (!InLevel())
        {
            return false;
        }

        if (StatsManager.Instance == null)
        {
            return false;
        }

        //Finished level.
        if (StatsManager.Instance.infoSent)
        {
            return false;
        }

        //Level timer has started, so level has started.
        return StatsManager.Instance.seconds > 0f;
    }
}

using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Util;

public static class ResolutionFuckeryUtils
{
    public static void ResetToDefault()
    {
        Screen.fullScreen = PrefsManager.Instance.GetBoolLocal("fullscreen", false);
        Screen.SetResolution(PrefsManager.Instance.GetIntLocal("resolutionWidth", Screen.width), PrefsManager.Instance.GetIntLocal("resolutionHeight", Screen.height), Screen.fullScreen);
    }

    public static Vector2Int StandardResolution => new(PrefsManager.Instance.GetIntLocal("resolutionWidth", Screen.width), PrefsManager.Instance.GetIntLocal("resolutionHeight", Screen.height));

    public static FullScreenMode StandardFullScreenMode => PrefsManager.Instance.GetBoolLocal("fullscreen", false) ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
}

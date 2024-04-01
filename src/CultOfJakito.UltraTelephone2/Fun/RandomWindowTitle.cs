using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using CultOfJakito.UltraTelephone2.Data;
using Configgy;

namespace CultOfJakito.UltraTelephone2.Fun;

public static class RandomWindowTitle
{
    [DllImport("user32.dll", EntryPoint = "SetWindowText", CharSet = CharSet.Unicode)]
    private static extern bool SetWindowText(System.IntPtr hwnd, System.String lpString);
    [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Unicode)]
    private static extern System.IntPtr FindWindow(System.String className, System.String windowName);

    [Configgable("Fun", "Random Window Title")]
    private static ConfigToggle s_enabled = new ConfigToggle(true);

    private static nint _windowHandle;

    public static void Reroll()
    {
        if(_windowHandle == IntPtr.Zero)
        {
            _windowHandle = FindWindow(null, "ULTRAKILL");
        }

        if(!s_enabled.Value)
        {
            SetWindowText(_windowHandle, "ULTRAKILL");
            return;
        }

        string text = UT2TextFiles.WindowTitlesFile.TextList[UnityEngine.Random.Range(0, UT2TextFiles.WindowTitlesFile.TextList.Count)];

        SetWindowText(_windowHandle, text);
    }
}

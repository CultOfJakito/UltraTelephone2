using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using CultOfJakito.UltraTelephone2.Data;
using Configgy;
using CultOfJakito.UltraTelephone2.Util;
using CultOfJakito.UltraTelephone2.Placeholders;

namespace CultOfJakito.UltraTelephone2.Fun;

public static class RandomWindowTitle
{
    [DllImport("user32.dll", EntryPoint = "SetWindowText", CharSet = CharSet.Unicode)]
    private static extern bool SetWindowText(System.IntPtr hwnd, System.String lpString);
    [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Unicode)]
    private static extern System.IntPtr FindWindow(System.String className, System.String windowName);

    [Configgable("Fun", "Random Window Title")]
    private static ConfigToggle s_enabled = new ConfigToggle(true);

    public static nint WindowHandle { get; private set; }

    public static void Reroll()
    {
        if(WindowHandle == IntPtr.Zero)
        {
            WindowHandle = FindWindow(null, "ULTRAKILL");
        }

        if(!s_enabled.Value)
        {
            SetWindowText(WindowHandle, "ULTRAKILL");
            return;
        }

        UniRandom random = new UniRandom(new SeedBuilder()
            .WithGlobalSeed()
            .WithSceneName()
            .WithSeed(nameof(RandomWindowTitle)));

        string text = random.SelectRandom(UT2TextFiles.WindowTitlesFile.TextList);
        text = PlaceholderHelper.ReplacePlaceholders(text);
        SetWindowText(WindowHandle, text);
    }
}

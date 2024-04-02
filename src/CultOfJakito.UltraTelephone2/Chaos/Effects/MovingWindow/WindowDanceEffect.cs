using System.Collections;
using System.Runtime.InteropServices;
using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.MovingWindow;

[RegisterChaosEffect]
public class WindowDanceEffect : ChaosEffect
{
    [Configgable("Chaos/Effects", "Window Dancing")]
    private static ConfigToggle s_enabled = new(true);

    private static Type[] s_effectTypes =
    {
        typeof(DvdScreensaver),
        typeof(Circles)
    };

    private Coroutine _movementRoutine;
    private IntPtr? _currentWindowHandle;
    private Rect _startRect;
    private Vector2Int _resolution;

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")] private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
    [DllImport("user32.dll", EntryPoint = "FindWindow")] private static extern IntPtr FindWindow(string className, string windowName);
    [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

    private struct Rect //sucks, but needed for getwindowrect
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
    }

    public override void BeginEffect(UniRandom random)
    {
        _movementRoutine = StartCoroutine(WindowMovement());
    }

    public override void Dispose()
    {
        StopCoroutine(_movementRoutine);
    }

    protected override void OnDestroy()
    {
        ResolutionFuckeryUtils.ResetToDefault();
    }

    public override int GetEffectCost() => 10;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && Application.platform == RuntimePlatform.WindowsPlayer && base.CanBeginEffect(ctx) && !ctx.ContainsEffect<ResolutionSwitcher>();

    private void StartMoving()
    {
        _currentWindowHandle ??= FindWindow(null, Application.productName);
        GetWindowRect(_currentWindowHandle.Value, ref _startRect);
        Vector2 resDecimal = (Vector2)ResolutionFuckeryUtils.StandardResolution / 2.5f;
        _resolution = new Vector2Int((int)resDecimal.x, (int)resDecimal.y);
        Screen.SetResolution(_resolution.x, _resolution.y, false);
    }

    private void StopMoving()
    {
        SetWindowPos(_currentWindowHandle.Value, 0, _startRect.Left, _startRect.Top, _startRect.Right - _startRect.Left, _startRect.Bottom - _startRect.Top, 5);
        ResolutionFuckeryUtils.ResetToDefault();
    }

    private IEnumerator WindowMovement()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(5, 20));
            StartMoving();

            float timer = 0;
            float moveLength = UnityEngine.Random.Range(10, 30);
            Vector2 cumulativeMovement = Vector3.zero;

            MovementMode mode = Activator.CreateInstance(s_effectTypes[UnityEngine.Random.Range(0, s_effectTypes.Length)]) as MovementMode; //i cant think of a better way, reflection sucks :(
            Debug.Log($"Window dance starting of type {mode.GetType().Name}");

            while (timer < moveLength)
            {
                Rect rect = new();
                GetWindowRect(_currentWindowHandle.Value, ref rect);

                Vector2Int currentWindowPoint = new(rect.Left, rect.Top);
                cumulativeMovement += mode.Move(_resolution, currentWindowPoint) * Time.unscaledDeltaTime;

                Vector2Int toMove = Vector2Int.FloorToInt(cumulativeMovement);
                cumulativeMovement -= toMove;

                Vector2Int targetPos = currentWindowPoint + toMove;
                targetPos.x = Mathf.Clamp(targetPos.x, 0, Screen.currentResolution.width - _resolution.x);
                targetPos.y = Mathf.Clamp(targetPos.y, 0, Screen.currentResolution.height - _resolution.y);
                SetWindowPos(_currentWindowHandle.Value, 0, targetPos.x, targetPos.y, _resolution.x, _resolution.y, 5);

                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            StopMoving();
            yield return null;
        }
    }
}

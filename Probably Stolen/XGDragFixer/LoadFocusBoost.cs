using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace XGDragFixer;

/// <summary>
/// Background-load frame boost.
///
/// The game caps itself to 10 FPS while its window does not have focus (the
/// "fpsCapWhileMinimized" option). Loading a save in that state can leave the load
/// time initialisation behind its visual objects: once the window regains focus the
/// game runs again but the broken initialisation stays broken, which showed up as a
/// container that cannot be opened until the game is restarted.
///
/// While an app-start or scene-load window is active this keeps the full frame rate
/// even when the window is in the background, then hands the frame rate back to the
/// vanilla option.
/// </summary>
internal static class LoadFocusBoost
{
    private const float StartupWindowSeconds = 60f;
    private const float LoadWindowSeconds = 30f;
    private const int FullFrameRate = 60;

    private static MethodInfo? _applyFpsCap;
    private static float _until;
    private static bool _boosted;

    internal static bool Active => Core.LoadFocusBoost?.Value == true;

    internal static void Init()
    {
        _until = Mathf.Max(_until, Time.realtimeSinceStartup + StartupWindowSeconds);
    }

    internal static void NotifySceneLoaded()
    {
        _until = Mathf.Max(_until, Time.realtimeSinceStartup + LoadWindowSeconds);
    }

    internal static void Tick()
    {
        if (!Active)
        {
            if (_boosted)
            {
                Restore();
            }

            return;
        }

        if (!Application.isFocused && Time.realtimeSinceStartup < _until)
        {
            if (!_boosted)
            {
                _boosted = true;
                QualitySettings.vSyncCount = 0;
                Core.Log?.Log("[稳定性] 检测到后台读档，临时解除后台限帧");
            }

            if (Application.targetFrameRate < FullFrameRate)
            {
                Application.targetFrameRate = FullFrameRate;
            }

            return;
        }

        if (_boosted)
        {
            Restore();
        }
    }

    private static void Restore()
    {
        _boosted = false;
        try
        {
            _applyFpsCap ??= AccessTools.Method(typeof(EmporiumOption), "ApplyFpsCap");
            _applyFpsCap?.Invoke(EmporiumOption.Instance, null);
        }
        catch
        {
            // Giving the frame rate back is best effort only.
        }
    }

    internal static void Reset() => _boosted = false;
}

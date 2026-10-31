using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuickSell;

internal static class ShortcutKeys
{
    internal static bool TryParse(string text, out KeyCode[] keys)
    {
        var result = new List<KeyCode>();
        foreach (string part in text.Split('+'))
        {
            if (part.Length == 0) continue;
            if (!Enum.TryParse(part, true, out KeyCode key) || !Enum.IsDefined(typeof(KeyCode), key)
                || key == KeyCode.None || key == KeyCode.Mouse0 || key == KeyCode.Mouse1)
            { keys = Array.Empty<KeyCode>(); return false; }
            result.Add(key);
        }
        keys = result.ToArray();
        return true;
    }

    private static bool Read(KeyCode key, Func<KeyCode, bool> input)
    {
        if (key == KeyCode.LeftControl || key == KeyCode.RightControl) return input(KeyCode.LeftControl) || input(KeyCode.RightControl);
        if (key == KeyCode.LeftAlt || key == KeyCode.RightAlt) return input(KeyCode.LeftAlt) || input(KeyCode.RightAlt);
        if (key == KeyCode.LeftShift || key == KeyCode.RightShift) return input(KeyCode.LeftShift) || input(KeyCode.RightShift);
        return input(key);
    }

    internal static bool Held(KeyCode[] keys, Func<KeyCode, bool> input)
    {
        if (keys.Length == 0) return false;
        foreach (var key in keys) if (!Read(key, input)) return false;
        return true;
    }

    internal static bool Pressed(KeyCode[] keys, Func<KeyCode, bool> held, Func<KeyCode, bool> down)
    {
        if (!Held(keys, held)) return false;
        foreach (var key in keys) if (Read(key, down)) return true;
        return false;
    }
}

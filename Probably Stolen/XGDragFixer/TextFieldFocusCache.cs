using UnityEngine;

namespace XGDragFixer;

/// <summary>
/// StoreUIManager.IsTextFieldFocused is called once per input handler every frame
/// (plus by game master checks). Each call walks EventSystem.state, fetches the
/// selected object and does a GetComponent lookup. The answer only changes on
/// selection changes, so cache it once per frame.
/// </summary>
internal static class TextFieldFocusCache
{
    private static int _frame = -1;
    private static bool _value;

    internal static void Reset()
    {
        _frame = -1;
    }

    internal static bool Prefix(ref bool __result)
    {
        if (Core.TextFieldFocusCache?.Value != true || _frame != Time.frameCount)
        {
            return true;
        }

        __result = _value;
        return false;
    }

    internal static void Postfix(bool __result)
    {
        if (Core.TextFieldFocusCache?.Value != true)
        {
            return;
        }

        _frame = Time.frameCount;
        _value = __result;
    }
}

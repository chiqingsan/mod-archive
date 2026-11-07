using ClickToAdvanceFinal;

static void Assert(bool ok, string label)
{
    if (!ok) throw new Exception(label);
    Console.WriteLine("PASS " + label);
}

void ResetInput() => UnityEngine.Input.Keys.Clear();

void Hold(params UnityEngine.KeyCode[] keys)
{
    foreach (var key in keys) UnityEngine.Input.Keys.Add(key);
}

// 1.0.0 原有行为
ClickToAdvanceRunner.Advance(null);
var noDialog = new DialogUIManager();
ClickToAdvanceRunner.Advance(noDialog);
Assert(noDialog.Calls == 0, "no active dialog");

var choice = new DialogUIManager { currentDialog = new object(), Choice = true };
ClickToAdvanceRunner.Advance(choice);
Assert(choice.Calls == 0, "choices are not advanced");

var active = new DialogUIManager { currentDialog = new object() };
ClickToAdvanceRunner.Advance(active);
Assert(active.Calls == 1, "active dialog calls native advance once");

// 默认状态：开关关闭、屏蔽键 LeftAlt
Assert(!ClickToAdvanceMod.ShouldSuppressAdvance(), "bypass is off by default");
Assert(ClickToAdvanceMod.BypassKey == UnityEngine.KeyCode.LeftAlt, "default bypass key is LeftAlt");
Assert(ClickToAdvanceMod.IsValidBypassKey(UnityEngine.KeyCode.None), "None is a valid bypass key (disables suppression)");
Assert(!ClickToAdvanceMod.IsValidBypassKey(UnityEngine.KeyCode.Mouse0), "mouse buttons are rejected");
Assert(!ClickToAdvanceMod.IsValidBypassKey(UnityEngine.KeyCode.Escape), "Escape is rejected");

// 开启后按住 Alt（左右都算）才屏蔽
ClickToAdvanceMod.SetBypassEnabled(true);
Assert(!ClickToAdvanceMod.ShouldSuppressAdvance(), "enabled but nothing held");

ResetInput();
Hold(UnityEngine.KeyCode.LeftAlt);
Assert(ClickToAdvanceMod.ShouldSuppressAdvance(), "left Alt suppresses");

ResetInput();
Hold(UnityEngine.KeyCode.RightAlt);
Assert(ClickToAdvanceMod.ShouldSuppressAdvance(), "right Alt also suppresses");

ResetInput();
Hold(UnityEngine.KeyCode.Space);
Assert(!ClickToAdvanceMod.ShouldSuppressAdvance(), "unrelated key does not suppress while Alt is bound");

// 换成别的键后只认该键
ClickToAdvanceMod.SetBypassKey(UnityEngine.KeyCode.Space);
Assert(ClickToAdvanceMod.BypassKey == UnityEngine.KeyCode.Space, "rebinding persists in memory");

ResetInput();
Hold(UnityEngine.KeyCode.LeftAlt);
Assert(!ClickToAdvanceMod.ShouldSuppressAdvance(), "bound Space: Alt no longer suppresses");

ResetInput();
Hold(UnityEngine.KeyCode.Space);
Assert(ClickToAdvanceMod.ShouldSuppressAdvance(), "bound Space suppresses");

// None = 关闭屏蔽
ClickToAdvanceMod.SetBypassKey(UnityEngine.KeyCode.None);
ResetInput();
Hold(UnityEngine.KeyCode.Space);
Assert(!ClickToAdvanceMod.ShouldSuppressAdvance(), "None disables suppression");

// 非法键回退到 LeftAlt
ClickToAdvanceMod.SetBypassKey(UnityEngine.KeyCode.Mouse0);
Assert(ClickToAdvanceMod.BypassKey == UnityEngine.KeyCode.LeftAlt, "mouse button falls back to LeftAlt");

ClickToAdvanceMod.SetBypassKey(UnityEngine.KeyCode.Escape);
Assert(ClickToAdvanceMod.BypassKey == UnityEngine.KeyCode.LeftAlt, "Escape falls back to LeftAlt");

// 总开关关闭时不屏蔽
ClickToAdvanceMod.SetBypassKey(UnityEngine.KeyCode.LeftAlt);
ClickToAdvanceMod.SetBypassEnabled(false);
ResetInput();
Hold(UnityEngine.KeyCode.LeftAlt);
Assert(!ClickToAdvanceMod.ShouldSuppressAdvance(), "master switch off overrides held key");

// 设置界面注册：挂到游戏按键分区，且重复调用不重复添加
GameOptionRegistry.EnsureVanilla();
ClickToAdvanceMod.TryRegisterOptions();
Assert(GameOptionRegistry.Find(ClickToAdvanceMod.BypassEnabledOptionId) != null, "toggle row registered");
Assert(GameOptionRegistry.Find(ClickToAdvanceMod.BypassKeyOptionId) != null, "keybind row registered");
ClickToAdvanceMod.TryRegisterOptions();
var keybindSection = GameOptionRegistry.FindSection(GameOptionRegistry.SECTION_KEYBIND)!;
Assert(keybindSection.entries.Count == 2, "registration is idempotent");

Console.WriteLine("ALL PASS");

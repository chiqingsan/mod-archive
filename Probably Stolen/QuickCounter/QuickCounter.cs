using Il2Cpp;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(QuickCounter.QuickCounterMelon), "Quick Counter", "1.2.0", "chiqingsan")]
[assembly: MelonGame("Questing Goose Studio", "Probably Stolen")]

namespace QuickCounter;

public sealed class QuickCounterMelon : MelonMod
{
    private MelonPreferences_Entry<string> _startBusinessHotkey = null!;
    private MelonPreferences_Entry<string> _nextCustomerHotkey = null!;
    private MelonPreferences_Entry<string> _transferAllHotkey = null!;
    private MelonPreferences_Entry<string> _dismissCustomerHotkey = null!;
    private MelonPreferences_Entry<string> _callSecurityHotkey = null!;
    private MelonPreferences_Entry<string> _calendarHotkey = null!;
    private MelonPreferences_Entry<string> _tabletHotkey = null!;
    private MelonPreferences_Entry<string> _confirmTabletTradeHotkey = null!;
    private MelonPreferences_Entry<string> _trashHoveredItemHotkey = null!;

    private readonly HotkeyState _startBusinessState = new();
    private readonly HotkeyState _nextCustomerState = new();
    private readonly HotkeyState _transferAllState = new();
    private readonly HotkeyState _dismissCustomerState = new();
    private readonly HotkeyState _callSecurityState = new();
    private readonly HotkeyState _calendarState = new();
    private readonly HotkeyState _tabletState = new();
    private readonly HotkeyState _confirmTabletTradeState = new();
    private readonly HotkeyState _trashHoveredItemState = new();

    private readonly HotkeyBinding _startBusinessBinding = new();
    private readonly HotkeyBinding _nextCustomerBinding = new();
    private readonly HotkeyBinding _transferAllBinding = new();
    private readonly HotkeyBinding _dismissCustomerBinding = new();
    private readonly HotkeyBinding _callSecurityBinding = new();
    private readonly HotkeyBinding _calendarBinding = new();
    private readonly HotkeyBinding _tabletBinding = new();
    private readonly HotkeyBinding _confirmTabletTradeBinding = new();
    private readonly HotkeyBinding _trashHoveredItemBinding = new();

    private StoreBell? _storeBell;
    private bool _storeBellResolved;
    private StoreDismissButton? _dismissButton;
    private bool _dismissButtonResolved;

    public override void OnInitializeMelon()
    {
        var category = MelonPreferences.CreateCategory("QuickCounter", "Quick Counter 快捷键");
        _startBusinessHotkey = category.CreateEntry("StartBusinessHotkey", "B", "开始营业");
        _nextCustomerHotkey = category.CreateEntry("NextCustomerHotkey", "Q", "呼叫下一位顾客");
        _transferAllHotkey = category.CreateEntry("TransferAllHotkey", "E", "转移全部");
        _dismissCustomerHotkey = category.CreateEntry("DismissCustomerHotkey", "X", "遣散顾客（不呼叫下一位）");
        _callSecurityHotkey = category.CreateEntry("CallSecurityHotkey", "R", "呼叫治安部");
        _calendarHotkey = category.CreateEntry("CalendarHotkey", "H", "打开日历");
        _tabletHotkey = category.CreateEntry("TabletHotkey", "F", "打开平板电脑");
        _confirmTabletTradeHotkey = category.CreateEntry("ConfirmTabletTradeHotkey", "C", "确认平板电脑交易");
        _trashHoveredItemHotkey = category.CreateEntry("TrashHoveredItemHotkey", "G", "将鼠标下物品放入垃圾箱");
        RefreshHotkeyBindings();
        LoggerInstance.Msg("Quick Counter 已成功加载！");
    }

    public override void OnUpdate()
    {
        var ui = StoreUIManager.Instance;
        if (ui == null || ui.IsTyping())
            return;

        RefreshHotkeyBindings();
        var input = InputSnapshot.Capture();

        var held = IsHeld(_startBusinessBinding, input);
        if (IsTriggered(held, _startBusinessState, false, 0f))
        {
            var shutter = StoreShutterButton.Instance;
            if (shutter != null && shutter.isClosed)
                shutter.OnClick();

            return;
        }

        held = IsHeld(_nextCustomerBinding, input);
        if (IsMouseItemTransformConflict(_nextCustomerBinding, input, held))
        {
            ResetState(_nextCustomerState);
            ResetStoreBell();
        }
        else if (TryTriggerNextCustomer(held))
            return;

        held = IsHeld(_transferAllBinding, input);
        if (IsMouseItemTransformConflict(_transferAllBinding, input, held))
            ResetState(_transferAllState);
        else if (IsTriggered(held, _transferAllState, false, 0f))
        {
            var transfer = UnityEngine.Object.FindObjectOfType<StoreTransferButton>();
            transfer?.OnClick();
            return;
        }

        held = IsHeld(_dismissCustomerBinding, input);
        if (TryTriggerDismissCustomer(held))
            return;

        held = IsHeld(_callSecurityBinding, input);
        if (TryTriggerSecurity(held))
            return;

        held = IsHeld(_calendarBinding, input);
        if (IsTriggered(held, _calendarState, false, 0f))
        {
            ui.storeCalendar?.OnClick();
            return;
        }

        held = IsHeld(_tabletBinding, input);
        if (IsMouseItemTransformConflict(_tabletBinding, input, held))
            ResetState(_tabletState);
        else if (IsTriggered(held, _tabletState, false, 0f))
        {
            var tablet = UnityEngine.Object.FindObjectOfType<StoreTablet>();
            tablet?.OnClick();
            return;
        }

        held = IsHeld(_confirmTabletTradeBinding, input);
        if (IsTriggered(held, _confirmTabletTradeState, false, 0f))
        {
            ConfirmTabletTrade();
            return;
        }

        held = IsHeld(_trashHoveredItemBinding, input);
        if (IsTriggered(held, _trashHoveredItemState, false, 0f))
            ui.TryTrashHoveredItem();
    }

    private void ConfirmTabletTrade()
    {
        var negotiation = NegociationUIManager.Instance;
        if (negotiation != null && negotiation.uiObject != null && negotiation.uiObject.activeInHierarchy)
        {
            if (negotiation.confirmPanel != null && negotiation.confirmPanel.activeInHierarchy)
            {
                negotiation.OnConfirmPanelYes();
                return;
            }

            if (negotiation.acceptOfferButton != null && negotiation.acceptOfferButton.activeInHierarchy &&
                negotiation.acceptOfferButtonButton != null && negotiation.acceptOfferButtonButton.interactable)
            {
                negotiation.OnOfferAcceptClick();
                return;
            }

            if (negotiation.wholesaleButton != null && negotiation.wholesaleButton.gameObject.activeInHierarchy &&
                negotiation.wholesaleButton.interactable)
            {
                negotiation.OnWholesaleClick();
                return;
            }

            if (negotiation.acceptMultipleButton != null && negotiation.acceptMultipleButton.gameObject.activeInHierarchy &&
                negotiation.acceptMultipleButton.interactable)
            {
                negotiation.OnAcceptMultipleButtonClick();
                return;
            }
        }

        var pc = PCUIManager.Instance;
        if (pc == null || pc.panel == null || !pc.panel.activeInHierarchy || pc.storeRequestElements == null)
            return;

        foreach (var element in pc.storeRequestElements)
        {
            if (element == null)
                continue;

            var button = element.acceptButton;
            if (button == null || !button.gameObject.activeInHierarchy)
                continue;

            element.OnClick();
            return;
        }

        LoggerInstance.Warning("平板电脑上没有可确认的交易。");
    }

    private bool TryTriggerNextCustomer(bool held)
    {
        if (!held)
        {
            ResetState(_nextCustomerState);
            ResetStoreBell();
            return false;
        }

        if (!_storeBellResolved)
        {
            _storeBell = UnityEngine.Object.FindObjectOfType<StoreBell>();
            _storeBellResolved = true;
        }

        var bell = _storeBell;
        if (bell == null)
        {
            ResetState(_nextCustomerState);
            ResetStoreBell();
            return false;
        }

        if (!IsTriggered(true, _nextCustomerState, bell.ShouldRequireHold(), bell.HoldDuration))
            return false;

        bell.OnClick();
        return true;
    }

    private bool TryTriggerDismissCustomer(bool held)
    {
        if (!held)
        {
            ResetState(_dismissCustomerState);
            ResetDismissButton();
            return false;
        }

        if (!_dismissButtonResolved)
        {
            _dismissButton = UnityEngine.Object.FindObjectOfType<StoreDismissButton>();
            _dismissButtonResolved = true;
        }

        var dismiss = _dismissButton;
        if (dismiss == null)
        {
            ResetState(_dismissCustomerState);
            ResetDismissButton();
            return false;
        }

        if (!IsTriggered(true, _dismissCustomerState, dismiss.ShouldRequireHold(), dismiss.HoldDuration))
            return false;

        dismiss.OnClick();
        return true;
    }

    private bool TryTriggerSecurity(bool held)
    {
        if (!held)
        {
            ResetState(_callSecurityState);
            return false;
        }

        var security = StoreSecButton.Instance;
        if (security == null)
        {
            ResetState(_callSecurityState);
            return false;
        }

        if (!IsTriggered(true, _callSecurityState, security.ShouldRequireHold(), security.HoldDuration))
            return false;

        security.OnClick();
        return true;
    }

    private static bool IsTriggered(bool held, HotkeyState state, bool requiresHold, float holdDuration)
    {
        if (!held)
        {
            ResetState(state);
            return false;
        }

        if (state.Triggered)
            return false;

        if (!requiresHold)
        {
            state.Triggered = true;
            return true;
        }

        state.Elapsed += Time.unscaledDeltaTime;
        var duration = Math.Max(holdDuration, 0.01f);
        ShowHoldRing(state, Math.Min(state.Elapsed / duration, 1f));
        if (state.Elapsed < duration)
            return false;

        HideHoldRing(state);
        state.Triggered = true;
        return true;
    }

    private static bool IsHeld(HotkeyBinding binding, InputSnapshot input)
    {
        return binding.Valid &&
               binding.Ctrl == input.Ctrl &&
               binding.Shift == input.Shift &&
               binding.Alt == input.Alt &&
               Input.GetKey(binding.Key);
    }

    private static bool IsMouseItemTransformConflict(HotkeyBinding binding, InputSnapshot input, bool held)
    {
        return held && input.MouseLeft && !binding.Ctrl && !binding.Shift && !binding.Alt &&
               (binding.Key == KeyCode.Q || binding.Key == KeyCode.E || binding.Key == KeyCode.F);
    }

    private static void ShowHoldRing(HotkeyState state, float progress)
    {
        var manager = CustomUIManager.Instance;
        if (manager == null)
            return;

        manager.ShowHoldRing(progress);
        state.RingVisible = true;
    }

    private static void HideHoldRing(HotkeyState state)
    {
        if (!state.RingVisible)
            return;

        CustomUIManager.Instance?.HideHoldRing();
        state.RingVisible = false;
    }

    private static void ResetState(HotkeyState state)
    {
        if (!state.Triggered && !state.RingVisible && state.Elapsed <= 0f)
            return;

        HideHoldRing(state);
        state.Reset();
    }

    private void ResetStoreBell()
    {
        _storeBell = null;
        _storeBellResolved = false;
    }

    private void ResetDismissButton()
    {
        _dismissButton = null;
        _dismissButtonResolved = false;
    }

    private void RefreshHotkeyBindings()
    {
        RefreshHotkeyBinding(_startBusinessHotkey.Value, _startBusinessBinding);
        RefreshHotkeyBinding(_nextCustomerHotkey.Value, _nextCustomerBinding);
        RefreshHotkeyBinding(_transferAllHotkey.Value, _transferAllBinding);
        RefreshHotkeyBinding(_dismissCustomerHotkey.Value, _dismissCustomerBinding);
        RefreshHotkeyBinding(_callSecurityHotkey.Value, _callSecurityBinding);
        RefreshHotkeyBinding(_calendarHotkey.Value, _calendarBinding);
        RefreshHotkeyBinding(_tabletHotkey.Value, _tabletBinding);
        RefreshHotkeyBinding(_confirmTabletTradeHotkey.Value, _confirmTabletTradeBinding);
        RefreshHotkeyBinding(_trashHoveredItemHotkey.Value, _trashHoveredItemBinding);
    }

    private static void RefreshHotkeyBinding(string hotkey, HotkeyBinding binding)
    {
        if (string.Equals(hotkey, binding.Source, StringComparison.Ordinal))
            return;

        binding.Source = hotkey;
        binding.Valid = TryParseHotkey(hotkey, out var key, out var ctrl, out var shift, out var alt);
        binding.Key = key;
        binding.Ctrl = ctrl;
        binding.Shift = shift;
        binding.Alt = alt;
    }

    private static bool TryParseHotkey(string hotkey, out KeyCode key, out bool ctrl, out bool shift, out bool alt)
    {
        key = KeyCode.None;
        ctrl = false;
        shift = false;
        alt = false;
        var parts = hotkey.Split('+', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return false;

        for (var i = 0; i < parts.Length - 1; i++)
        {
            var modifier = parts[i];
            if (modifier.Equals("CTRL", StringComparison.OrdinalIgnoreCase) ||
                modifier.Equals("CONTROL", StringComparison.OrdinalIgnoreCase))
                ctrl = true;
            else if (modifier.Equals("SHIFT", StringComparison.OrdinalIgnoreCase))
                shift = true;
            else if (modifier.Equals("ALT", StringComparison.OrdinalIgnoreCase))
                alt = true;
            else
                return false;
        }

        return Enum.TryParse(parts[^1], true, out key) && key != KeyCode.None;
    }

    private sealed class HotkeyState
    {
        public bool Triggered { get; set; }
        public bool RingVisible { get; set; }
        public float Elapsed { get; set; }

        public void Reset()
        {
            Triggered = false;
            RingVisible = false;
            Elapsed = 0f;
        }
    }

    private sealed class HotkeyBinding
    {
        public string? Source { get; set; }
        public KeyCode Key { get; set; }
        public bool Ctrl { get; set; }
        public bool Shift { get; set; }
        public bool Alt { get; set; }
        public bool Valid { get; set; }
    }

    private readonly struct InputSnapshot
    {
        private InputSnapshot(bool ctrl, bool shift, bool alt, bool mouseLeft)
        {
            Ctrl = ctrl;
            Shift = shift;
            Alt = alt;
            MouseLeft = mouseLeft;
        }

        public bool Ctrl { get; }
        public bool Shift { get; }
        public bool Alt { get; }
        public bool MouseLeft { get; }

        public static InputSnapshot Capture()
        {
            return new InputSnapshot(
                Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl),
                Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift),
                Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt),
                Input.GetMouseButton(0));
        }
    }
}

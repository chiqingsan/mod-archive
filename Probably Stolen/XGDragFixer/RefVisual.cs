using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace XGDragFixer;

/// <summary>
/// 两项逻辑优化：陈化机结算折叠、模块效果重算去重。
/// 物品视觉刷新保持原版同步执行。
/// 每项都可单独关闭；目标缺失时只记 [兼容] 日志，行为保持原版。
/// </summary>
internal static class RefVisual
{
    private static bool _suppress;

    internal static bool AgingEnabled => Core.AgingCollapse?.Value == true;

    internal static bool ModuleEnabled => Core.ModuleRecomputeDedup?.Value == true;

    internal static void Install(Harmony harmony)
    {
        var ok = 0;

        void Try(string label, MethodBase? target, string? prefix = null, string? postfix = null, string? finalizer = null)
        {
            if (target == null)
            {
                Core.Log?.Log($"[兼容] 未找到 {label}，跳过该项");
                return;
            }

            try
            {
                harmony.Patch(target,
                    prefix == null ? null : new HarmonyMethod(AccessTools.Method(typeof(RefVisual), prefix)),
                    postfix == null ? null : new HarmonyMethod(AccessTools.Method(typeof(RefVisual), postfix)),
                    finalizer: finalizer == null ? null : new HarmonyMethod(AccessTools.Method(typeof(RefVisual), finalizer)));
                ok++;
            }
            catch (Exception ex)
            {
                Core.Log?.Warning($"{label} 补丁失败: {ex.Message}");
            }
        }

        // 2) 陈化机按天循环折叠（原版在该方法内按天循环，这里一次结算）
        Try("陈化机结算折叠", FindAgingClosure(), nameof(AgingPrefix), finalizer: nameof(AgingFinalizer));

        // 3) 模块效果重算去重
        Try("模块动作结算去重",
            AccessTools.Method(typeof(MachineHelper), nameof(MachineHelper.OnMachineActioned),
                new[] { typeof(GameItem), typeof(GameInventory) }),
            nameof(ModuleActionedPrefix), finalizer: nameof(ModuleFinalizer));
        Try("模块临时属性重算",
            AccessTools.Method(typeof(ModuleEffectHelper), nameof(ModuleEffectHelper.ComputeAllModuleTempStat),
                new[] { typeof(List<GameItem>) }),
            nameof(SkipModuleRecalcPrefix));
        Try("模块效果重算",
            AccessTools.Method(typeof(ModuleHelper), nameof(ModuleHelper.ComputeModuleEffect),
                new[] { typeof(List<GameItem>), typeof(GameItem) }),
            nameof(SkipModuleRecalcPrefix));
        Try("模块基础效果应用",
            AccessTools.Method(typeof(ModuleHelper), nameof(ModuleHelper.ApplyBasicModuleEffect),
                new[] { typeof(GameInventory), typeof(GameItem), typeof(GameItem) }),
            nameof(SkipModuleRecalcPrefix));

        Core.Log?.Log($"[优化] 视觉与机器结算优化已挂接 {ok} 项");
    }

    internal static void Reset()
    {
        _suppress = false;
    }

    // 目标闭包方法 <AgeWell>b__5 的签名是 void(GameItem, GameInventory, SlotMarker)，
    // 没有返回值；机器/酒架/电池/模块都从闭包的 DisplayClass 字段注入。
    private static bool AgingPrefix(GameItem ___machine, GameGridInventory ___wineSlot,
        GameSlotInventory ___itemInventoryBattery, GameGridInventory ___itemInventoryModule, out long __state)
    {
        __state = Diagnostics.Begin();
        if (!AgingEnabled || ReferenceEquals(___machine, null) || ReferenceEquals(___wineSlot, null)
            || ReferenceEquals(___itemInventoryBattery, null) || ReferenceEquals(___itemInventoryModule, null))
        {
            return true;
        }

        var wines = GraphUtils.FindAllChildrenType<GameItem>(___wineSlot, MachineWineRack.CanAgeWine);
        if (wines.Count == 0 || !MachineHelper.TryDrawCyclePower(___machine, ___itemInventoryBattery))
        {
            // 原版同样不会陈化，直接跳过逐天循环。
            return false;
        }

        var days = MachineWineRack.GetAgingDays(___machine);
        foreach (var wine in wines)
        {
            if (wine.IsTag("WINE_COUNTERFEIT_TAG"))
            {
                continue;
            }

            var perDay = wine.IsTag("AGE_VALUE_MOD_PER_DAY_INT")
                ? wine.GetTagReadonly("AGE_VALUE_MOD_PER_DAY_INT").GetInt()
                : 3;
            wine.ModifyTag("AGE_INT", state =>
            {
                state.Enable();
                state.SetInt(state.GetInt() + days);
            }, true);
            wine.unitValue += (long)days * perDay;
        }

        foreach (var wine in wines)
        {
            wine.ValidateShapeState();
        }

        MachineHelper.OnMachineActioned(___machine, ___itemInventoryModule);
        return false;
    }

    private static bool ModuleActionedPrefix(GameItem machine, GameInventory moduleGrid, out long __state)
    {
        __state = Diagnostics.Begin();
        if (!ModuleEnabled || _suppress || ReferenceEquals(machine, null) || ReferenceEquals(moduleGrid, null))
        {
            return true;
        }

        try
        {
            _suppress = true;
            foreach (var child in moduleGrid.childItems)
            {
                ModuleEffectHelper.ActivateOnUseAction(child, moduleGrid, machine);
            }
        }
        finally
        {
            _suppress = false;
        }

        var children = moduleGrid.childItems;
        ModuleEffectHelper.ComputeAllModuleTempStat(children);
        ModuleHelper.ComputeModuleEffect(moduleGrid, null, machine);
        ModuleHelper.ApplyBasicModuleEffect(moduleGrid, null, machine);

        return false;
    }

    private static Exception? AgingFinalizer(Exception? __exception, long __state)
    {
        Diagnostics.End(Diagnostics.Probe.Aging, __state, __exception);
        return __exception;
    }

    private static Exception? ModuleFinalizer(Exception? __exception, long __state)
    {
        Diagnostics.End(Diagnostics.Probe.Module, __state, __exception);
        return __exception;
    }

    private static bool SkipModuleRecalcPrefix()
    {
        return !(ModuleEnabled && _suppress);
    }

    /// <summary>
    /// 定位陈化机按天循环所在的编译器闭包方法。
    /// 优先按参考实现验证过的编译器命名硬定位：MachineWineRack+&lt;&gt;c__DisplayClass3_0 / &lt;AgeWell&gt;b__5；
    /// 游戏更新导致编译器命名变化时，退回“名字含 AgeWell 且 IL 调用 TryDrawCyclePower”的扫描。
    /// </summary>
    private static MethodBase? FindAgingClosure()
    {
        var displayClass = AccessTools.TypeByName("MachineWineRack+<>c__DisplayClass3_0");
        if (displayClass != null)
        {
            foreach (var method in displayClass.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (method.Name == "<AgeWell>b__5")
                {
                    return method;
                }
            }
        }

        var tryDraw = AccessTools.Method(typeof(MachineHelper), nameof(MachineHelper.TryDrawCyclePower),
            new[] { typeof(GameItem), typeof(GameSlotInventory) });
        if (tryDraw == null)
        {
            return null;
        }

        foreach (var nested in typeof(MachineWineRack).GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (nested.Name.IndexOf("<>c", StringComparison.Ordinal) < 0)
            {
                continue;
            }

            foreach (var method in nested.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (method.Name.IndexOf("AgeWell", StringComparison.Ordinal) < 0)
                {
                    continue;
                }

                if (IlCalls(method, tryDraw))
                {
                    return method;
                }
            }
        }

        return null;
    }

    private static bool IlCalls(MethodBase method, MethodBase target)
    {
        try
        {
            var body = method.GetMethodBody();
            var il = body?.GetILAsByteArray();
            if (il == null)
            {
                return false;
            }

            var module = method.Module;
            for (var i = 0; i + 4 < il.Length; i++)
            {
                var op = il[i];
                if (op != 0x28 && op != 0x6F && op != 0x73)
                {
                    continue;
                }

                var token = BitConverter.ToInt32(il, i + 1);
                try
                {
                    if (module.ResolveMethod(token) == target)
                    {
                        return true;
                    }
                }
                catch
                {
                    // Not a method token.
                }
            }
        }
        catch
        {
            // Body unavailable.
        }

        return false;
    }
}

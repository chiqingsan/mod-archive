using System;
using HarmonyLib;

namespace XGDragFixer;

/// <summary>
/// 化学精炼机（压片）流程的标签校验合并。
///
/// 原版点一次「产出」依次调用：
///   ChemicalHelper.RemoveAllLiquid(chemical)        —— 每种液体 ModifyTag（默认校验）
///   ChemicalHelper.TransferLiquidToChem(...)        —— RemoveAndReturn 每批 ModifyTag + AddLiquidToChemFromList 每种液体 ModifyTag（默认校验）
///   MachineChemFinisher.IntroduceImpurity(...)      —— RemoveVolume（skipValidate:true）+ AddChemContentUL 的 ModifyTag（默认校验）
/// 每次默认校验都会完整执行 GameItem.ValidateShapeState（更新 modifiedShape/state、递归父物品并刷新库存 UI）。
///
/// 本补丁只在这三个方法执行期间临时跳过 ValidateShapeState，并在每个方法结束时补做一次；
/// 标签数据写入路径（ModifyTag 的 action）完全不变，只是把同一批标签写入后的校验合并。
/// </summary>
internal static class ChemFinisherFix
{
    internal static bool Enabled => Core.PressBatch?.Value == true;

    /// <summary>当前处于校验合并窗口内的方法调用深度。</summary>
    private static int _depth;

    internal static void Install(Harmony harmony)
    {
        var ok = 0;

        void Try(string label, System.Reflection.MethodBase? target, string? prefix = null, string? postfix = null,
            string? finalizer = null)
        {
            if (target == null)
            {
                Core.Log?.Log($"[兼容] 未找到 {label}，跳过该项");
                return;
            }

            try
            {
                harmony.Patch(target,
                    prefix == null ? null : new HarmonyMethod(AccessTools.Method(typeof(ChemFinisherFix), prefix)),
                    postfix == null ? null : new HarmonyMethod(AccessTools.Method(typeof(ChemFinisherFix), postfix)),
                    finalizer: finalizer == null ? null : new HarmonyMethod(AccessTools.Method(typeof(ChemFinisherFix), finalizer)));
                ok++;
            }
            catch (Exception ex)
            {
                Core.Log?.Warning($"{label} 补丁失败: {ex.Message}");
            }
        }

        // 压片流程的三个入口（RemoveAllLiquid 还被 SetNomoreInjectorPurity 调用，行为同样等价）。
        Try("压片液体清空",
            AccessTools.Method(typeof(ChemicalHelper), nameof(ChemicalHelper.RemoveAllLiquid),
                new[] { typeof(GameItem) }),
            nameof(BeginScopePrefix), nameof(RemoveAllLiquidPostfix), nameof(ResetOnErrorFinalizer));
        Try("压片液体转移",
            AccessTools.Method(typeof(ChemicalHelper), nameof(ChemicalHelper.TransferLiquidToChem),
                new[] { typeof(GameItem), typeof(GameItem), typeof(int) }),
            nameof(BeginScopePrefix), nameof(TransferLiquidPostfix), nameof(ResetOnErrorFinalizer));
        Try("压片杂质注入",
            AccessTools.Method(typeof(MachineChemFinisher), nameof(MachineChemFinisher.IntroduceImpurity),
                new[] { typeof(GameItem), typeof(int) }),
            nameof(BeginScopePrefix), nameof(IntroduceImpurityPostfix), nameof(ResetOnErrorFinalizer));

        // 抑制目标：窗口期内跳过形状校验（标签写入不受影响）。
        Try("形状校验合并",
            AccessTools.Method(typeof(GameItem), "ValidateShapeState", Type.EmptyTypes),
            nameof(ValidateShapeStatePrefix));

        Core.Log?.Log($"[优化] 压片校验优化已挂接 {ok} 项");
    }

    private static void BeginScopePrefix()
    {
        if (Enabled)
        {
            _depth++;
        }
    }

    private static void RemoveAllLiquidPostfix(object[]? __args)
    {
        EndScope(__args, 0, -1);
    }

    private static void TransferLiquidPostfix(object[]? __args)
    {
        // 参数: (container, chemicalItem, volumeInPart)；容器与化学品都补一次校验。
        EndScope(__args, 0, 1);
    }

    private static void IntroduceImpurityPostfix(object[]? __args)
    {
        EndScope(__args, 0, -1);
    }

    private static void EndScope(object[]? __args, int firstIndex, int secondIndex)
    {
        if (_depth > 0)
        {
            _depth--;
        }

        if (_depth != 0 || !Enabled)
        {
            return;
        }

        SafeValidate(__args, firstIndex);
        SafeValidate(__args, secondIndex);
    }

    private static void SafeValidate(object[]? args, int index)
    {
        if (args == null || index < 0 || index >= args.Length || args[index] is not GameItem item)
        {
            return;
        }

        try
        {
            item.ValidateShapeState();
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"压片收尾校验失败: {ex.Message}");
        }
    }

    private static Exception? ResetOnErrorFinalizer(Exception? __exception)
    {
        // 方法抛异常时 postfix 不执行，由 finalizer 兜底复位；正常路径 postfix 已归零。
        if (__exception != null && _depth > 0)
        {
            _depth--;
        }

        return __exception;
    }

    private static bool ValidateShapeStatePrefix()
    {
        return !(Enabled && _depth > 0);
    }
}

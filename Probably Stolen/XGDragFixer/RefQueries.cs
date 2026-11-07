using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace XGDragFixer;

/// <summary>
/// 用占用位图判断容器是否还有空位。
/// 全店物品查询保持原版实现，不挂接 FindAllItem / GetAllItems。
/// </summary>
internal static class RefQueries
{
    private static readonly FieldInfo? GridItemsField = AccessTools.Field(typeof(GameGridInventory), "items");

    private static bool[] _occupied = new bool[1024];

    internal static bool OccupancyEnabled => Core.GridOccupancyShortcut?.Value == true;

    internal static void Install(Harmony harmony)
    {
        var ok = 0;

        void Try(string label, MethodBase? target, string prefix, string? postfix = null, string? finalizer = null)
        {
            if (target == null)
            {
                Core.Log?.Log($"[兼容] 未找到 {label}，跳过该项");
                return;
            }

            try
            {
                harmony.Patch(target,
                    new HarmonyMethod(AccessTools.Method(typeof(RefQueries), prefix)),
                    postfix == null ? null : new HarmonyMethod(AccessTools.Method(typeof(RefQueries), postfix)),
                    finalizer: finalizer == null ? null : new HarmonyMethod(AccessTools.Method(typeof(RefQueries), finalizer)));
                ok++;
            }
            catch (Exception ex)
            {
                Core.Log?.Warning($"{label} 补丁失败: {ex.Message}");
            }
        }

        if (OccupancyEnabled)
        {
            Try("容器空位位图",
                AccessTools.Method(typeof(GameGridInventory), nameof(GameGridInventory.HasFreeCell), Type.EmptyTypes),
                nameof(HasFreeCellPrefix), finalizer: nameof(HasFreeCellFinalizer));
        }
        Core.Log?.Log($"[优化] 查询优化已挂接 {ok} 项");
    }

    internal static void Reset()
    {
        _occupied = new bool[1024];
    }

    private static bool HasFreeCellPrefix(GameGridInventory __instance, ref bool __result, out long __state)
    {
        __state = Diagnostics.Begin();
        if (!OccupancyEnabled)
        {
            return true;
        }

        try
        {
            var shape = __instance.inventoryShape;
            if (shape == null)
            {
                return true;
            }

            var width = shape.width;
            var height = shape.height;
            if (width <= 0 || height <= 0 || width * height > 4096)
            {
                return true;
            }

            var items = GridItemsField?.GetValue(__instance) as List<GameItem>;
            if (items == null)
            {
                return true;
            }

            var cells = width * height;
            if (_occupied.Length < cells)
            {
                _occupied = new bool[cells];
            }
            else
            {
                Array.Clear(_occupied, 0, cells);
            }

            var occupied = _occupied;
            for (var index = 0; index < items.Count; index++)
            {
                var modifiedShape = items[index]?.modifiedShape;
                if (modifiedShape == null)
                {
                    return true;
                }

                var minX = modifiedShape.minX;
                var minY = modifiedShape.minY;
                var maxX = modifiedShape.maxX;
                var maxY = modifiedShape.maxY;
                if (minX < 0 || minY < 0 || maxX >= width || maxY >= height)
                {
                    return true;
                }

                for (var y = minY; y <= maxY; y++)
                {
                    var row = y * width;
                    for (var x = minX; x <= maxX; x++)
                    {
                        if (modifiedShape.Intersects(x, y))
                        {
                            occupied[row + x] = true;
                        }
                    }
                }
            }

            for (var y = 0; y < height; y++)
            {
                var row = y * width;
                for (var x = 0; x < width; x++)
                {
                    if (shape.Get(x, y) == 0 && !occupied[row + x])
                    {
                        __result = true;
                        return false;
                    }
                }
            }

            __result = false;
            return false;
        }
        catch (Exception ex)
        {
            Diagnostics.Count("空位位图异常回退");
            Core.Log?.Warning($"容器空位位图失败，回退原版: {ex.GetType().Name}: {ex.Message}");
            return true;
        }
    }

    private static Exception? HasFreeCellFinalizer(Exception? __exception, long __state)
    {
        Diagnostics.End(Diagnostics.Probe.Occupancy, __state, __exception);
        return __exception;
    }

}

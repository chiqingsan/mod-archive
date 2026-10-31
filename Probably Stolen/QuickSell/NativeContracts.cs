using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace QuickSell;

internal static class NativeContracts
{
    internal static void CheckParenting()
    {
        // Constructor-free, detached objects only: exercise the same native
        // graph contract without creating graphics, IDs, inventory or save data.
        var item = (GameItem)FormatterServices.GetUninitializedObject(typeof(GameItemElement));
        var inventory = (GameInventory)FormatterServices.GetUninitializedObject(typeof(GameGridInventory));
        var add = typeof(GameItem).GetMethod(nameof(GameItem.AddParent))!;
        var remove = typeof(GameItem).GetMethod(nameof(GameItem.RemoveParent))!;
        if (!(bool)add.Invoke(item, new object[] { inventory })! || !ReferenceEquals(item.parentInventory, inventory))
            throw new InvalidOperationException("GameItem.AddParent 合同检查失败，停止启用交易助手。");
        if (!(bool)remove.Invoke(item, new object[] { inventory })! || item.parentInventory != null)
            throw new InvalidOperationException("GameItem.RemoveParent 合同检查失败，停止启用交易助手。");
    }
}

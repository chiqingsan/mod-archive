using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuickSell;

// 原版 EmporiumEntry.GetAllItems() 用 GraphUtils.FindAllChildrenType 遍历商店物品图；
// 图里一旦出现空节点（存档或其它模组留下的异常状态），它会抛空引用，游戏自己的
// EmporiumEntry.Update 也会每秒报一次。面板和日结清理只需要「当前存在的物品编号」，
// 所以这里用带空值保护的同构遍历代替：遇到坏节点跳过并记录一次日志，不再中断界面。
internal static class StoreGraph
{
    private static readonly List<GraphNodeStorage> pending = new List<GraphNodeStorage>();
    private static readonly HashSet<int> seen = new HashSet<int>();
    private static bool warned;

    internal static List<GameItem> GetAllItems()
    {
        var items = new List<GameItem>();
        var entry = EmporiumEntry.Instance;
        if (entry == null) return items;
        pending.Clear();
        seen.Clear();
        Collect(entry.invElement, items);
        Collect(entry.backInvinvElement, items);
        Collect(entry.frontInvinvElement, items);
        Collect(entry.docInvElement, items);
        Collect(entry.showcaseElement, items);
        Collect(entry.trashInvElement, items);
        Collect(entry.hiddenElement, items);
        Collect(entry.afterhourWindow, items);
        Collect(entry.vendingFountainElement, items);
        Collect(entry.vendingMachineElement, items);
        return items;
    }

    private static void Collect(GraphNodeStorage root, List<GameItem> items)
    {
        if (root == null) return;
        pending.Add(root);
        while (pending.Count > 0)
        {
            int last = pending.Count - 1;
            var node = pending[last];
            pending.RemoveAt(last);
            if (node == null)
            {
                Warn();
                continue;
            }
            if (node is GameItem item && item.uniqueId > 0 && seen.Add(item.uniqueId)) items.Add(item);
            List<GraphNodeStorage> children;
            try
            {
                children = node.children;
            }
            catch (Exception error)
            {
                Warn();
                Debug.LogWarning("[QuickSell] 读取商店物品图子节点失败，已跳过该分支：" + error.GetType().Name);
                continue;
            }
            if (children == null)
            {
                Warn();
                continue;
            }
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                if (child == null) Warn();
                else pending.Add(child);
            }
        }
    }

    private static void Warn()
    {
        if (warned) return;
        warned = true;
        Debug.LogWarning("[QuickSell] 商店物品图存在空节点或异常分支，已跳过；原版 GetAllItems 在该状态下会报空引用（游戏自身 Update 同样在报，属存档或其它模组造成的问题）。");
    }
}

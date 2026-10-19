using UnityEngine;

namespace LootRarityMarker
{
    /// <summary>
    /// 档位 → 颜色。索引就是 <see cref="RarityRules"/> 的档位（0=白 … 6=深红）。
    ///
    /// 颜色直接取自工坊 mod「物品价值稀有度与搜索音效」的
    /// `ModBehaviour.{White,Green,Blue,Purple,Orange,LightRed,Red}`
    /// （反编译源码里的 `ColorUtility.TryParseHtmlString` 那几行）：
    ///   White #FFFFFF  Green #7CFF7C  Blue #7CD5FF  Purple #D0ACFF
    ///   Orange #FFDC24  LightRed #FF5858  Red #BB0000
    /// 那个 mod 的 alpha 是给 UI 文字底纹用的（0x40/0x96），这里只取 RGB，
    /// 透明度交给 <see cref="MarkerTint"/> 沿用交互点原本的 alpha。
    ///
    /// 用它的颜色而不是游戏自己的品质色，是为了让交互点的颜色和物品名的颜色**一模一样** ——
    /// 玩家是靠那个 mod 给名字上的色来判断「这是什么级别」的。
    /// </summary>
    internal static class QualityPalette
    {
        private static readonly Color[] Colors =
        {
            new Color(1.0000f, 1.0000f, 1.0000f, 1f), // 0 White    #FFFFFF
            new Color(0.4863f, 1.0000f, 0.4863f, 1f), // 1 Green    #7CFF7C
            new Color(0.4863f, 0.8353f, 1.0000f, 1f), // 2 Blue     #7CD5FF
            new Color(0.8157f, 0.6745f, 1.0000f, 1f), // 3 Purple   #D0ACFF
            new Color(1.0000f, 0.8627f, 0.1412f, 1f), // 4 Orange   #FFDC24
            new Color(1.0000f, 0.3451f, 0.3451f, 1f), // 5 LightRed #FF5858
            new Color(0.7333f, 0.0000f, 0.0000f, 1f), // 6 Red      #BB0000
        };

        /// <summary>取某档的颜色。越界按白处理。零分配。</summary>
        public static Color Get(int level)
        {
            if (level < 0 || level >= Colors.Length)
            {
                return Colors[0];
            }
            return Colors[level];
        }
    }
}

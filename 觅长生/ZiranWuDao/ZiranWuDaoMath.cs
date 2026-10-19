using System;

namespace ZiranWuDao;

/// <summary>
/// 自然悟道结算算法（纯函数，不依赖游戏程序集，便于独立验证）。
///
/// 术语：
///   思绪 / 感悟：游戏内部称 LingGuang（灵光），一条记录含 startTime（获得时刻）、guoqiTime（存活天数）。
///   总量 total：该条思绪手动感悟可获得的悟道经验总量 = studyTime × 品阶倍率。
///   自然结算：随时间流逝按月自动发放的悟道经验。
///   微经验：以千分之一经验为单位的整数，用于避免浮点误差并保留跨月余数。
/// </summary>
public static class ZiranWuDaoMath
{
    /// <summary>月索引：把日期折算成「自 1 年 1 月起的月序号」，仅用于计算两个时刻之间跨越的日历月数。</summary>
    private static long MonthIndex(DateTime t)
    {
        return (long)t.Year * 12L + t.Month;
    }

    /// <summary>两个时刻之间跨越的日历月数（与游戏 Avatar.AddTime 内 num 的算法一致）。</summary>
    public static int MonthsBetween(DateTime from, DateTime to)
    {
        return (int)(((to.Year - from.Year) * 12) + (to.Month - from.Month));
    }

    /// <summary>
    /// 推进时间，复刻游戏 WorldTime.addTime 的运算顺序（年 → 月 → 日）。
    ///
    /// 顺序必须与游戏一致：DateTime 的加法顺序不同结果会差一天，进而影响月数计算。
    /// 例如 1 月 30 日 + 1 月 + 1 天，先加月得 3 月 1 日（跨 2 个日历月），
    /// 先加日得 2 月 28 日（跨 1 个日历月）——后者会少结算一个月。
    /// </summary>
    public static DateTime AdvanceTime(DateTime from, int days, int months, int years)
    {
        return from.AddYears(years).AddMonths(months).AddDays(days);
    }

    /// <summary>
    /// 计算一条思绪在本次时间推进中应结算的月数。
    /// </summary>
    /// <param name="before">本次时间推进前的游戏时刻</param>
    /// <param name="after">本次时间推进后的游戏时刻</param>
    /// <param name="startTime">思绪的获得时刻</param>
    /// <param name="expireTime">思绪的到期时刻（获得时刻 + 存活天数）</param>
    /// <param name="alreadyMonths">该思绪此前已自然结算的月数</param>
    /// <param name="maxSettleMonths">单条思绪自然结算的月数上限</param>
    /// <returns>本次应结算的月数（0 表示不结算）</returns>
    public static int MonthsToSettle(
        DateTime before,
        DateTime after,
        DateTime startTime,
        DateTime expireTime,
        int alreadyMonths,
        int maxSettleMonths)
    {
        int remaining = maxSettleMonths - alreadyMonths;
        if (remaining <= 0)
        {
            return 0;
        }

        // 起点不早于本次推进时刻，也不早于思绪获得时刻：安装 mod 之前的历史不追补。
        DateTime from = before > startTime ? before : startTime;
        // 终点不晚于本次推进时刻，也不晚于到期时刻：中途过期的思绪只结算到过期为止。
        DateTime to = after < expireTime ? after : expireTime;

        if (MonthIndex(to) <= MonthIndex(from))
        {
            return 0;
        }

        int months = (int)(MonthIndex(to) - MonthIndex(from));
        return months < remaining ? months : remaining;
    }

    /// <summary>
    /// 计算本次应发放的经验。
    ///
    /// 以「结算后的累计月数」重算累计应得，再减去已发放部分，因此玩家中途调整配置项
    /// （月比例 / 总量上限月数）也只会按新配置重新收敛，不会重复发放或倒退。
    /// 不足 1 点经验的零头保留在 paidMilli 里，等后续月份凑满再发。
    ///
    /// 累计值上限取 int.MaxValue：结算进度要存回思绪的整数字段，若被 mod 加入超大 studyTime
    /// 导致超界，截断会把进度变成负数、引发重复发放，所以在源头就封顶。
    /// </summary>
    /// <param name="totalExp">该条思绪的悟道经验总量</param>
    /// <param name="totalMonths">本次结算后的累计月数（已结算月数 + 本次月数）</param>
    /// <param name="perMonthPermille">每月的结算比例（千分比，0.5% = 5）</param>
    /// <param name="maxTotalPermille">累计发放上限（总量的千分比，6% = 60）</param>
    /// <param name="paidMilli">已发放的微经验（千分之一经验），出参为更新后的值</param>
    /// <returns>本次实际发放的整数经验</returns>
    public static int GrantExp(
        int totalExp,
        int totalMonths,
        int perMonthPermille,
        int maxTotalPermille,
        ref long paidMilli)
    {
        if (totalExp <= 0 || totalMonths <= 0)
        {
            return 0;
        }

        long capMilli = (long)totalExp * maxTotalPermille;
        long earnedMilli = (long)totalExp * perMonthPermille * totalMonths;
        long targetMilli = earnedMilli < capMilli ? earnedMilli : capMilli;
        if (targetMilli > int.MaxValue)
        {
            targetMilli = int.MaxValue;
        }

        if (targetMilli <= paidMilli)
        {
            return 0;
        }

        long granted = (targetMilli - paidMilli) / 1000L;
        if (granted <= 0)
        {
            return 0;
        }
        paidMilli += granted * 1000L;
        return (int)granted;
    }

    /// <summary>
    /// 构造结算提示文本（单条大道）。一条大道一条提示，长度短且可预测，
    /// 从根本上避免气泡内换行（游戏气泡单行实测约 28 个半角宽）。
    /// </summary>
    /// <param name="prefix">消息前缀，如「自然悟道：」</param>
    /// <param name="daoName">大道名称，如「剑道」（见 FormatDaoName）</param>
    /// <param name="amount">本次该大道发放的经验</param>
    public static string BuildTip(string prefix, string daoName, int amount)
    {
        return prefix + daoName + "+" + amount;
    }

    /// <summary>
    /// 取大道类型的显示名。
    ///
    /// 类型表同时有 name（单字，如"体"）和 name1（完整称呼，如"体道"/"金之道"）。
    /// **name1 必须原样使用，不能再拼「之道」**——曾经因此得到「体道之道」的错误提示。
    /// name1 缺失时用 name 兜底：单字补「道」（体→体道、剑→剑道，与真实配表一致），
    /// 多字补「之道」；全缺时退化为「大道N」。
    /// 类型表可被前置框架扩展（如「灵界」mod 追加的 45 灵道），所以每一级都要容错。
    /// </summary>
    public static string FormatDaoName(string name1, string name, int type)
    {
        if (!string.IsNullOrEmpty(name1))
        {
            return name1;
        }
        if (!string.IsNullOrEmpty(name))
        {
            if (name.EndsWith("道"))
            {
                return name;
            }
            return name + (name.Length == 1 ? "道" : "之道");
        }
        return "大道" + type;
    }
}

using System;
using ZiranWuDao;

/// <summary>
/// 自然悟道结算算法的独立验证程序：直接引用游戏侧同一份 ZiranWuDaoMath.cs，
/// 逐条断言需求里的边界条件。用 dotnet run 执行。
/// </summary>
internal static class Program
{
	private const int PerMonth = 5;    // 0.5% → 千分之 5
	private const int MaxMonths = 12;  // 单条思绪最多结算 12 个月
	private const int MaxTotal = 60;   // 累计上限 6% → 千分之 60

	private static int _failed;
	private static int _passed;

	private static void Main()
	{
		// 场景 1：思绪完整存活，逐月结算到 12 个月上限即止（6%）。
		{
			DateTime start = new DateTime(1, 1, 1);
			DateTime expire = start.AddDays(365);
			const int total = 1200;
			CheckTotal(start, expire, total, 12, "12 个月刚好发满 6%");
		}

		// 场景 2（用户补充边界）：思绪只存在 8 个月，玩家却闭关 36 个月 → 只结算 8 个月。
		{
			DateTime start = new DateTime(1, 1, 1);
			DateTime expire = start.AddDays(243); // 约 8 个月
			int settled = SimulateMonths(start, expire, new DateTime(1, 1, 1), new DateTime(4, 1, 1));
			AssertEqual(8, settled, "存在 8 个月的思绪，闭关 36 个月只结算 8 个月");
		}

		// 场景 3（用户补充边界）：已结算 4 次的 14 个月思绪，再闭关 → 受 12 上限约束只结算 8 个月。
		{
			DateTime start = new DateTime(1, 1, 1);
			DateTime expire = start.AddDays(420); // 约 14 个月
			int already = 4;
			int settled = SimulateMonths(start, expire, new DateTime(1, 5, 1), new DateTime(4, 1, 1), already);
			AssertEqual(8, settled, "已结算 4 次的 14 个月思绪，闭关后只再结算 8 个月（总 12 个月）");
		}

		// 场景 4：不满一个月的时间推进不结算。
		{
			DateTime start = new DateTime(1, 1, 1);
			DateTime expire = start.AddDays(365);
			int settled = SimulateMonths(start, expire, new DateTime(1, 1, 1), new DateTime(1, 1, 20));
			AssertEqual(0, settled, "未跨月的时间推进不结算");
		}

		// 场景 5：一次跨 3 个月，正常补足 3 个月。
		{
			DateTime start = new DateTime(1, 1, 1);
			DateTime expire = start.AddDays(365);
			int settled = SimulateMonths(start, expire, new DateTime(1, 1, 1), new DateTime(1, 4, 1));
			AssertEqual(3, settled, "一次跨 3 个月补足 3 个月");
		}

		// 场景 6：安装 mod 之前就已存在的思绪，不追补历史收益（起点被推进前时刻限制）。
		{
			DateTime start = new DateTime(1, 1, 1);
			DateTime expire = start.AddDays(3650);
			int settled = SimulateMonths(start, expire, new DateTime(2, 6, 1), new DateTime(2, 7, 1));
			AssertEqual(1, settled, "老存档既有思绪不追补安装前的历史");
		}

		// 场景 7：经验零头跨月累积，不会因取整丢失。
		{
			long paid = 0;
			int total = 1234;
			int g1 = ZiranWuDaoMath.GrantExp(total, 1, PerMonth, MaxTotal, ref paid);
			int g2 = ZiranWuDaoMath.GrantExp(total, 2, PerMonth, MaxTotal, ref paid);
			int sum = g1 + g2;
			// 1234 × 0.5% = 6.17/月，2 个月共 12.34 → 累计发放 12 点（零头留存）。
			AssertEqual(12, sum, "两个月零头累积后发放 12 点（而非 6+6=12 的巧合值校验）");
		}

		// 场景 8：累计发放永不超过总量上限 6%。
		{
			long paid = 0;
			int total = 1234;
			int sum = 0;
			for (int m = 1; m <= MaxMonths; m++)
			{
				sum += ZiranWuDaoMath.GrantExp(total, m, PerMonth, MaxTotal, ref paid);
			}
			AssertEqual((int)(total * MaxTotal / 1000L), sum, "12 个月累计发放 = floor(总量 × 6%)，不超上限");
		}

		// 场景 9：MaxSettleMonths 与 MaxTotalPercent 两个上限独立生效（取更严者）。
		{
			long paid = 0;
			// 月数上限放宽到 20，但总量上限仍是 6%：总量先到顶。
			int sum = 0;
			for (int m = 1; m <= 20; m++)
			{
				sum += ZiranWuDaoMath.GrantExp(1200, m, PerMonth, MaxTotal, ref paid);
			}
			AssertEqual(72, sum, "月数上限放宽到 20 时，总量仍被 6% = 72 点卡住");

			// 总量上限收紧到 3%，月数上限仍是 12：总量提前到顶。
			paid = 0;
			sum = 0;
			for (int m = 1; m <= 12; m++)
			{
				sum += ZiranWuDaoMath.GrantExp(1200, m, PerMonth, 30, ref paid);
			}
			AssertEqual(36, sum, "总量上限收紧到 3% = 36 点，提前到顶后不再发放");
		}

		// 场景 10：已满 12 个月后不再结算任何月份。
		{
			DateTime start = new DateTime(1, 1, 1);
			DateTime expire = start.AddDays(3650);
			int settled = SimulateMonths(start, expire, new DateTime(1, 1, 1), new DateTime(3, 1, 1), MaxMonths);
			AssertEqual(0, settled, "已满 12 个月的思绪不再结算");
		}

		// 场景 11：思绪在本次跨月区间中途到期，只结算到到期那个月。
		{
			DateTime start = new DateTime(1, 1, 1);
			DateTime expire = new DateTime(1, 5, 20); // 4 月 20 日到期
			int settled = SimulateMonths(start, expire, new DateTime(1, 4, 25), new DateTime(1, 9, 1));
			AssertEqual(1, settled, "区间中途到期的思绪只结算到到期所在月");
		}

		// 场景 12：提示文案 —— 大道名直接用配表的完整称呼，不得重复拼「之道」。
		{
			// 配表里 name1 本身就是完整称呼：金之道 / 体道 / 剑道 / 灵道。
			// 出现过的真实 bug：name1("体道") + "之道" = 「体道之道」。
			string dao = ZiranWuDaoMath.FormatDaoName("体道", "体", 7);
			AssertText("体道", dao, "name1 原样使用，不重复拼接「之道」（无「体道之道」）");

			string msg = ZiranWuDaoMath.BuildTip("自然悟道：", dao, 46);
			AssertText("自然悟道：体道+46", msg, "提示文本拼接正确");
		}

		// 场景 12b：大道名格式化的逐级退化（应对其它 mod 扩展的类型表缺字段）。
		{
			AssertText("金之道", ZiranWuDaoMath.FormatDaoName("金之道", "金", 1), "name1 优先");
			AssertText("剑道", ZiranWuDaoMath.FormatDaoName(null, "剑", 8), "name1 缺失时用单字 name 补「道」");
			AssertText("水道", ZiranWuDaoMath.FormatDaoName(null, "水", 3), "name1 缺失时单字 name 补「道」");
			AssertText("灵道", ZiranWuDaoMath.FormatDaoName(null, "灵道", 45), "name 已是完整称呼时原样使用，不重复拼接");
			AssertText("大道45", ZiranWuDaoMath.FormatDaoName(null, null, 45), "全缺失时退化为「大道N」，不抛异常");
			AssertText("灵道", ZiranWuDaoMath.FormatDaoName("灵道", "灵", 45), "其它 mod 追加的类型（灵道）正常显示");
		}

		// 场景 12c：时间推进顺序必须与游戏 WorldTime.addTime 一致（年 → 月 → 日）。
		{
			// 1 月 30 日 + 1 月 + 1 天：游戏先加月得 3 月 1 日（跨 2 个日历月）；
			// 若先加日会得 2 月 28 日（跨 1 个日历月），导致少结算一个月。
			DateTime from = new DateTime(1, 1, 30);
			DateTime after = ZiranWuDaoMath.AdvanceTime(from, 1, 1, 0);
			AssertText(new DateTime(1, 3, 1).ToString("yyyy/MM/dd"), after.ToString("yyyy/MM/dd"), "时间推进顺序与游戏一致（年→月→日）");
			AssertEqual(2, ZiranWuDaoMath.MonthsBetween(from, after), "跨月数按游戏算法得出 2 个月");

			// 跨年 + 月 + 日的组合也一致。
			DateTime after2 = ZiranWuDaoMath.AdvanceTime(new DateTime(1, 12, 31), 1, 1, 1);
			AssertText(new DateTime(3, 2, 1).ToString("yyyy/MM/dd"), after2.ToString("yyyy/MM/dd"), "跨年组合推进结果与游戏一致");
		}

		// 场景 13：提示文案 —— 每个大道一条独立提示，每条都短，不折行。
		{
			// 游戏气泡一行实测约容得下 28 个半角宽（中文按 2 计），
			// 「自然悟道：土之道+38、剑道+17」= 28 宽即触发折行。
			string[] names = { "土之道", "剑道", "气道", "体道", "神道", "阵道", "金之道", "水之道" };
			int[] amounts = { 38, 17, 12, 9, 7, 5, 4, 3 };
			int maxWidth = 0;
			for (int i = 0; i < names.Length; i++)
			{
				string msg = ZiranWuDaoMath.BuildTip("自然悟道：", names[i], amounts[i]);
				int w = DisplayWidth(msg);
				if (w > maxWidth)
				{
					maxWidth = w;
				}
			}
			AssertTrue(maxWidth <= 28, $"每个大道一条提示，最长 {maxWidth} 宽，均在气泡单行内（≤28）");
		}

		// 场景 14：提示文案 —— 数字很长时（高经验），单条提示宽度仍然可控。
		{
			string msg = ZiranWuDaoMath.BuildTip("自然悟道：", "金之道", 99999);
			AssertText("自然悟道：金之道+99999", msg, "长数字下单条提示内容正确");
			AssertTrue(DisplayWidth(msg) <= 28, "长数字下宽度仍在气泡单行内");
		}

		// 场景 15：极端数据（如其它 mod 塞入超大 studyTime）不能让累计值溢出成负数。
		{
			long paid = 0;
			int total = int.MaxValue;
			int sum = 0;
			for (int m = 1; m <= MaxMonths; m++)
			{
				sum += ZiranWuDaoMath.GrantExp(total, m, PerMonth, MaxTotal, ref paid);
			}
			AssertTrue(paid >= 0, $"超大总量的累计进度不为负（paid={paid}）");
			AssertTrue(sum > 0, "超大总量仍能正常发放，不退化为不发");
		}

		// 场景 16（配置组合）：把月比例改成 1% 时，总量上限 6% 会提前生效。
		{
			long paid = 0;
			int total = 10000;
			int sum = 0;
			int month7 = -1;
			for (int m = 1; m <= 12; m++)
			{
				int g = ZiranWuDaoMath.GrantExp(total, m, 10, 60, ref paid); // 1%/月，上限 6%
				if (m == 7)
				{
					month7 = g;
				}
				sum += g;
			}
			AssertEqual(600, sum, "月比例 1% + 总量上限 6%：累计仍封顶 6%（600）");
			AssertEqual(0, month7, "月比例 1% + 总量上限 6%：第 7 个月已到顶，不再发放");
		}

		// 场景 17（配置组合）：想要 1% × 12 个月 = 12% 的收益，需同时放开通量上限。
		{
			long paid = 0;
			int total = 10000;
			int sum = 0;
			for (int m = 1; m <= 12; m++)
			{
				sum += ZiranWuDaoMath.GrantExp(total, m, 10, 120, ref paid); // 1%/月，上限 12%
			}
			AssertEqual(1200, sum, "月比例 1% + 总量上限 12%：累计 12%（1200）");
		}

		// 场景 18（配置组合）：中途改配置安全——按累计月数重算，只补差额，不重复发放也不倒退。
		{
			long paid = 0;
			int total = 10000;
			int before = 0;
			for (int m = 1; m <= 6; m++)
			{
				before += ZiranWuDaoMath.GrantExp(total, m, 5, 60, ref paid); // 先按 0.5% 走 6 个月
			}
			int after = ZiranWuDaoMath.GrantExp(total, 7, 10, 60, ref paid); // 第 7 个月起按 1%
			AssertEqual(300, before, "改配置前按 0.5% 累计 3%（300）");
			AssertEqual(300, after, "改成 1% 后只补差额至 6%（补 300），不重复发放");
		}

		Console.WriteLine();
		Console.WriteLine($"通过 {_passed} 项，失败 {_failed} 项。");
		if (_failed > 0)
		{
			Environment.Exit(1);
		}
	}

	/// <summary>估算提示文本的显示宽度：中文/全角按 2、其余按 1，用于校验不超气泡单行。</summary>
	private static int DisplayWidth(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return 0;
		}
		int width = 0;
		foreach (char c in text)
		{
			width += c > 0x2E7F ? 2 : 1;
		}
		return width;
	}

	/// <summary>按「一次推进跨越 months 个月」的方式模拟一条思绪的累计结算月数。</summary>
	private static int SimulateMonths(DateTime start, DateTime expire, DateTime before, DateTime after, int alreadyMonths = 0)
	{
		int settle = ZiranWuDaoMath.MonthsToSettle(before, after, start, expire, alreadyMonths, MaxMonths);
		return settle;
	}

	/// <summary>校验一条完整存活到期的思绪，累计发放恰好是 floor(总量 × 6%)。</summary>
	private static void CheckTotal(DateTime start, DateTime expire, int total, int months, string name)
	{
		long paid = 0;
		int sum = 0;
		for (int m = 1; m <= months; m++)
		{
			sum += ZiranWuDaoMath.GrantExp(total, m, PerMonth, MaxTotal, ref paid);
		}
		AssertEqual(total * MaxTotal / 1000, sum, name);
	}

	private static void AssertEqual(int expected, int actual, string name)
	{
		if (expected == actual)
		{
			_passed++;
			Console.WriteLine($"  [OK]   {name}（期望 {expected}，实际 {actual}）");
		}
		else
		{
			_failed++;
			Console.WriteLine($"  [FAIL] {name}（期望 {expected}，实际 {actual}）");
		}
	}

	private static void AssertText(string expected, string actual, string name)
	{
		if (expected == actual)
		{
			_passed++;
			Console.WriteLine($"  [OK]   {name}（{actual}）");
		}
		else
		{
			_failed++;
			Console.WriteLine($"  [FAIL] {name}（期望「{expected}」，实际「{actual}」）");
		}
	}

	private static void AssertTrue(bool condition, string name)
	{
		if (condition)
		{
			_passed++;
			Console.WriteLine($"  [OK]   {name}");
		}
		else
		{
			_failed++;
			Console.WriteLine($"  [FAIL] {name}");
		}
	}
}

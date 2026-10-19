namespace DeepSpacePawnshop.FutureTech;

internal static class FutureText
{
	internal static string PerkName => "未来科技";

	internal static string PerkDescription => "王老菊未来科技有限公司入职礼包：开局获得未来科技净水器、未来科技高密度电池各一件。其他未来科技设备可在杰克逊博士处购买。董事长说，先从基础水做起。";

	internal static (string Name, string Description) Item(string id)
	{
		return id switch
		{
			"dspf_future_purifier" => (Name: "未来科技净水器", Description: FutureEdition.HighQualityPurifier ? "海德拉的技术，未来科技的野心。对水使用可直接得到优质水；滤芯会磨损，放入电池过夜便能恢复耐久。董事长说，水质可以加班。" : "由海德拉微型净水器改装，可将水净化为基础水。滤芯会磨损，放入电池过夜便能恢复耐久。董事长说，技术迭代要循序渐进。"), 
			"dspf_future_charger" => (Name: "未来科技充电器", Description: "把电池放进去，过一晚，每块电池可恢复 9 点电量；解锁商业供电后恢复 18 点。员工可以下班，电池不行。"), 
			"dspf_future_module" => (Name: "未来科技模组", Description: $"装上后，设备的性能、效率与质量各提升 {200}%。董事长已经花十分钟完全了解了这台机器。"), 
			"dspf_future_aging_cabinet" => (Name: "未来科技陈化柜", Description: "放入可陈化的酒或其他物品，再放入电池；每个处理夜消耗 12 点电量。未完成发酵的酒一夜酿成，之后继续陈化。铁锭可以扩容。董事长承诺：酒有工龄，员工没有。"), 
			"dspf_future_nuclear_battery" => (Name: "未来科技高密度电池", Description: "出厂即充满，可储存 1000 点电量。未来科技有限公司的续航焦虑已被董事长开除。"), 
			"dspf_future_pocket" => (Name: "未来科技四次元口袋", Description: "什么都能往里装，带着去拾荒也方便。搜查时，口袋里的东西也不会被发现。装得下满仓货物，装不下董事长的新点子。"), 
			"dspf_future_void_jug" => (Name: "虚空科技水壶", Description: "能装下 18 升水，是普通大水壶的三倍。董事长说，还能装；虚空也只好点头。"), 
			_ => (Name: id, Description: id), 
		};
	}
}

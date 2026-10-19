using JSONClass;
using KBEngine;

public static class PlayerTalkCmdHelper
{
	public static string ReplacePlayerTalkToNPCWord(this string str, UINPCData npc)
	{
		str = DaoYouReplace(str, npc);
		return str;
	}

	private static string DaoYouReplace(string str, UINPCData npc)
	{
		string daoyou = "道友";
		daoyou = DaoYou_TongMenReplace(daoyou, npc);
		daoyou = DaoYou_ShiTuReplace(daoyou, npc);
		daoyou = DaoYou_XiongDiReplace(daoyou, npc);
		daoyou = DaoYou_SpecialNPCReplace(daoyou, npc);
		daoyou = DaoYou_LevelReplace(daoyou, npc);
		str = str.Replace("{daoyouPlayer}", daoyou);
		return str;
	}

	private static string DaoYou_TongMenReplace(string daoyou, UINPCData npc)
	{
		Avatar player = PlayerEx.Player;
		if (player.menPai != 0 && npc.MenPai == player.menPai)
		{
			int levelType = player.getLevelType();
			daoyou = ((npc.BigLevel > levelType) ? ((npc.BigLevel < 5 || levelType >= 5) ? "前辈" : "师祖") : ((npc.BigLevel < levelType) ? ((levelType < 3) ? ((npc.Sex == 1) ? "师弟" : "师妹") : "师侄") : ((npc.Sex != 1) ? ((player.level >= npc.Level) ? "师妹" : "师姐") : ((player.level > npc.Level) ? "师弟" : "师兄"))));
		}
		return daoyou;
	}

	private static string DaoYou_ShiTuReplace(string daoyou, UINPCData npc)
	{
		if (PlayerEx.IsTheather(npc.ID))
		{
			daoyou = "师傅";
		}
		return daoyou;
	}

	private static string DaoYou_XiongDiReplace(string daoyou, UINPCData npc)
	{
		if (PlayerEx.IsBrother(npc.ID))
		{
			daoyou = ((npc.IsZhongYaoNPC && npc.ZhongYaoNPCID == 615) ? "百里兄" : ((!npc.IsZhongYaoNPC || npc.ZhongYaoNPCID != 609) ? ((npc.Sex == 1) ? "兄弟" : "姑娘") : "倪兄"));
		}
		return daoyou;
	}

	private static string DaoYou_SpecialNPCReplace(string daoyou, UINPCData npc)
	{
		if (npc.IsZhongYaoNPC && NpcTalkSpecialNPCAddress.DataDict.ContainsKey(npc.ZhongYaoNPCID))
		{
			NpcTalkSpecialNPCAddress npcTalkSpecialNPCAddress = NpcTalkSpecialNPCAddress.DataDict[npc.ZhongYaoNPCID];
			daoyou = ((!PlayerEx.IsDaoLv(npc.ID)) ? npcTalkSpecialNPCAddress.normalAddress : npcTalkSpecialNPCAddress.daolvAddress);
		}
		return daoyou;
	}

	private static string DaoYou_LevelReplace(string daoyou, UINPCData npc)
	{
		int levelType = PlayerEx.Player.getLevelType();
		if (daoyou == "道友")
		{
			if (levelType < npc.BigLevel)
			{
				daoyou = "前辈";
			}
			else if (levelType > npc.BigLevel)
			{
				daoyou = "小友";
			}
		}
		return daoyou;
	}
}

using System.Collections.Generic;
using JSONClass;

namespace Tab;

public class WuDaoUtils
{
	private static WuDaoUtils _inst;

	public Dictionary<int, Dictionary<int, List<int>>> WuDaoDictionary = new Dictionary<int, Dictionary<int, List<int>>>();

	public List<int> HasStudy = new List<int>();

	public static WuDaoUtils Inst
	{
		get
		{
			if (_inst == null)
			{
				_inst = new WuDaoUtils();
			}
			return _inst;
		}
	}

	public void Update()
	{
		WuDaoDictionary = new Dictionary<int, Dictionary<int, List<int>>>();
		HasStudy = new List<int>();
		foreach (WuDaoJson value in WuDaoJson.DataDict.Values)
		{
			foreach (int item in value.Type)
			{
				if (!WuDaoDictionary.ContainsKey(item))
				{
					WuDaoDictionary.Add(item, new Dictionary<int, List<int>>());
				}
				if (!WuDaoDictionary[item].ContainsKey(value.Lv))
				{
					WuDaoDictionary[item].Add(value.Lv, new List<int>());
				}
				WuDaoDictionary[item][value.Lv].Add(value.id);
			}
		}
		foreach (int key in WuDaoDictionary.Keys)
		{
			List<int> list = PlayerEx.Player.wuDaoMag.getWuDaoStudy(key).ToList();
			if (list.Count <= 0)
			{
				continue;
			}
			foreach (int item2 in list)
			{
				if (!HasStudy.Contains(item2))
				{
					HasStudy.Add(item2);
				}
			}
		}
	}
}

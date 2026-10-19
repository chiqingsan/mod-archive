using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using script.Steam;

public static class WorkshopTool
{
	private static string workshopRootPath;

	private static string disableModPath;

	private static List<string> disableMods;

	public static string WorkshopRootPath
	{
		get
		{
			if (string.IsNullOrEmpty(workshopRootPath))
			{
				workshopRootPath = Application.dataPath + "/../../../workshop/content/1189490";
			}
			return workshopRootPath;
		}
	}

	public static string DisableModPath
	{
		get
		{
			if (string.IsNullOrEmpty(disableModPath))
			{
				disableModPath = Application.dataPath + "/../DontLoadModsList.txt";
			}
			return disableModPath;
		}
	}

	public static DirectoryInfo GetModDirectory(string workshopID)
	{
		return new DirectoryInfo(WorkshopRootPath + "/" + workshopID);
	}

	public static List<DirectoryInfo> GetAllModDirectory()
	{
		List<DirectoryInfo> list = new List<DirectoryInfo>();
		DirectoryInfo directoryInfo = new DirectoryInfo(WorkshopRootPath);
		if (directoryInfo.Exists)
		{
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				FileInfo[] files = directoryInfo2.GetFiles();
				for (int j = 0; j < files.Length; j++)
				{
					if (files[j].Name == "Mod.bin")
					{
						list.Add(directoryInfo2);
						break;
					}
				}
			}
		}
		return list;
	}

	public static List<DirectoryInfo> GetAllModChildDirectoryByName(string directoryName)
	{
		List<DirectoryInfo> list = new List<DirectoryInfo>();
		foreach (DirectoryInfo item in GetAllModDirectory())
		{
			DirectoryInfo[] directories = item.GetDirectories();
			foreach (DirectoryInfo directoryInfo in directories)
			{
				if (directoryInfo.Name == directoryName)
				{
					list.Add(directoryInfo);
					break;
				}
			}
		}
		return list;
	}

	public static bool CheckModIsDisable(string workshopID)
	{
		if (disableMods == null)
		{
			InitDisableMods();
		}
		return disableMods.Contains(workshopID);
	}

	private static void InitDisableMods()
	{
		disableMods = new List<string>();
		if (!File.Exists(DisableModPath))
		{
			return;
		}
		string[] array = File.ReadAllLines(DisableModPath);
		foreach (string text in array)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				disableMods.Add(text);
			}
		}
	}

	public static void OpenMod(string workshopID)
	{
		if (disableMods == null)
		{
			InitDisableMods();
		}
		if (disableMods.Contains(workshopID))
		{
			disableMods.Remove(workshopID);
			File.WriteAllLines(disableModPath, disableMods);
		}
	}

	public static void CloseMod(string workshopID)
	{
		if (disableMods == null)
		{
			InitDisableMods();
		}
		if (!disableMods.Contains(workshopID))
		{
			disableMods.Add(workshopID);
			File.WriteAllLines(disableModPath, disableMods);
		}
	}

	public static string GetModsDetails()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("创意工坊文件夹的Mod列表:");
		foreach (DirectoryInfo item in GetAllModDirectory())
		{
			string path = item.FullName + "/Mod.bin";
			if (File.Exists(path))
			{
				try
				{
					WorkShopItem workShopItem = new WorkShopItem();
					FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
					workShopItem = (WorkShopItem)new BinaryFormatter().Deserialize(fileStream);
					fileStream.Close();
					stringBuilder.AppendLine(workShopItem.Title + "\tPath:" + item.FullName);
				}
				catch
				{
					stringBuilder.AppendLine("未成功读取的Mod.bin\tPath:" + item.FullName);
				}
			}
		}
		stringBuilder.AppendLine("本地测试的Mod列表:");
		DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + "/../本地Mod测试");
		if (directoryInfo.Exists)
		{
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				stringBuilder.AppendLine(directoryInfo2.FullName);
			}
		}
		return stringBuilder.ToString();
	}
}

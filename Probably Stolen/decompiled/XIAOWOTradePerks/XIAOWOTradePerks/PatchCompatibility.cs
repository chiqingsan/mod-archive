using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MelonLoader;

namespace XIAOWOTradePerks;

internal static class PatchCompatibility
{
	private static readonly Dictionary<string, Harmony> Active = new Dictionary<string, Harmony>(StringComparer.Ordinal);

	private static readonly HashSet<string> Failed = new HashSet<string>(StringComparer.Ordinal);

	private static string _ownerPrefix = string.Empty;

	private static bool _reported;

	private static string _lastReport = string.Empty;

	internal static int FailedModuleCount => Failed.Count;

	internal static int InstalledModuleCount => Active.Count;

	internal static int PossibleConflictCount { get; private set; }

	internal static int ConfirmedConflictCount { get; private set; }

	internal static int DuplicateOwnPatchCount { get; private set; }

	internal static string LastReport => _lastReport;

	internal static bool Available(string module)
	{
		return !Failed.Contains(module);
	}

	internal static void Install(Harmony root)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		Dispose();
		_ownerPrefix = root.Id + ".feature.";
		foreach (IGrouping<string, Type> item in (from type in typeof(Core).Assembly.GetTypes()
			where type.IsDefined(typeof(HarmonyPatch), inherit: false)
			select type).GroupBy(ModuleFor).OrderBy<IGrouping<string, Type>, string>((IGrouping<string, Type> group) => group.Key, StringComparer.Ordinal))
		{
			Harmony val = new Harmony(_ownerPrefix + item.Key);
			try
			{
				int num = 0;
				foreach (Type item2 in item.OrderBy<Type, string>((Type type) => type.FullName, StringComparer.Ordinal))
				{
					num += val.CreateClassProcessor(item2).Patch().Count;
				}
				if (item.Key == "owner")
				{
					OwnerDealCompatibility.Install(val);
				}
				Active.Add(item.Key, val);
				Instance log = Core.Log;
				if (log != null)
				{
					log.Msg("Patch module " + item.Key + " ready (" + num + " targets).");
				}
			}
			catch (Exception ex)
			{
				val.UnpatchSelf();
				Failed.Add(item.Key);
				Instance log2 = Core.Log;
				if (log2 != null)
				{
					log2.Error("Patch module " + item.Key + " disabled; other modules remain available: " + ex);
				}
			}
		}
	}

	internal static string ReportSharedMethods()
	{
		if (_reported)
		{
			return _lastReport;
		}
		_reported = true;
		List<string> list = new List<string>();
		HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
		HashSet<string> hashSet2 = new HashSet<string>(StringComparer.Ordinal);
		foreach (MethodBase item in Harmony.GetAllPatchedMethods().OrderBy<MethodBase, string>((MethodBase method) => method.DeclaringType?.FullName, StringComparer.Ordinal).ThenBy<MethodBase, string>((MethodBase method) => method.Name, StringComparer.Ordinal))
		{
			Patches patchInfo = Harmony.GetPatchInfo(item);
			if (patchInfo == null || !patchInfo.Owners.Any(IsOurs) || patchInfo.Owners.Where((string owner) => !IsOurs(owner)).Distinct().ToArray()
				.Length == 0)
			{
				continue;
			}
			string target = MethodName(item);
			(string, Patch)[] source = NamedPatches(patchInfo).ToArray();
			string text = string.Join(", ", source.Select(((string Kind, Patch Value) patch) => patch.Kind + ":" + (IsOurs(patch.Value.owner) ? "ours" : patch.Value.owner) + "/" + patch.Value.PatchMethod?.DeclaringType?.Name + "/p" + patch.Value.priority));
			list.Add(target + ": " + text);
			foreach (IGrouping<string, (string, Patch)> item2 in from @group in source.GroupBy<(string, Patch), string>(((string Kind, Patch Value) patch) => patch.Kind + "|" + patch.Value.owner + "|" + PatchMethodName(patch.Value.PatchMethod), StringComparer.Ordinal)
				where @group.Count() > 1
				select @group)
			{
				(string, Patch) tuple = item2.First();
				if (IsOurs(tuple.Item2.owner))
				{
					DuplicateOwnPatchCount++;
				}
				hashSet2.Add("目标方法 " + target + " 存在重复的" + PatchKindName(tuple.Item1) + "补丁 " + PatchMethodName(tuple.Item2.PatchMethod) + "，来源 MOD：" + OwnerName(tuple.Item2.owner) + "，共注册 " + item2.Count() + " 次");
			}
			(string, Patch)[] source2 = (from patch in source.Where(((string Kind, Patch Value) patch) => !IsOurs(patch.Value.owner)).ToArray()
				where !IsKnownCompatible(target, patch)
				select patch).ToArray();
			string text2 = OwnerNames(source2.Select(((string Kind, Patch Value) patch) => patch.Value.owner));
			if (source2.Any(((string Kind, Patch Value) patch) => patch.Kind == "transpile"))
			{
				hashSet.Add("目标方法 " + target + " 与 MOD " + text2 + " 共同使用 Transpiler");
			}
			if (source2.Any(((string Kind, Patch Value) patch) => patch.Kind == "pre" && CanSkipOriginal(patch.Value.PatchMethod)))
			{
				hashSet.Add("目标方法 " + target + " 可能被 MOD " + OwnerNames(from patch in source2
					where patch.Kind == "pre" && CanSkipOriginal(patch.Value.PatchMethod)
					select patch.Value.owner) + " 的前置补丁跳过原方法");
			}
			if (source.Any(((string Kind, Patch Value) patch) => IsOurs(patch.Value.owner) && WritesResult(patch.Value.PatchMethod)) && source2.Any(((string Kind, Patch Value) patch) => WritesResult(patch.Value.PatchMethod)))
			{
				hashSet.Add("目标方法 " + target + " 的返回值也会被 MOD " + OwnerNames(from patch in source2
					where WritesResult(patch.Value.PatchMethod)
					select patch.Value.owner) + " 修改");
			}
		}
		foreach (string item3 in Failed)
		{
			hashSet2.Add("主 MOD 功能模块 " + item3 + " 已停用");
		}
		PossibleConflictCount = hashSet.Count;
		ConfirmedConflictCount = hashSet2.Count;
		Instance log = Core.Log;
		if (log != null)
		{
			log.Msg("MOD 兼容性检测：共享方法=" + list.Count + "，潜在冲突=" + hashSet.Count + "，确认冲突=" + hashSet2.Count + "，停用模块=" + Failed.Count + "。");
		}
		foreach (string item4 in hashSet)
		{
			Instance log2 = Core.Log;
			if (log2 != null)
			{
				log2.Warning("可能存在 MOD 冲突：" + item4);
			}
		}
		foreach (string item5 in hashSet2)
		{
			Instance log3 = Core.Log;
			if (log3 != null)
			{
				log3.Error("确认存在 MOD 冲突：" + item5);
			}
		}
		_lastReport = "shared=" + list.Count + ", possible=" + hashSet.Count + ", confirmed=" + hashSet2.Count + ", installed=" + Active.Count + ", disabled=" + Failed.Count + "\n" + string.Join("\n", list) + ((hashSet.Count == 0) ? string.Empty : ("\nPOSSIBLE\n" + string.Join("\n", hashSet))) + ((hashSet2.Count == 0) ? string.Empty : ("\nCONFIRMED\n" + string.Join("\n", hashSet2)));
		return _lastReport;
	}

	internal static void Dispose()
	{
		foreach (Harmony value in Active.Values)
		{
			try
			{
				value.UnpatchSelf();
			}
			catch (Exception ex)
			{
				Instance log = Core.Log;
				if (log != null)
				{
					log.Warning("Patch cleanup failed for " + value.Id + ": " + ex);
				}
			}
		}
		Active.Clear();
		Failed.Clear();
		PossibleConflictCount = 0;
		ConfirmedConflictCount = 0;
		DuplicateOwnPatchCount = 0;
		_reported = false;
		_lastReport = string.Empty;
	}

	private static bool IsOurs(string owner)
	{
		if (!string.IsNullOrEmpty(owner))
		{
			return owner.StartsWith(_ownerPrefix, StringComparison.Ordinal);
		}
		return false;
	}

	private static IEnumerable<(string Kind, Patch Value)> NamedPatches(Patches info)
	{
		return info.Prefixes.Select((Patch patch) => ("pre", patch: patch)).Concat(info.Postfixes.Select((Patch patch) => ("post", patch: patch))).Concat(info.Transpilers.Select((Patch patch) => ("transpile", patch: patch)))
			.Concat(info.Finalizers.Select((Patch patch) => ("final", patch: patch)));
	}

	private static string MethodName(MethodBase method)
	{
		return (method.DeclaringType?.Name ?? "<unknown>") + "." + method.Name;
	}

	private static string PatchMethodName(MethodInfo method)
	{
		if (!(method == null))
		{
			return (method.DeclaringType?.Name ?? "<unknown>") + "." + method.Name;
		}
		return "<unknown>";
	}

	private static bool CanSkipOriginal(MethodInfo method)
	{
		if (method != null)
		{
			return method.ReturnType == typeof(bool);
		}
		return false;
	}

	private static bool WritesResult(MethodInfo method)
	{
		if (method != null)
		{
			return method.GetParameters().Any((ParameterInfo parameter) => parameter.Name == "__result" && parameter.ParameterType.IsByRef);
		}
		return false;
	}

	private static string OwnerNames(IEnumerable<string> owners)
	{
		string[] array = owners.Where((string owner) => !string.IsNullOrWhiteSpace(owner)).Select(OwnerName).Distinct<string>(StringComparer.Ordinal)
			.OrderBy<string, string>((string name) => name, StringComparer.Ordinal)
			.ToArray();
		if (array.Length != 0)
		{
			return string.Join(", ", array);
		}
		return "未知 MOD";
	}

	private static string OwnerName(string owner)
	{
		if (string.IsNullOrWhiteSpace(owner))
		{
			return "未知 MOD";
		}
		int num = owner.IndexOf(',');
		int num2 = owner.IndexOf(':');
		int num3 = ((num >= 0 && num2 >= 0) ? Math.Min(num, num2) : Math.Max(num, num2));
		if (num3 <= 0)
		{
			return owner;
		}
		return owner.Substring(0, num3);
	}

	private static string PatchKindName(string kind)
	{
		return kind switch
		{
			"pre" => "前置", 
			"post" => "后置", 
			"transpile" => "转译", 
			"final" => "收尾", 
			_ => kind, 
		};
	}

	private static bool IsKnownCompatible(string target, (string Kind, Patch Value) patch)
	{
		string text = OwnerName(patch.Value.owner);
		string text2 = patch.Value.PatchMethod?.DeclaringType?.Name ?? string.Empty;
		switch (target)
		{
		case "GameItem.CanTarget":
			if (!(text == "XIAOWOKitchenBusiness"))
			{
				if (text == "XIAOWORatColony" && text2 == "RatMedicineCanTargetPatch")
				{
					return true;
				}
			}
			else if (text2 == "RottenDishCanTargetPatch")
			{
				return true;
			}
			break;
		case "GameItem.GetNegociatedValue":
			if (text == "XIAOWOKitchenBusiness")
			{
				if (text2 == "FreshnessPricePatch")
				{
					return true;
				}
				if (text2 == "FeaturedDishPricePatch")
				{
					return true;
				}
			}
			break;
		case "GameItem.MayTarget":
			if (!(text == "XIAOWOKitchenBusiness"))
			{
				if (text == "XIAOWORatColony" && text2 == "RatMedicineMayTargetPatch")
				{
					return true;
				}
			}
			else if (text2 == "RottenDishMayTargetPatch")
			{
				return true;
			}
			break;
		case "GameItem.Target":
			if (!(text == "XIAOWOKitchenBusiness"))
			{
				if (text == "XIAOWORatColony" && text2 == "RatMedicineTargetPatch")
				{
					return true;
				}
			}
			else if (text2 == "KitchenDishTargetPatch")
			{
				return true;
			}
			break;
		case "StartingPerk.GetLocalizedDescription":
			if (!(text == "ExtraPerks") || !(text2 == "PerkDescriptionPatch"))
			{
				break;
			}
			return true;
		case "StartingPerk.GetLocalizedDisplayName":
			if (!(text == "ExtraPerks") || !(text2 == "PerkDisplayNamePatch"))
			{
				break;
			}
			return true;
		case "StoreClientManager.TrackClient":
			if (!(text == "ExtraPerks") || !(text2 == "BoardInsiderTrackClientPatch"))
			{
				break;
			}
			return true;
		}
		return false;
	}

	private static string ModuleFor(Type type)
	{
		string name = type.Name;
		if (name.StartsWith("OwnerDeal", StringComparison.Ordinal))
		{
			return "owner";
		}
		if (name.StartsWith("AugHunt", StringComparison.Ordinal))
		{
			return "aug-hunt";
		}
		if (name.StartsWith("BayExpansion", StringComparison.Ordinal))
		{
			return "bay-expansion";
		}
		if (name.StartsWith("Fridge", StringComparison.Ordinal))
		{
			return "fridge-expansion";
		}
		if (name.StartsWith("LoadingProgress", StringComparison.Ordinal))
		{
			return "loading";
		}
		if (name.StartsWith("OperatingSurcharge", StringComparison.Ordinal))
		{
			return "operating-costs";
		}
		bool flag = name.StartsWith("Cashflow", StringComparison.Ordinal);
		if (!flag)
		{
			bool flag2;
			switch (name)
			{
			case "DailyClientGenerationPatch":
			case "NewGameCashflowPatch":
			case "LoadGameCashflowPatch":
				flag2 = true;
				break;
			default:
				flag2 = false;
				break;
			}
			flag = flag2;
		}
		if (flag)
		{
			return "calendar";
		}
		flag = name.StartsWith("NightScavenging", StringComparison.Ordinal);
		if (!flag)
		{
			bool flag2 = ((name == "MinorWoundChancePatch" || name == "MajorWoundChancePatch") ? true : false);
			flag = flag2;
		}
		if (flag)
		{
			return "scavenging";
		}
		flag = name.StartsWith("StableSupply", StringComparison.Ordinal);
		if (!flag)
		{
			bool flag2 = ((name == "PlayerSaleContextPatch" || name == "NegotiatedValuePatch") ? true : false);
			flag = flag2;
		}
		if (flag)
		{
			return "trade-pricing";
		}
		if (name.StartsWith("PrecisionBay", StringComparison.Ordinal))
		{
			return "precision-bay";
		}
		switch (name)
		{
		case "PerkDescriptionPatch":
		case "PerkElementIconPatch":
		case "PerkPickerPatch":
		case "FreeBuildLimitPatch":
		case "PerkSelectedOrderPatch":
		case "PerkDeselectedOrderPatch":
		case "StartingPerkRegistrationPatch":
		case "PerkNamePatch":
		case "IconInjectionPatch":
			flag = true;
			break;
		default:
			flag = false;
			break;
		}
		if (flag)
		{
			return "perk-picker";
		}
		return "trade-effects";
	}
}

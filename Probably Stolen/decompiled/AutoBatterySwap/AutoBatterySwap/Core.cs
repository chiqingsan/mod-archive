using System;
using System.Reflection;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;

namespace AutoBatterySwap;

public class Core : MelonMod
{
	public override void OnInitializeMelon()
	{
		((MelonBase)this).LoggerInstance.Msg("AutoBatterySwap loaded.");
	}

	public override void OnLateInitializeMelon()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Expected O, but got Unknown
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Expected O, but got Unknown
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Expected O, but got Unknown
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Expected O, but got Unknown
		try
		{
			Harmony val = new Harmony("AutoBatterySwap");
			int num = 0;
			MethodInfo methodInfo = AccessTools.Method(typeof(PowerHelper), "HandlePower", new Type[1] { typeof(bool) }, (Type[])null);
			if (methodInfo != null)
			{
				val.Patch((MethodBase)methodInfo, new HarmonyMethod(typeof(Hooks), "BeforeHandlePower", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
				num++;
			}
			MethodInfo methodInfo2 = AccessTools.Method(typeof(PlayerStore), "HandlePower", Type.EmptyTypes, (Type[])null);
			if (methodInfo2 != null)
			{
				val.Patch((MethodBase)methodInfo2, new HarmonyMethod(typeof(Hooks), "BeforePlayerHandlePower", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
				num++;
			}
			MethodInfo methodInfo3 = AccessTools.Method(typeof(MachineHelper), "TryDrawCyclePower", new Type[2]
			{
				typeof(GameItem),
				typeof(GameSlotInventory)
			}, (Type[])null);
			if (methodInfo3 != null)
			{
				val.Patch((MethodBase)methodInfo3, new HarmonyMethod(typeof(Hooks), "BeforeTryDrawCyclePower", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
				num++;
			}
			MethodInfo methodInfo4 = AccessTools.Method(typeof(MachineHelper), "CanPower", new Type[2]
			{
				typeof(GameItem),
				typeof(GameSlotInventory)
			}, (Type[])null);
			if (methodInfo4 != null)
			{
				val.Patch((MethodBase)methodInfo4, new HarmonyMethod(typeof(Hooks), "BeforeCanPower", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
				num++;
			}
			MethodInfo methodInfo5 = AccessTools.Method(typeof(PowerHelper), "DrawPowerSource", new Type[2]
			{
				typeof(GameItem),
				typeof(int)
			}, (Type[])null);
			if (methodInfo5 != null)
			{
				val.Patch((MethodBase)methodInfo5, new HarmonyMethod(typeof(Hooks), "BeforeDrawPowerSource", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
				num++;
			}
			MethodInfo methodInfo6 = AccessTools.Method(typeof(PowerHelper), "CanDrawPowerSource", new Type[2]
			{
				typeof(GameItem),
				typeof(int)
			}, (Type[])null);
			if (methodInfo6 != null)
			{
				val.Patch((MethodBase)methodInfo6, new HarmonyMethod(typeof(Hooks), "BeforeCanDrawPowerSource", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
				num++;
			}
			MethodInfo methodInfo7 = AccessTools.Method(typeof(PowerHelper), "TryRemoveEnergy", new Type[3]
			{
				typeof(GameItem),
				typeof(int),
				typeof(bool)
			}, (Type[])null);
			if (methodInfo7 != null)
			{
				val.Patch((MethodBase)methodInfo7, new HarmonyMethod(typeof(Hooks), "BeforeTryRemoveEnergy", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
				num++;
			}
			((MelonBase)this).LoggerInstance.Msg($"patches: {num}");
		}
		catch (Exception value)
		{
			((MelonBase)this).LoggerInstance.Error($"patch failed: {value}");
		}
	}
}

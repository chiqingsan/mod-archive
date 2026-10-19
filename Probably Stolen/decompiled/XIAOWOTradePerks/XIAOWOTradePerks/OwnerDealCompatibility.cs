using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Il2Cpp;
using Il2CppSystem;
using MelonLoader;

namespace XIAOWOTradePerks;

internal static class OwnerDealCompatibility
{
	private static MethodInfo _cashBuyoutConditionTarget;

	private static MethodInfo _landlordStockTarget;

	internal static MethodInfo CashBuyoutConditionTarget => _cashBuyoutConditionTarget;

	internal static MethodInfo LandlordStockTarget => _landlordStockTarget;

	internal static bool UsesCustomCashBuyoutChoice => _cashBuyoutConditionTarget == null;

	internal static Dialogue AddChoice(Dialogue dialogue, GameItem choiceItem, string name, string description, Dialogue nextDialogue, Action action)
	{
		if (dialogue == null)
		{
			return null;
		}
		MethodInfo methodInfo = (from candidate in typeof(Dialogue).GetMethods(BindingFlags.Instance | BindingFlags.Public)
			where string.Equals(candidate.Name, "AddChoice", StringComparison.Ordinal)
			orderby candidate.GetParameters().Length
			select candidate).FirstOrDefault(delegate(MethodInfo candidate)
		{
			int num = candidate.GetParameters().Length;
			return (uint)(num - 5) <= 1u;
		});
		if (methodInfo == null)
		{
			throw new MissingMethodException(typeof(Dialogue).FullName, "AddChoice");
		}
		object[] parameters = ((methodInfo.GetParameters().Length != 5) ? new object[6] { choiceItem, name, description, nextDialogue, action, null } : new object[5] { choiceItem, name, description, nextDialogue, action });
		object? obj = methodInfo.Invoke(dialogue, parameters);
		return (Dialogue)((obj is Dialogue) ? obj : null);
	}

	internal static void Install(Harmony harmony)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected O, but got Unknown
		_cashBuyoutConditionTarget = typeof(PlayerStore).GetMethod("CanBuyOutStore", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (_cashBuyoutConditionTarget != null)
		{
			harmony.Patch((MethodBase)_cashBuyoutConditionTarget, (HarmonyMethod)null, new HarmonyMethod(AccessTools.Method(typeof(OwnerDealBuyoutChoicePatch), "Postfix", (Type[])null, (Type[])null)), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
			Instance log = Core.Log;
			if (log != null)
			{
				log.Msg("Owner deal cash-buyout condition patch: " + _cashBuyoutConditionTarget.Name);
			}
		}
		else
		{
			Instance log2 = Core.Log;
			if (log2 != null)
			{
				log2.Msg("Owner deal: native CanBuyOutStore is absent; using the current property-purchase flow.");
			}
		}
		_landlordStockTarget = (from method in typeof(__c).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
			where method.Name.StartsWith("_LandlordWholesale_b__", StringComparison.Ordinal) && method.ReturnType == typeof(void) && method.GetParameters().Length == 0
			select method).OrderBy<MethodInfo, string>((MethodInfo method) => method.Name, StringComparer.Ordinal).FirstOrDefault();
		if (_landlordStockTarget == null)
		{
			Instance log3 = Core.Log;
			if (log3 != null)
			{
				log3.Warning("Owner deal: landlord wholesale stock callback was not found; native stock will be used.");
			}
			return;
		}
		harmony.Patch((MethodBase)_landlordStockTarget, new HarmonyMethod(AccessTools.Method(typeof(OwnerDealStockPatch), "Prefix", (Type[])null, (Type[])null)), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
		Instance log4 = Core.Log;
		if (log4 != null)
		{
			log4.Msg("Owner deal landlord stock patch: " + _landlordStockTarget.Name);
		}
	}
}

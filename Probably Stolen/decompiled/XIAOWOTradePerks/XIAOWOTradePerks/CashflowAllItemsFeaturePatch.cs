using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(ItemFeature), "CreateAndAddFeatureFromEvent")]
internal static class CashflowAllItemsFeaturePatch
{
	private static bool Prefix(GameItem gameItem, StoreEvent storeEvent)
	{
		if (!DeterministicSchedule.IsCashflowEvent((storeEvent != null) ? storeEvent.identifier : null))
		{
			return true;
		}
		if (gameItem == null || storeEvent.negociationDatas == null || storeEvent.negociationDatas.Count == 0)
		{
			return false;
		}
		NegociationData val = storeEvent.negociationDatas[0];
		if (val == null || gameItem.IsAlreadyContainFeatureWithId(val.identifier))
		{
			return false;
		}
		ItemFeature val2 = ItemFeature.CreateFeatureFromNegociationData(val);
		if (val2 == null)
		{
			return false;
		}
		string cashflowFeatureDisplay = DeterministicSchedule.GetCashflowFeatureDisplay(storeEvent.identifier);
		val2.SetPublicDisplay(cashflowFeatureDisplay);
		val2.SetActualDisplay(cashflowFeatureDisplay);
		gameItem.AddItemFeature(val2);
		return false;
	}
}

namespace KillSystem;

public abstract class RewardOrderService
{
	public abstract void CreateOrder();

	protected abstract bool IsOverdue(RewardOrderModel orderModel);

	protected abstract bool AddRewardOrder(RewardOrderModel orderModel);

	protected abstract bool RemoveRewardOrder(RewardOrderModel orderModel);

	public abstract bool AcceptRewardOrder(RewardOrderModel orderModel);

	public abstract bool GiveUpRewardOrder();

	public abstract bool CompleteRewardOrder();

	public abstract bool GiveOrderReward();

	public abstract bool HasAcceptOrder();

	public abstract bool IsRewarded(int npcId);

	public abstract void UpdateRewardOrderList();

	public abstract void UpdateAcceptedRewardOrder();

	public abstract int GetOrderTaskState();
}

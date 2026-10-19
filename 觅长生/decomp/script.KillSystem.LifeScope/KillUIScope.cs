using KillSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace script.KillSystem.LifeScope;

public class KillUIScope : LifetimeScope
{
	public static KillUIScope Inst;

	protected override void Configure(IContainerBuilder builder)
	{
		Inst = this;
		ContainerBuilderUnityExtensions.RegisterComponent<KillView>(builder, ((Component)this).GetComponent<KillView>());
		ContainerBuilderUnityExtensions.RegisterComponent<RewardOrderView>(builder, ((Component)((Component)this).transform.Find("悬赏列表/悬赏令")).GetComponent<RewardOrderView>());
		ContainerBuilderUnityExtensions.RegisterEntryPoint<KillUILogic>(builder, (Lifetime)1);
	}

	protected override void OnDestroy()
	{
		((LifetimeScope)this).OnDestroy();
		Inst = null;
	}
}

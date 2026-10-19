using Fungus;
using UnityEngine;

[CommandInfo("YSTools", "设置场景背景图片", "设置场景背景图片", 0)]
[AddComponentMenu("")]
public class CmdSetBG : Command
{
	[Tooltip("背景名称")]
	[SerializeField]
	protected string BGName = "1";

	[Tooltip("是否记入存档")]
	[SerializeField]
	protected bool IsSave;

	[Tooltip("是否是当前场景，为true时，SceneName将不起作用")]
	[SerializeField]
	protected bool IsNowScene = true;

	[Tooltip("场景名称")]
	[SerializeField]
	protected string SceneName = "SceneName";

	public override void OnEnter()
	{
		string text = SceneName;
		if (IsNowScene)
		{
			text = SceneEx.NowSceneName;
		}
		BackGroundImage backGroundImage = Object.FindObjectOfType<BackGroundImage>();
		if (text == SceneEx.NowSceneName)
		{
			if ((Object)(object)backGroundImage != (Object)null)
			{
				backGroundImage.SetBG(BGName, IsSave);
			}
			else
			{
				Debug.LogError((object)("尝试给场景" + text + "改变场景图为" + BGName + "，但是此场景没有BackGroundImage组件"));
			}
		}
		else if (IsSave)
		{
			BackGroundImage.SaveBG(text, BGName);
		}
		Continue();
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using BepInEx;
using GUIPackage;
using HarmonyLib;
using JSONClass;
using KBEngine;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SkySwordKill.Next.DialogEvent;
using SkySwordKill.Next.DialogSystem;
using SkySwordKill.Next.DialogTrigger;
using Tab;
using TuPo;
using UniqueCream.MCSExtraTools.Interfaces;
using UniqueCream.MCSExtraTools.MoreTools;
using UniqueCream.MCSExtraTools.RecreateSceneOverAnimation;
using UniqueCream.MCSExtraTools.RecreateSceneOverAnimation.Interface;
using UniqueCream.MCSExtraTools.RecreateSceneOverAnimation.NextSupport;
using UniqueCream.MCSExtraTools.TabExtraSetting;
using UniqueCream.MCSExtraTools.TabExtraSetting.Interface;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.DisableOptimizations | DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints | DebuggableAttribute.DebuggingModes.EnableEditAndContinue)]
[assembly: AssemblyTitle("DataSaveHelper")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("HP")]
[assembly: AssemblyProduct("DataSaveHelper")]
[assembly: AssemblyCopyright("Copyright © HP 2022")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: Guid("e333ad59-cd8e-467a-8d1f-a35345dedbb2")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyVersion("1.0.0.0")]
namespace UniqueCream.MCSExtraTools
{
	/// <summary>
	/// 自动化加载Ab包
	/// </summary>
	public class AbPackageManager
	{
		private readonly Dictionary<string, AssetBundle> _assetBundleDict = new Dictionary<string, AssetBundle>();

		private readonly string _abPackagePath;

		/// <summary>
		/// 创建并初始化Ab包管理器
		/// </summary>
		/// <param name="path">加载Ab包的基础路径</param>
		public AbPackageManager(string path)
		{
			_abPackagePath = path;
		}

		/// <summary>
		/// 获取Ab包
		/// </summary>
		/// <param name="path">以基础路径为基础的路径</param>
		/// <returns>返回的Ab包</returns>
		public AssetBundle GetAssetBundle(string path)
		{
			if (!_assetBundleDict.ContainsKey(path))
			{
				LoadAbPackage(path);
			}
			return _assetBundleDict[path];
		}

		private void LoadAbPackage(string path)
		{
			if (_assetBundleDict.ContainsKey(path))
			{
				return;
			}
			if (!File.Exists(_abPackagePath + path))
			{
				Main.LogError("AB包路径不存在：" + _abPackagePath + path);
				return;
			}
			try
			{
				AssetBundle val = AssetBundle.LoadFromFile(_abPackagePath + path);
				if ((Object)(object)val != (Object)null)
				{
					_assetBundleDict.Add(path, val);
				}
			}
			catch (Exception arg)
			{
				Main.LogError($"加载AB包失败：{path}，错误信息：\n{arg}");
			}
		}
	}
	[BepInPlugin("UniqueCream.MCSExtraTools", "MCSExtraTools", "1.3.6")]
	internal class Main : BaseUnityPlugin
	{
		public const string ModVersion = "1.3.6";

		public const string ModName = "MCSExtraTools";

		public static Main Instance;

		public static string RootFilePath;

		public AbPackageManager AbPackageManager;

		private void Start()
		{
			Init();
		}

		private void Awake()
		{
			Instance = this;
			RootFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
			InitCores();
		}

		private void Init()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			new Harmony("UniqueCream.MCSExtraTools").PatchAll();
			AbPackageManager = new AbPackageManager(RootFilePath + "Assets\\AbPackage\\");
			InitTools.Init();
			LogInfo("觅长生更多工具加载成功！By神奇丶奶油");
		}

		private void InitCores()
		{
			string[] files = Directory.GetFiles(RootFilePath + "\\Core", "*.dll");
			string[] array = files;
			foreach (string text in array)
			{
				LogInfo("加载核心文件:" + text);
				Assembly.LoadFile(text);
			}
		}

		public static void LogInfo(object obj)
		{
			((BaseUnityPlugin)Instance).Logger.LogInfo(obj);
		}

		public static void LogError(object obj)
		{
			((BaseUnityPlugin)Instance).Logger.LogError(obj);
		}

		public static void LogWarning(object obj)
		{
			((BaseUnityPlugin)Instance).Logger.LogWarning(obj);
		}

		public static void PreLoadLogError(object obj)
		{
			((BaseUnityPlugin)Instance).Logger.LogError((object)$"{obj}");
			PreloadManager.IsException = true;
			PreloadManager.ExceptionData += $"{obj}\n";
		}
	}
}
namespace UniqueCream.MCSExtraTools.TabExtraSetting
{
	public class SingleChoiceSetting : MonoBehaviour
	{
		public Text NameText;

		public Text NowStateText;

		public Dropdown Dropdown;

		public GameObject Template;

		public List<string> ChoiceList;

		public void Init()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			Transform obj = ((Component)this).transform.Find("Name");
			RectTransform val = (RectTransform)(object)((obj is RectTransform) ? obj : null);
			val.pivot = new Vector2(0.5f, 0.5f);
			NameText = ((Component)val).GetComponent<Text>();
			Dropdown = ((Component)((Component)this).transform.Find("Dropdown")).GetComponent<Dropdown>();
			NowStateText = ((Component)((Component)this).transform.Find("Dropdown/Label")).GetComponent<Text>();
			Dropdown.ClearOptions();
		}

		public void SetChoices(List<string> choiceList)
		{
			ChoiceList = choiceList;
			Dropdown.AddOptions(choiceList);
		}

		public void SetNowChoice(string nowChoice)
		{
			if (ChoiceList.Contains(nowChoice))
			{
				Dropdown.value = ChoiceList.FindIndex((string x) => x == nowChoice);
			}
			else
			{
				Main.LogWarning("设置了列表中不存在的值！" + nowChoice);
			}
		}

		public void SetName(string newName)
		{
			((Object)((Component)this).gameObject).name = newName;
			NameText.text = newName;
		}

		public string GetNowChoice()
		{
			return ChoiceList[Dropdown.value];
		}
	}
	public static class TabExtraSettingManager
	{
		public static Dictionary<string, ITabExtraSetting> TabExtraSettingDic = new Dictionary<string, ITabExtraSetting>();

		public static void Init()
		{
			InitTools.AutoFindAttributeAndType(typeof(TabExtraSettingAttribute), typeof(ITabExtraSetting), "初始化生成Tab更多选项出错！", delegate(Attribute attribute, Type type)
			{
				TabExtraSettingAttribute tabExtraSettingAttribute = attribute as TabExtraSettingAttribute;
				string settingName = tabExtraSettingAttribute.SettingName;
				Main.LogInfo("检测到Tab更多设置：" + settingName);
				if (TabExtraSettingDic.ContainsKey(settingName))
				{
					Main.LogError("已经存在相同名称的设置！" + settingName);
				}
				else
				{
					TabExtraSettingDic.Add(settingName, Activator.CreateInstance(type) as ITabExtraSetting);
				}
			});
		}
	}
}
namespace UniqueCream.MCSExtraTools.TabExtraSetting.Patch
{
	[HarmonyPatch]
	public class TabSetPanelSetting
	{
		private static readonly List<SingleChoiceSetting> SingleChoiceSettingList = new List<SingleChoiceSetting>();

		[HarmonyPostfix]
		[HarmonyPatch(typeof(TabSetPanel), "Init")]
		public static void TabSetPanel_Init_PostPatch()
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			Main.LogInfo("重新创建Tab设置界面");
			TabUIMag instance = SingletonMono<TabUIMag>.Instance;
			if ((Object)(object)instance == (Object)null)
			{
				return;
			}
			SingleChoiceSettingList.Clear();
			Transform val = ((Component)instance).transform.Find("TabSelect/Panel/系统/Select/Panel/设置/一键静音");
			val.localPosition = new Vector3(370f, 255f, 0f);
			Transform parent = val.parent;
			GameObject val2 = Object.Instantiate<GameObject>(Main.Instance.AbPackageManager.GetAssetBundle("VerticalScrollView").LoadAsset<GameObject>("VerticalScrollView"), parent);
			val2.transform.localPosition = new Vector3(15f, -37.2f);
			ScrollRect component = val2.GetComponent<ScrollRect>();
			component.movementType = (MovementType)2;
			RectTransform content = component.content;
			EditSingleChoice(parent.Find("分辨率"), (Transform)(object)content);
			EditSingleChoice(parent.Find("显示模式"), (Transform)(object)content);
			EditSingleChoice(parent.Find("存档间隔"), (Transform)(object)content);
			parent.Find("结算提示").SetParent(parent.Find("结算频率"));
			EditSingleChoice(parent.Find("结算频率"), (Transform)(object)content);
			Transform val3 = ((Transform)content).Find("显示模式");
			Transform val4 = Object.Instantiate<Transform>(val3, (Transform)(object)content);
			((Object)val4).name = "ChoicePrefab";
			((Component)val4.Find("Name")).GetComponent<Text>().text = "预制体";
			((Component)val4).gameObject.AddComponent<SingleChoiceSetting>().Init();
			((Component)val4).gameObject.SetActive(false);
			foreach (KeyValuePair<string, ITabExtraSetting> item in TabExtraSettingManager.TabExtraSettingDic)
			{
				SingleChoiceSetting component2 = ((Component)Object.Instantiate<Transform>(val4, (Transform)(object)content)).GetComponent<SingleChoiceSetting>();
				((Component)component2).gameObject.SetActive(true);
				component2.SetName(item.Key);
				component2.SetChoices(item.Value.GetAllChoices());
				component2.SetNowChoice(item.Value.GetNowChoice());
				SingleChoiceSettingList.Add(component2);
			}
			((MonoBehaviour)Main.Instance).StartCoroutine(ForceRebuild((RectTransform)(object)((parent is RectTransform) ? parent : null)));
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(TabSetPanel), "SaveConfig")]
		public static void TabSetPanel_SaveConfig()
		{
			foreach (SingleChoiceSetting singleChoiceSetting in SingleChoiceSettingList)
			{
				TabExtraSettingManager.TabExtraSettingDic[((Object)singleChoiceSetting).name].ConfirmChoice(singleChoiceSetting.GetNowChoice());
			}
		}

		public static IEnumerator ForceRebuild(RectTransform transform)
		{
			yield return (object)new WaitForSeconds(0.01f);
			LayoutRebuilder.ForceRebuildLayoutImmediate(transform);
		}

		public static void EditSingleChoice(Transform transform, Transform newParent)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			RectTransform val = (RectTransform)(object)((transform is RectTransform) ? transform : null);
			val.sizeDelta = new Vector2(900f, 80f);
			Transform val2 = transform.Find("Name");
			val2.localPosition = new Vector3(-200f, 0f);
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				Vector3 localPosition = child.localPosition;
				child.localPosition = new Vector3(localPosition.x, 0f);
			}
			transform.SetParent(newParent);
		}
	}
}
namespace UniqueCream.MCSExtraTools.TabExtraSetting.Interface
{
	[AttributeUsage(AttributeTargets.Class)]
	public class TabExtraSettingAttribute : Attribute
	{
		public string SettingName { get; set; }

		public TabExtraSettingAttribute(string settingName)
		{
			SettingName = settingName;
		}
	}
	public interface ITabExtraSetting
	{
		void ConfirmChoice(string choiceStr);

		List<string> GetAllChoices();

		string GetNowChoice();
	}
}
namespace UniqueCream.MCSExtraTools.RecreateSceneOverAnimation
{
	/// <summary>
	/// 自动反射初始化工具会调用此接口的方法来初始化动画设置
	/// </summary>
	public interface ISceneOverAnimationInitHelper
	{
		/// <summary>
		///  初始化动画方法，自动反射初始化工具会调用此方法
		///  可以自行继承SceneOverAnimationBase类来创建自己的动画逻辑
		/// </summary>
		/// <returns>返回的动画控制器</returns>
		ISceneOverAnimationControl InitAnimation();
	}
	public class SceneOverAnimationManager : MonoBehaviour
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class <>c
		{
			public static readonly <>c <>9 = new <>c();

			public static UnityAction <>9__18_0;

			public static UnityAction <>9__21_0;

			internal void <OnlyForToolsLoadMapScenes>b__18_0()
			{
				Main.LogInfo("触发Next战斗结束事件");
				Tools.instance.CanShowFightUI = 0;
				string str = DialogAnalysis.GetStr("Fight", "VictoryEvent");
				string str2 = DialogAnalysis.GetStr("Fight", "DefeatEvent");
				bool flag = DialogAnalysis.GetInt("Fight", "IsVictory") == 1;
				bool flag2 = flag && !string.IsNullOrEmpty(str);
				StartFight.ResetEventFight();
				if (flag2)
				{
					DialogAnalysis.StartDialogEvent(str, (DialogEnvironment)null);
				}
				else if (!flag && !string.IsNullOrEmpty(str2))
				{
					DialogAnalysis.StartDialogEvent(str2, (DialogEnvironment)null);
				}
			}

			internal void <LoadScene>b__21_0()
			{
			}
		}

		public static SceneOverAnimationManager Instance;

		public Dictionary<string, ISceneOverAnimationControl> SceneOverAnimationDic = new Dictionary<string, ISceneOverAnimationControl>();

		public GameObject RayCastBg;

		private string NextLoadSceneName;

		private string NextAniationName;

		private UnityAction NextLoadedAction;

		public bool IsLoading;

		public string NowAnimationName
		{
			get
			{
				string text = WholeGamingDataTools.GetData("NowSceneOverAnimationName")?.str;
				if (string.IsNullOrWhiteSpace(text))
				{
					NowAnimationName = "黑屏";
					return "黑屏";
				}
				return text;
			}
			set
			{
				WholeGamingDataTools.SetData("NowSceneOverAnimationName", JSONObject.CreateStringObject(value), saveImmediately: true);
			}
		}

		public float NowAnimationSpeed
		{
			get
			{
				JSONObject data = WholeGamingDataTools.GetData("NowSceneOverAnimationSpeed");
				float num;
				if (data == null)
				{
					num = 1f;
					NowAnimationSpeed = 1f;
				}
				else
				{
					num = data.f;
				}
				if (num <= 0f)
				{
					NowAnimationSpeed = 1f;
					return 1f;
				}
				return num;
			}
			set
			{
				WholeGamingDataTools.SetData("NowSceneOverAnimationSpeed", JSONObject.Create(value), saveImmediately: true);
			}
		}

		public void SetNowAnimation(string animationName)
		{
			if (animationName == "随机")
			{
				NowAnimationName = animationName;
				return;
			}
			if (SceneOverAnimationDic.ContainsKey(animationName))
			{
				NowAnimationName = animationName;
				return;
			}
			Main.LogError("不存在对应的过场动画：" + animationName + "，将使用默认过场动画！");
			NowAnimationName = "黑屏";
		}

		private void Awake()
		{
			Instance = this;
			RayCastBg = ((Component)((Component)this).transform.Find("RaycastBg")).gameObject;
			RayCastBg.SetActive(false);
			InitTools.AutoFindAttributeAndType(typeof(SceneOverAnimationAttribute), typeof(ISceneOverAnimationInitHelper), "初始化过度场景出错！", delegate(Attribute attribute, Type type)
			{
				SceneOverAnimationAttribute sceneOverAnimationAttribute = attribute as SceneOverAnimationAttribute;
				string animationName = sceneOverAnimationAttribute.AnimationName;
				Main.LogInfo("读取到过度场景：" + animationName);
				if (animationName == "随机")
				{
					throw new Exception("切换场景动画不能为随机！");
				}
				if (!SceneOverAnimationDic.ContainsKey(animationName))
				{
					ISceneOverAnimationInitHelper sceneOverAnimationInitHelper = Activator.CreateInstance(type) as ISceneOverAnimationInitHelper;
					ISceneOverAnimationControl sceneOverAnimationControl = sceneOverAnimationInitHelper.InitAnimation();
					sceneOverAnimationControl.HideSelf();
					SceneOverAnimationDic.Add(animationName, sceneOverAnimationControl);
				}
				else
				{
					Main.LogError("读取到重复名称的过度场景：" + animationName);
				}
			});
			SetUIBlackMask setUIBlackMask = new SetUIBlackMask();
			DialogAnalysis.RegisterCommand("SetUIBlackMask", (IDialogEvent)(object)setUIBlackMask);
			DialogAnalysis.RegisterCommand("设置黑屏", (IDialogEvent)(object)setUIBlackMask);
		}

		private string GetAnimationName()
		{
			string nowAnimationName = NowAnimationName;
			return (nowAnimationName == "随机") ? SceneOverAnimationDic.Keys.ToList()[Random.Range(0, SceneOverAnimationDic.Count)] : nowAnimationName;
		}

		private ISceneOverAnimationControl GetSceneOverAnimation(string animationName = null)
		{
			if (string.IsNullOrWhiteSpace(animationName))
			{
				animationName = GetAnimationName();
			}
			if (SceneOverAnimationDic.TryGetValue(animationName, out var value))
			{
				return value;
			}
			Main.LogWarning("未找到名为" + animationName + "的CrossFade，将使用默认的黑屏过度！");
			NowAnimationName = "黑屏";
			return SceneOverAnimationDic["黑屏"];
		}

		/// <summary>
		/// 异步加载场景的方法，需要提前将场景加载进游戏
		/// </summary>
		/// <param name="sceneName">场景名称</param>
		/// <param name="animationName">需要播放的动画的名称，默认是Null，即使用当前全局设置中的动画</param>
		/// <param name="action">场景加载完时触发的事件，会在场景加载完成后执行，执行完再触发动画结束</param>
		public void LoadSceneAsync(string sceneName, string animationName = null, UnityAction action = null)
		{
			ISceneOverAnimationControl sceneOverAnimation = GetSceneOverAnimation(animationName);
			if (IsLoading)
			{
				Main.LogWarning("加载场景未完成时尝试加载其他场景！" + sceneName);
				Main.LogWarning("将会在场景加载完成后加载！");
				NextLoadSceneName = sceneName;
				NextAniationName = animationName;
				NextLoadedAction = action;
			}
			else
			{
				((MonoBehaviour)this).StartCoroutine(LoadScene(sceneName, sceneOverAnimation, action));
			}
		}

		internal static void OnlyForToolsLoadMapScenes(string sceneName, UnityAction action = null)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			Scene activeScene = SceneManager.GetActiveScene();
			if (((Scene)(ref activeScene)).name == "YSNewFight")
			{
				object obj = <>c.<>9__18_0;
				if (obj == null)
				{
					UnityAction val = delegate
					{
						Main.LogInfo("触发Next战斗结束事件");
						Tools.instance.CanShowFightUI = 0;
						string str = DialogAnalysis.GetStr("Fight", "VictoryEvent");
						string str2 = DialogAnalysis.GetStr("Fight", "DefeatEvent");
						bool flag = DialogAnalysis.GetInt("Fight", "IsVictory") == 1;
						bool flag2 = flag && !string.IsNullOrEmpty(str);
						StartFight.ResetEventFight();
						if (flag2)
						{
							DialogAnalysis.StartDialogEvent(str, (DialogEnvironment)null);
						}
						else if (!flag && !string.IsNullOrEmpty(str2))
						{
							DialogAnalysis.StartDialogEvent(str2, (DialogEnvironment)null);
						}
					};
					<>c.<>9__18_0 = val;
					obj = (object)val;
				}
				UnityAction val2 = (UnityAction)obj;
				action = (UnityAction)((action != null) ? ((object)(UnityAction)Delegate.Combine((Delegate)(object)action, (Delegate)(object)val2)) : ((object)val2));
				Instance.LoadSceneAsync(sceneName, null, action);
			}
			else
			{
				Instance.LoadSceneAsync(sceneName, null, action);
			}
		}

		internal void OnlyForToolsLoadOtherScene(string sceneName, UnityAction action = null)
		{
			Main.LogInfo("加载其他场景：" + sceneName);
			((MonoBehaviour)this).StartCoroutine(LoadSceneOnlyForToolsLoadOtherScene(sceneName, action));
		}

		private IEnumerator LoadSceneOnlyForToolsLoadOtherScene(string sceneName, UnityAction action = null)
		{
			if (IsLoading)
			{
				Main.LogWarning("加载场景未完成时尝试加载其他场景！");
			}
			ISceneOverAnimationControl sceneOverAnimationControl = GetSceneOverAnimation();
			IsLoading = true;
			RayCastBg.SetActive(true);
			sceneOverAnimationControl.StartAnimation(NowAnimationSpeed);
			float speed = NowAnimationSpeed;
			float startUseTime = sceneOverAnimationControl.StartUseTime / speed;
			startUseTime = ((startUseTime < 0.1f) ? 0.1f : startUseTime);
			yield return (object)new WaitForSeconds(startUseTime);
			SceneManager.LoadScene(sceneName);
			while (true)
			{
				Scene activeScene = SceneManager.GetActiveScene();
				if (!(((Scene)(ref activeScene)).name != Tools.instance.ohtherSceneName))
				{
					break;
				}
				yield return null;
			}
			if (action != null)
			{
				action.Invoke();
			}
			sceneOverAnimationControl.EndAnimation();
			float endUseTime2 = sceneOverAnimationControl.EndUseTime / speed;
			endUseTime2 = ((endUseTime2 < 0.1f) ? 0.1f : endUseTime2);
			yield return (object)new WaitForSeconds(endUseTime2);
			RayCastBg.SetActive(false);
			IsLoading = false;
			sceneOverAnimationControl.HideSelf();
			Main.LogInfo("其他场景加载完成");
		}

		private IEnumerator LoadScene(string sceneName, ISceneOverAnimationControl sceneOverAnimationControl, UnityAction action = null)
		{
			Main.LogInfo("加载场景：" + sceneName);
			if ((Object)(object)PanelMamager.inst.UIBlackMaskGameObject != (Object)null)
			{
				Object.Destroy((Object)(object)PanelMamager.inst.UIBlackMaskGameObject);
			}
			IsLoading = true;
			Tools.instance.CanOpenTab = false;
			RayCastBg.SetActive(true);
			float speed = NowAnimationSpeed;
			sceneOverAnimationControl.StartAnimation(speed);
			yield return (object)new WaitForSeconds(sceneOverAnimationControl.StartUseTime / speed);
			AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
			while (!async.isDone)
			{
				yield return null;
			}
			try
			{
				if (action != null)
				{
					action.Invoke();
				}
			}
			catch (Exception ex)
			{
				Main.LogError("场景加载完成时触发事件出错！" + ex);
			}
			if (!string.IsNullOrWhiteSpace(NextLoadSceneName))
			{
				if (NextLoadedAction == null)
				{
					SceneOverAnimationManager sceneOverAnimationManager = this;
					object obj = <>c.<>9__21_0;
					if (obj == null)
					{
						UnityAction val = delegate
						{
						};
						<>c.<>9__21_0 = val;
						obj = (object)val;
					}
					sceneOverAnimationManager.NextLoadedAction = (UnityAction)obj;
				}
				NextLoadedAction = (UnityAction)Delegate.Combine((Delegate)(object)NextLoadedAction, (Delegate)(UnityAction)delegate
				{
					ToolsLoadMapSceneDoneEvent(sceneName);
					((MonoBehaviour)this).StartCoroutine(EndAnimationAction(sceneOverAnimationControl, speed));
				});
				LoadSceneAsyncWithoutAnimation(NextLoadSceneName, NextLoadedAction);
				NextLoadSceneName = null;
				NextAniationName = null;
				NextLoadedAction = null;
			}
			else
			{
				ToolsLoadMapSceneDoneEvent(sceneName);
				((MonoBehaviour)this).StartCoroutine(EndAnimationAction(sceneOverAnimationControl, speed));
			}
		}

		private IEnumerator EndAnimationAction(ISceneOverAnimationControl sceneOverAnimationControl, float speed)
		{
			sceneOverAnimationControl.EndAnimation();
			yield return (object)new WaitForSeconds(sceneOverAnimationControl.EndUseTime / speed);
			sceneOverAnimationControl.HideSelf();
			RayCastBg.SetActive(false);
			IsLoading = false;
			Main.LogInfo("场景加载完成");
		}

		public void LoadSceneAsyncWithoutAnimation(string sceneName, UnityAction action = null)
		{
			((MonoBehaviour)this).StartCoroutine(LoadSceneWithoutAnimation(sceneName, action));
		}

		private IEnumerator LoadSceneWithoutAnimation(string sceneName, UnityAction action = null)
		{
			IsLoading = true;
			AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
			while (!async.isDone)
			{
				yield return null;
			}
			if (action != null)
			{
				action.Invoke();
			}
			IsLoading = false;
			Main.LogInfo("场景加载完成");
		}

		public void ToolsLoadPreparation(string sceneName, bool lastSceneIsValue = true)
		{
			if (sceneName != "LianDan" && lastSceneIsValue)
			{
				Tools.instance.getPlayer().lastScence = sceneName;
			}
			Tools.instance.loadSceneType = 1;
			Tools.jumpToName = sceneName;
		}

		public void ToolsLoadMapSceneDoneEvent(string sceneName)
		{
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Expected O, but got Unknown
			if ((Object)(object)PanelMamager.inst.UISceneGameObject == (Object)null)
			{
				SceneManager.LoadScene("UIScene", (LoadSceneMode)1);
			}
			if ((Object)(object)PanelMamager.inst.UISceneGameObject != (Object)null)
			{
				Transform val = PanelMamager.inst.UISceneGameObject.transform.Find("ThreeSceneNpcCanvas");
				if ((Object)(object)val != (Object)null)
				{
					((Component)val).gameObject.SetActive(true);
				}
			}
			if ((Object)(object)UI_Manager.inst != (Object)null)
			{
				if ((Object)(object)ThreeSceernUIFab.inst != (Object)null)
				{
					Object.Destroy((Object)(object)((Component)ThreeSceernUIFab.inst).gameObject);
				}
				if ((Object)(object)ThreeSceneMagFab.inst != (Object)null)
				{
					Object.Destroy((Object)(object)((Component)ThreeSceneMagFab.inst).gameObject);
				}
			}
			Tools.instance.isNeedSetTalk = true;
			Tools.instance.CanOpenTab = true;
			Main.LogInfo("场景加载完成，触发Next进入场景事件");
			DialogEnvironment val2 = new DialogEnvironment
			{
				mapScene = sceneName
			};
			DialogAnalysis.TryTrigger((IEnumerable<string>)new string[2] { "进入场景", "EnterMapScene" }, val2, true);
		}

		/// <summary>
		/// 展示场景过度动画
		/// </summary>
		/// <param name="animationName">动画名称</param>
		/// <param name="speed">动画播放速度</param>
		/// <param name="action">动画播放中间执行的事件</param>
		/// <returns></returns>
		public IEnumerator ShowSceneOverAnimation(string animationName, float speed, UnityAction action = null)
		{
			RayCastBg.SetActive(true);
			ISceneOverAnimationControl sceneOverAnimationControl = GetSceneOverAnimation(animationName);
			float startUseTime = sceneOverAnimationControl.StartUseTime / speed;
			startUseTime = ((startUseTime < 0.1f) ? 0.1f : startUseTime);
			sceneOverAnimationControl.StartAnimation(speed);
			yield return (object)new WaitForSeconds(startUseTime);
			sceneOverAnimationControl.EndAnimation();
			float endUseTime2 = sceneOverAnimationControl.EndUseTime / speed;
			endUseTime2 = ((endUseTime2 < 0.1f) ? 0.1f : endUseTime2);
			if (action != null)
			{
				action.Invoke();
			}
			yield return (object)new WaitForSeconds(endUseTime2);
			sceneOverAnimationControl.HideSelf();
			RayCastBg.SetActive(false);
		}
	}
	[TabExtraSetting("场景过度动画")]
	public class SceneOverAnimationTabSetting : ITabExtraSetting
	{
		public void ConfirmChoice(string choiceStr)
		{
			SceneOverAnimationManager.Instance.SetNowAnimation(choiceStr);
		}

		public List<string> GetAllChoices()
		{
			List<string> list = new List<string> { "随机" };
			list.AddRange(SceneOverAnimationManager.Instance.SceneOverAnimationDic.Keys.ToList());
			return list;
		}

		public string GetNowChoice()
		{
			return SceneOverAnimationManager.Instance.NowAnimationName;
		}
	}
	[TabExtraSetting("过度动画倍速")]
	public class SceneOverSpeedTabSetting : ITabExtraSetting
	{
		public void ConfirmChoice(string choiceStr)
		{
			SceneOverAnimationManager.Instance.NowAnimationSpeed = float.Parse(choiceStr);
		}

		public List<string> GetAllChoices()
		{
			return new List<string>
			{
				"0.1", "0.2", "0.5", "1", "2", "4", "8", "16", "32", "64",
				"128", "256", "512", "1024"
			};
		}

		public string GetNowChoice()
		{
			return SceneOverAnimationManager.Instance.NowAnimationSpeed.ToString();
		}
	}
}
namespace UniqueCream.MCSExtraTools.RecreateSceneOverAnimation.SceneOverAnimations
{
	[SceneOverAnimation("黑屏")]
	public class CrossFadeBlack : ISceneOverAnimationInitHelper
	{
		public ISceneOverAnimationControl InitAnimation()
		{
			GameObject val = Main.Instance.AbPackageManager.GetAssetBundle("SceneOverAnimation/黑屏").LoadAsset<GameObject>("黑屏");
			SceneOverAnimationBase sceneOverAnimationBase = Object.Instantiate<GameObject>(val, ((Component)SceneOverAnimationManager.Instance).transform).AddComponent<SceneOverAnimationBase>();
			sceneOverAnimationBase.StartUseTime = 0.5f;
			sceneOverAnimationBase.EndUseTime = 0.5f;
			sceneOverAnimationBase.EndTrigger = Animator.StringToHash("End");
			return sceneOverAnimationBase;
		}
	}
}
namespace UniqueCream.MCSExtraTools.RecreateSceneOverAnimation.Patch
{
	[HarmonyPatch]
	public class NextFix
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(OnLoadMapScene), "Postfix")]
		public static bool OnLoadMapScene_Prefix()
		{
			return false;
		}
	}
	[HarmonyPatch]
	internal class TuPoResultFix
	{
		private static bool flag;

		[HarmonyPrefix]
		[HarmonyPatch(typeof(BigTuPoResultIMag), "ShowSuccess")]
		private static void SuccessPrefix(BigTuPoResultIMag __instance)
		{
			flag = true;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(BigTuPoResultIMag), "ShowFail")]
		private static void FailPrefix(BigTuPoResultIMag __instance)
		{
			flag = true;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(BigTuPoResultIMag), "Close")]
		private static bool Prefix2(BigTuPoResultIMag __instance)
		{
			if (flag)
			{
				flag = false;
				return true;
			}
			return false;
		}
	}
	[HarmonyPatch]
	internal class ToolsCanClick
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(Tools), "canClick")]
		private static void Tools_canClick_Postfix(ref bool __result)
		{
			if (SceneOverAnimationManager.Instance.IsLoading)
			{
				__result = false;
			}
		}
	}
	[HarmonyPatch]
	internal class ToolsLoadOtherScene
	{
		private static readonly List<string> EnableSceneNameList = new List<string> { "YSNewFight", "YSNewJieDanFight", "YSNewYuanYingFight", "YSNewZhuJi", "YSNewHuaShen", "YSNewDuanTiFight", "YSNewTianJieFight", "LunDao", "PaiMai" };

		[HarmonyPrefix]
		[HarmonyPatch(typeof(Tools), "loadOtherScenes")]
		public static bool ToolsLoadOtherScenePatch(Tools __instance, string name)
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Expected O, but got Unknown
			if (!EnableSceneNameList.Contains(name))
			{
				return true;
			}
			__instance.loadSceneType = 0;
			__instance.ohtherSceneName = name;
			__instance.CanOpenTab = false;
			if ((Object)(object)SingletonMono<TabUIMag>.Instance != (Object)null)
			{
				SingletonMono<TabUIMag>.Instance.TryEscClose();
			}
			if (name == "YSNewFight")
			{
				SceneOverAnimationManager.Instance.OnlyForToolsLoadOtherScene("NextScene", new UnityAction(OnFightStart.Trigger));
			}
			else
			{
				SceneOverAnimationManager.Instance.OnlyForToolsLoadOtherScene("NextScene");
			}
			return false;
		}
	}
	[HarmonyPatch]
	internal class UISceneFix
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(UISceneTotal), "Start")]
		public static void UISceneTotal_Start_Postfix(UISceneTotal __instance)
		{
			if (SceneOverAnimationManager.Instance.IsLoading)
			{
				((Component)((Component)__instance).transform.Find("CanvasFader")).gameObject.SetActive(false);
			}
			else
			{
				((Component)((Component)__instance).transform.Find("CanvasFader")).gameObject.SetActive(true);
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(UIBlackMask), "Awake")]
		public static void UIBlackMask_Awake_Postfix(UIBlackMask __instance)
		{
			if (SceneOverAnimationManager.Instance.IsLoading)
			{
				Object.Destroy((Object)(object)((Component)__instance).gameObject);
			}
		}
	}
	[HarmonyPatch]
	internal class ToolsLoadMapScene
	{
		[HarmonyTranspiler]
		[HarmonyPatch(typeof(Tools), "loadMapScenes")]
		private static IEnumerable<CodeInstruction> Tools_LoadMapScenesPatch(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo removeMethodInfo = AccessTools.Method(typeof(SceneManager), "LoadScene", new Type[1] { typeof(string) }, (Type[])null);
			MethodInfo newMethodInfo = AccessTools.Method(typeof(SceneOverAnimationManager), "OnlyForToolsLoadMapScenes", new Type[2]
			{
				typeof(string),
				typeof(UnityAction)
			}, (Type[])null);
			List<CodeInstruction> codeList = instructions.ToList();
			int count = 0;
			foreach (CodeInstruction code in codeList)
			{
				if (code.opcode == OpCodes.Call && (MethodInfo)code.operand == removeMethodInfo)
				{
					yield return new CodeInstruction(OpCodes.Ldnull, (object)null);
					yield return new CodeInstruction(OpCodes.Call, (object)newMethodInfo);
					yield return new CodeInstruction(OpCodes.Ret, (object)null);
					yield break;
				}
				yield return code;
				count++;
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(Tools), "loadMapScenes")]
		private static void LoadMapScenesPostfix()
		{
			Tools.instance.isNeedSetTalk = false;
			Tools.instance.CanOpenTab = false;
		}
	}
}
namespace UniqueCream.MCSExtraTools.RecreateSceneOverAnimation.NextSupport
{
	public class SetUIBlackMask : IDialogEvent
	{
		public void Execute(DialogCommand command, DialogEnvironment env, Action callback)
		{
			((MonoBehaviour)SceneOverAnimationManager.Instance).StartCoroutine(SceneOverAnimationManager.Instance.ShowSceneOverAnimation("黑屏", 4f));
			callback?.Invoke();
		}
	}
	[DialogEvent("ShowSceneOverAnimation")]
	[DialogEvent("显示场景过度动画")]
	public class ShowSceneOverAnimation : IDialogEvent
	{
		public void Execute(DialogCommand command, DialogEnvironment env, Action callback)
		{
			string str = command.GetStr(0, "");
			float @float = command.GetFloat(1, 1f);
			((MonoBehaviour)SceneOverAnimationManager.Instance).StartCoroutine(SceneOverAnimationManager.Instance.ShowSceneOverAnimation(str, @float));
			callback?.Invoke();
		}
	}
}
namespace UniqueCream.MCSExtraTools.RecreateSceneOverAnimation.NextSupport.Patch
{
	[HarmonyPatch]
	internal class PatchAfterBattleDialogPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(AfterBattleDialogPatch), "Prefix")]
		public static bool Prefix()
		{
			return false;
		}
	}
}
namespace UniqueCream.MCSExtraTools.RecreateSceneOverAnimation.Interface
{
	public interface ISceneOverAnimationControl
	{
		/// <summary>
		/// 动画开始需要的时间
		/// </summary>
		float StartUseTime { get; set; }

		/// <summary>
		/// 动画结束需要的时间
		/// </summary>
		float EndUseTime { get; set; }

		/// <summary>
		/// 开始动画调用方法
		/// </summary>
		/// <param name="speed"></param>
		void StartAnimation(float speed);

		/// <summary>
		/// 结束动画
		/// </summary>
		void EndAnimation();

		/// <summary>
		/// 隐藏自身
		/// </summary>
		void HideSelf();
	}
	public class RandomPictureControl : SceneOverAnimationBase
	{
		private static readonly List<Sprite> Sprites = new List<Sprite>();

		private static bool _isInit;

		private static bool _hasSprite;

		private Image image;

		public override void Awake()
		{
			base.Awake();
			image = ((Component)((Component)this).transform.Find("图片")).GetComponent<Image>();
			InitSpriteList();
		}

		private static void InitSpriteList()
		{
			if (_isInit)
			{
				return;
			}
			_isInit = true;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			List<DirectoryInfo> list = new List<DirectoryInfo>();
			list.AddRange(WorkshopTool.GetAllModDirectory());
			list.AddRange(new DirectoryInfo(Paths.GameRootPath + "\\本地mod测试").GetDirectories());
			Sprites.Clear();
			int num = 0;
			List<byte[]> list2 = new List<byte[]>();
			foreach (DirectoryInfo item in list)
			{
				DirectoryInfo dir = new DirectoryInfo(item.FullName + "\\plugins\\MCSExtraTools\\随机图片");
				List<byte[]> collection = LoadBytes(dir);
				list2.AddRange(collection);
			}
			foreach (byte[] item2 in list2)
			{
				LoadPic(item2);
			}
			_hasSprite = Sprites.Count > 0;
			stopwatch.Stop();
			Main.LogInfo("读取到随机图片数量：" + Sprites.Count + "，耗时：" + stopwatch.ElapsedMilliseconds / 1000 + "s");
		}

		private static void LoadPic(byte[] bytes)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected O, but got Unknown
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			Texture2D val = new Texture2D(1, 1, (TextureFormat)5, false)
			{
				hideFlags = (HideFlags)61
			};
			ImageConversion.LoadImage(val, bytes);
			Sprite item = Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), Vector2.zero);
			Sprites.Add(item);
		}

		private static List<byte[]> LoadBytes(DirectoryInfo dir)
		{
			List<byte[]> list = new List<byte[]>();
			if (!dir.Exists)
			{
				return list;
			}
			List<FileInfo> list2 = dir.GetFiles("*.png").ToList();
			list2.AddRange(dir.GetFiles("*.jpg"));
			list2.AddRange(dir.GetFiles("*.jpeg"));
			foreach (FileInfo item2 in list2)
			{
				byte[] item = File.ReadAllBytes(item2.FullName);
				list.Add(item);
			}
			return list;
		}

		public override void StartAnimation(float speed)
		{
			if (_hasSprite)
			{
				image.sprite = Sprites[Random.Range(0, Sprites.Count)];
			}
			base.StartAnimation(speed);
		}
	}
	public class SceneOverAnimationBase : MonoBehaviour, ISceneOverAnimationControl
	{
		public Animator TransitionAnimation;

		public int EndTrigger;

		public float StartUseTime { get; set; }

		public float EndUseTime { get; set; }

		public virtual void Awake()
		{
			TransitionAnimation = ((Component)this).GetComponent<Animator>();
		}

		/// <summary>
		/// 动画开始时调用的方法，默认是将自身SetActive(false)然后SetActive(true)来触发动画
		/// </summary>
		/// <param name="speed">设置当前动画播放的速度（受全局设置调控）</param>
		public virtual void StartAnimation(float speed)
		{
			TransitionAnimation.speed = speed;
			((Component)this).gameObject.SetActive(false);
			((Component)this).gameObject.SetActive(true);
		}

		/// <summary>
		/// 动画结束时调用的方法，可以在动画结束时修改EndUseTime来自行调整动画结束时等待的时间，在等待时间结束后会调用HideSelf方法
		/// </summary>
		public virtual void EndAnimation()
		{
			TransitionAnimation.SetTrigger(EndTrigger);
		}

		public virtual void HideSelf()
		{
			((Component)this).gameObject.SetActive(false);
		}
	}
	[AttributeUsage(AttributeTargets.Class)]
	public class SceneOverAnimationAttribute : Attribute
	{
		public string AnimationName { get; set; }

		public SceneOverAnimationAttribute(string animationName)
		{
			AnimationName = animationName;
		}
	}
}
namespace UniqueCream.MCSExtraTools.Patch
{
	[HarmonyPatch]
	public class FirstEnterGame
	{
		private static bool isInited;

		[HarmonyPostfix]
		[HarmonyPatch(typeof(MainUIMag), "Awake")]
		private static void MainUIMag_PostPatch()
		{
			if (!isInited)
			{
				isInited = true;
				DataTools.Instance.RegisterAutoSaveEvent();
				GameObject val = Object.Instantiate<GameObject>(Main.Instance.AbPackageManager.GetAssetBundle("sceneoveranimationcanvas").LoadAsset<GameObject>("SceneOverAnimationCanvas"));
				Object.DontDestroyOnLoad((Object)(object)val);
				val.AddComponent<SceneOverAnimationManager>();
				TabExtraSettingManager.Init();
				WholeGamingDataTools.Init();
			}
		}
	}
	[HarmonyPatch]
	public class LoadGame
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(LoadPlayerDateFactory), "initChuanYingFu")]
		public static void LoadSave_Postfix()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			DataTools.Instance.LoadSaveData();
			stopwatch.Stop();
			Main.LogInfo($"读档耗时：{stopwatch.ElapsedMilliseconds}ms");
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(LoadPlayerDateFactory), "SaveJieSuanData")]
		public static void SaveGame_Postfix()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			DataTools.Instance.SaveData();
			stopwatch.Stop();
			Main.LogInfo($"存档耗时：{stopwatch.ElapsedMilliseconds}ms");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(MainUIMag), "Awake")]
		public static void MainUIMag_Awake_PostPatch()
		{
			DataTools.Instance.ResetData();
		}
	}
}
namespace UniqueCream.MCSExtraTools.MoreTools
{
	/// <summary>
	/// 游戏中存档的工具
	/// </summary>
	public class DataTools
	{
		private static DataTools instance;

		internal Dictionary<string, JObject> AllDataSave = new Dictionary<string, JObject>();

		private List<AutoSaveEvent> autoSaveEventList = new List<AutoSaveEvent>();

		/// <summary>
		/// 获取DataTools的实例
		/// </summary>
		public static DataTools Instance => instance ?? (instance = new DataTools());

		/// <summary>
		/// 重置所有存档数据
		/// </summary>
		public void ResetData()
		{
			AllDataSave = new Dictionary<string, JObject>();
		}

		/// <summary>
		/// 保存Mod存档数据，会在游戏存档时统一存档到存档文件夹；可以使用自动存档工具以实现在存档之前触发一次保存，将实例数据保存
		/// </summary>
		/// <param name="modName">Mod名称</param>
		/// <param name="dataName">Data名称</param>
		/// <param name="data">Data实例</param>
		/// <typeparam name="T">Data类型</typeparam>
		public void Save<T>(string modName, string dataName, T data)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			if (!AllDataSave.ContainsKey(modName))
			{
				Main.LogInfo("生成" + modName + "的新存档");
				AllDataSave.Add(modName, new JObject());
			}
			if (!AllDataSave[modName].ContainsKey(dataName))
			{
				AllDataSave[modName].Add(dataName, JToken.FromObject((object)data));
			}
			else
			{
				AllDataSave[modName][dataName] = JToken.FromObject((object)data);
			}
		}

		/// <summary>
		/// 加载Mod存档数据，如果没有存档数据则返回默认值
		/// </summary>
		/// <param name="modName">Mod名称</param>
		/// <param name="dataName">Data名称</param>
		/// <param name="defaultValue">默认值</param>
		/// <typeparam name="T">Data类型</typeparam>
		/// <returns>存档中保存的数据，如果没有则返回默认值</returns>
		public T Load<T>(string modName, string dataName, T defaultValue = default(T))
		{
			if (!AllDataSave.ContainsKey(modName))
			{
				return defaultValue;
			}
			if (!AllDataSave[modName].ContainsKey(dataName))
			{
				return defaultValue;
			}
			return AllDataSave[modName].GetValue(dataName).ToObject<T>();
		}

		/// <summary>
		/// 判断是否包含某个存档
		/// </summary>
		/// <param name="modName">Mod名称</param>
		/// <param name="dataName">Data名称</param>
		/// <returns>如果包含返回True，否则返回False</returns>
		public bool HasSave(string modName, string dataName)
		{
			if (!AllDataSave.ContainsKey(modName))
			{
				return false;
			}
			if (!AllDataSave[modName].ContainsKey(dataName))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// 移除某个存档
		/// </summary>
		/// <param name="modName">Mod名称</param>
		/// <param name="dataName">Data名称</param>
		public void RemoveSave(string modName, string dataName)
		{
			if (AllDataSave.ContainsKey(modName) && AllDataSave[modName].ContainsKey(dataName))
			{
				AllDataSave[modName].Remove(dataName);
			}
		}

		[HandleProcessCorruptedStateExceptions]
		internal void LoadSaveData()
		{
			Main.LogInfo("DataSaveTools加开始加载存档");
			string path = Paths.GetNewSavePath() + "/" + YSNewSaveSystem.NowAvatarPathPre + "/MCSDataSaveHelper.json";
			DirectoryInfo directoryInfo = new DirectoryInfo(Paths.GetNewSavePath() + "/" + YSNewSaveSystem.NowAvatarPathPre + "/MCSDataSaveHelper/");
			AllDataSave.Clear();
			if (directoryInfo.Exists)
			{
				FileInfo[] files = directoryInfo.GetFiles();
				foreach (FileInfo fileInfo in files)
				{
					try
					{
						string text = File.ReadAllText(fileInfo.FullName);
						JObject value = JObject.Parse(text);
						AllDataSave.Add(fileInfo.Name, value);
					}
					catch (Exception obj)
					{
						Main.LogError(obj);
						Main.LogError("加载" + fileInfo.Name + "的存档信息失败！");
					}
				}
			}
			else if (File.Exists(path))
			{
				try
				{
					JObject val = JObject.Parse(File.ReadAllText(path));
					AllDataSave = ((JToken)val).ToObject<Dictionary<string, JObject>>();
					Main.LogInfo("DataSaveTools加载旧版存档信息成功！");
				}
				catch (Exception arg)
				{
					Main.LogError($"DataSaveTools加载旧版存档信息失败！：\n{arg}");
				}
			}
			else
			{
				Main.LogInfo("DataSaveTools没有找到存档信息！，已生成新存档信息！");
				AllDataSave = new Dictionary<string, JObject>();
			}
			RunLoadEvent();
		}

		internal void SaveData()
		{
			Main.LogInfo("开始保存");
			RunSaveEvent();
			string text = Paths.GetNewSavePath() + "/" + YSNewSaveSystem.NowAvatarPathPre + "/MCSDataSaveHelper/";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			else
			{
				FileInfo[] files = new DirectoryInfo(text).GetFiles();
				foreach (FileInfo fileInfo in files)
				{
					fileInfo.Delete();
				}
			}
			foreach (KeyValuePair<string, JObject> item in AllDataSave)
			{
				try
				{
					string path = text + item.Key;
					string value = JsonConvert.SerializeObject((object)item.Value);
					using (StreamWriter streamWriter = new StreamWriter(path))
					{
						streamWriter.Write(value);
					}
					Main.LogInfo("保存" + item.Key + "的存档信息成功！");
				}
				catch (Exception arg)
				{
					Main.LogError($"保存{item.Key}的存档信息失败！：\n{arg}");
					throw;
				}
			}
			Main.LogInfo("DataSave保存完毕！");
			WholeGamingDataTools.SaveAll();
		}

		internal void RegisterAutoSaveEvent()
		{
			InitTools.AutoFindType(typeof(AutoSaveEvent), "初始化自动存档事件出错", delegate(Type type)
			{
				autoSaveEventList.Add((AutoSaveEvent)Activator.CreateInstance(type));
			});
			Main.LogInfo($"注册自动存档事件完毕，共计注册了{autoSaveEventList.Count}个自动存档事件");
		}

		private void RunSaveEvent()
		{
			foreach (AutoSaveEvent autoSaveEvent in autoSaveEventList)
			{
				try
				{
					autoSaveEvent.OnSaveGame();
				}
				catch (Exception obj)
				{
					Main.LogError(obj);
				}
			}
		}

		internal void RunLoadEvent()
		{
			foreach (AutoSaveEvent autoSaveEvent in autoSaveEventList)
			{
				try
				{
					autoSaveEvent.OnLoadSave();
				}
				catch (Exception obj)
				{
					Main.LogError(obj);
				}
			}
			foreach (AutoSaveEvent autoSaveEvent2 in autoSaveEventList)
			{
				try
				{
					autoSaveEvent2.OnEndLoadSave();
				}
				catch (Exception obj2)
				{
					Main.LogError(obj2);
				}
			}
		}
	}
	/// <summary>
	/// 自动初始化的工具
	/// </summary>
	public static class InitTools
	{
		/// <summary>
		/// 启用的mod的路径
		/// </summary>
		public static List<DirectoryInfo> EnabledModsDirPaths { get; private set; }

		/// <summary>
		/// 根据基础类型自动反射初始化，会自动遍历所有程序集
		/// </summary>
		/// <param name="basicAttribute">类型需要拥有的标签</param>
		/// <param name="basic">类型继承的基类</param>
		/// <param name="errorInfo">出现错误时的报错</param>
		/// <param name="callBack">获取到符合条件的类时执行的事件</param>
		public static void AutoFindAttributeAndType(Type basicAttribute, Type basic, string errorInfo, Action<Attribute, Type> callBack)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			foreach (Assembly assembly in array)
			{
				try
				{
					Type[] types = assembly.GetTypes();
					Type[] array2 = types;
					foreach (Type type in array2)
					{
						if (!basic.IsAssignableFrom(type))
						{
							continue;
						}
						foreach (Attribute customAttribute in type.GetCustomAttributes())
						{
							if (customAttribute == null || !(customAttribute.GetType() != basicAttribute))
							{
								callBack(customAttribute, type);
							}
						}
					}
				}
				catch (Exception ex)
				{
					Main.PreLoadLogError(errorInfo);
					Main.PreLoadLogError("自动反射初始化出现问题，目标程序集信息：" + assembly.FullName + "，路径：" + assembly.Location + "，错误日志：\n" + ex.Message);
					if (ex is ReflectionTypeLoadException { LoaderExceptions: var loaderExceptions })
					{
						Exception[] array3 = loaderExceptions;
						foreach (Exception ex3 in array3)
						{
							Main.PreLoadLogError(ex3.Message);
						}
					}
				}
			}
		}

		/// <summary>
		/// 根据基础类型自动反射初始化，会自动遍历所有程序集
		/// </summary>
		/// <param name="basic">类型继承的基类</param>
		/// <param name="errorInfo">出现错误时的报错</param>
		/// <param name="callBack">获取到符合条件的类时执行的事件</param>
		public static void AutoFindType(Type basic, string errorInfo, Action<Type> callBack)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			foreach (Assembly assembly in array)
			{
				try
				{
					Type[] types = assembly.GetTypes();
					Type[] array2 = types;
					foreach (Type type in array2)
					{
						if (basic.IsAssignableFrom(type))
						{
							callBack(type);
						}
					}
				}
				catch (Exception ex)
				{
					Main.PreLoadLogError(errorInfo);
					Main.PreLoadLogError("自动反射初始化出现问题，目标程序集信息：" + assembly.FullName + "，路径：" + assembly.Location + "，错误日志：\n" + ex.Message);
					if (ex is ReflectionTypeLoadException { LoaderExceptions: var loaderExceptions })
					{
						Exception[] array3 = loaderExceptions;
						foreach (Exception ex3 in array3)
						{
							Main.PreLoadLogError(ex3.Message);
						}
					}
				}
			}
		}

		internal static void Init()
		{
			EnabledModsDirPaths = new List<DirectoryInfo>();
			List<DirectoryInfo> list = new List<DirectoryInfo>();
			list.AddRange(WorkshopTool.GetAllModDirectory());
			foreach (DirectoryInfo item in list)
			{
				if (!WorkshopTool.CheckModIsDisable(item.Name))
				{
					EnabledModsDirPaths.Add(item);
				}
			}
			EnabledModsDirPaths.AddRange(new DirectoryInfo(Paths.GameRootPath + "\\本地mod测试").GetDirectories());
		}
	}
	/// <summary>
	/// 神通拓展工具
	/// </summary>
	public static class SkillTools
	{
		/// <summary>
		/// 移除玩家的一个神通
		/// </summary>
		/// <param name="player">当前玩家实例</param>
		/// <param name="skillId">神通的SkillId</param>
		public static void RemoveSkill(this Avatar player, int skillId)
		{
			using (List<SkillItem>.Enumerator enumerator = player.hasSkillList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((ITEM_INFO)enumerator.Current).itemId == skillId)
					{
						player.hasSkillList.Remove(enumerator.Current);
						break;
					}
				}
			}
			player.UnEquipSkill(skillId);
		}

		/// <summary>
		/// 获取玩家是否拥有某个神通
		/// </summary>
		/// <param name="player">当前玩家实例</param>
		/// <param name="skillId">神通SkillId</param>
		/// <returns></returns>
		public static bool HasSkill(this Avatar player, int skillId)
		{
			foreach (SkillItem hasSkill in player.hasSkillList)
			{
				if (((ITEM_INFO)hasSkill).itemId == skillId)
				{
					return true;
				}
			}
			return false;
		}
	}
	/// <summary>
	/// 功法拓展工具
	/// </summary>
	public static class StaticSkillTools
	{
		/// <summary>
		/// 获取玩家是否拥有某个功法
		/// </summary>
		/// <param name="player">当前玩家</param>
		/// <param name="skillId">功法的SkillId</param>
		public static void RemoveStaticSkill(this Avatar player, int skillId)
		{
			using (List<SkillItem>.Enumerator enumerator = player.hasStaticSkillList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((ITEM_INFO)enumerator.Current).itemId == skillId)
					{
						player.hasStaticSkillList.Remove(enumerator.Current);
						break;
					}
				}
			}
			player.UnEquipStaticSkill(skillId);
		}

		/// <summary>
		/// 获取玩家某个功法的等级
		/// </summary>
		/// <param name="player">当前玩家</param>
		/// <param name="skillId">功法的SkillId</param>
		/// <returns></returns>
		public static int GetStaticSkillLevel(this Avatar player, int skillId)
		{
			using (List<SkillItem>.Enumerator enumerator = player.hasStaticSkillList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((ITEM_INFO)enumerator.Current).itemId == skillId)
					{
						return enumerator.Current.level;
					}
				}
			}
			return 0;
		}

		/// <summary>
		/// 获取玩家是否习得某个功法
		/// </summary>
		/// <param name="player">当前玩家</param>
		/// <param name="skillId">功法的SkillId</param>
		/// <returns></returns>
		public static bool HasStaticSkill(this Avatar player, int skillId)
		{
			foreach (SkillItem hasStaticSkill in player.hasStaticSkillList)
			{
				if (((ITEM_INFO)hasStaticSkill).itemId == skillId)
				{
					return true;
				}
			}
			return false;
		}
	}
	/// <summary>
	/// 天赋相关工具
	/// </summary>
	public static class TianFuTools
	{
		/// <summary>
		/// 增加指定天赋buff
		/// </summary>
		/// <param name="player">当前玩家实例</param>
		/// <param name="buffId">要增加的Buff的Id</param>
		/// <param name="Num">增加的Buff的数量</param>
		public static void AddTianFuBuff(this Avatar player, int buffId, int Num = 1)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			string text = 16.ToString();
			if (!player.TianFuID.HasField(text))
			{
				player.TianFuID.AddField(text, new JSONObject((Type)3));
			}
			for (int i = 0; i < Num; i++)
			{
				player.TianFuID[text].Add(buffId);
			}
		}

		/// <summary>
		/// 移除指定天赋buff
		/// </summary>
		/// <param name="player">当前玩家</param>
		/// <param name="buffId">需要被移除的BuffId</param>
		/// <param name="Num">移除的数量</param>
		/// <param name="RemoveAll">是否全部移除</param>
		public static void RemoveTianFuBuff(this Avatar player, int buffId, int Num = 1, bool RemoveAll = false)
		{
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Expected O, but got Unknown
			string text = 16.ToString();
			if (!player.TianFuID.HasField(text))
			{
				return;
			}
			List<int> list = player.TianFuID[text].ToList();
			if (RemoveAll)
			{
				list.RemoveAll((int x) => x == buffId);
			}
			else
			{
				for (int i = 0; i < Num; i++)
				{
					if (list.Contains(buffId))
					{
						list.Remove(buffId);
					}
				}
			}
			player.TianFuID[text] = new JSONObject(((IEnumerable<int>)list).Select((Func<int, JSONObject>)((int buff) => new JSONObject(buff))).ToArray());
		}

		/// <summary>
		/// 增加指定天赋Id
		/// </summary>
		/// <param name="player">当前玩家实例</param>
		/// <param name="tianFuId">增加的天赋Id</param>
		public static void AddTianFu(this Avatar player, int tianFuId)
		{
			player.SelectTianFuID.Add(tianFuId);
		}

		/// <summary>
		/// 移除指定天赋Id
		/// </summary>
		/// <param name="player">当前玩家实例</param>
		/// <param name="tianFuId">移除的天赋Id</param>
		public static void RemoveTianFu(this Avatar player, int tianFuId)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			JSONObject val = new JSONObject((Type)4);
			foreach (int item in player.SelectTianFuID.ToList())
			{
				if (item != tianFuId)
				{
					val.Add(item);
				}
			}
			player.SelectTianFuID = val;
		}

		/// <summary>
		/// 是否拥有指定天赋
		/// </summary>
		/// <param name="player">当前玩家实例</param>
		/// <param name="tianFuId">需要被判断的天赋Id</param>
		/// <returns>拥有则返回True，没有则返回False</returns>
		public static bool HasTianFu(this Avatar player, int tianFuId)
		{
			foreach (int item in player.SelectTianFuID.ToList())
			{
				if (item == tianFuId)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 获取指定天赋Buff层数
		/// </summary>
		/// <param name="player">当前玩家实例</param>
		/// <param name="tianFuBuffId">天赋Buff的Id</param>
		/// <returns>玩家拥有对应Id的天赋Buff的层数</returns>
		public static int GetTianFuBuffNum(this Avatar player, int tianFuBuffId)
		{
			int num = 0;
			if (!player.TianFuID.HasField("16"))
			{
				return 0;
			}
			foreach (int item in player.TianFuID["16"].ToList())
			{
				if (item == tianFuBuffId)
				{
					num++;
				}
			}
			return num;
		}
	}
	/// <summary>
	/// 游戏全局数据工具，可以用来保存一些游戏全局数据，保存在对应游戏版本存档文件夹下；如果保存时选择立即保存则会立即保存，否则会在游戏存档时存档
	/// </summary>
	public static class WholeGamingDataTools
	{
		/// <summary>
		/// 存档信息
		/// </summary>
		private static Dictionary<string, JSONObject> JsonObjectDataSaveDic = new Dictionary<string, JSONObject>();

		private static string _savePath;

		internal static void Init()
		{
			_savePath = Paths.GetNewSavePath() + "\\WholeGamingData\\";
			if (!Directory.Exists(_savePath))
			{
				Directory.CreateDirectory(_savePath);
			}
			JsonObjectDataSaveDic.Clear();
			LoadAll();
		}

		/// <summary>
		/// 设置全局数据
		/// </summary>
		/// <param name="key">全局数据的Key</param>
		/// <param name="data">全局数据的Value</param>
		/// <param name="saveImmediately">是否立即保存</param>
		public static void SetData(string key, JSONObject data, bool saveImmediately = false)
		{
			JsonObjectDataSaveDic[key] = data;
			if (saveImmediately)
			{
				Save(key, ((object)data).ToString());
			}
		}

		/// <summary>
		/// 获取全局数据
		/// </summary>
		/// <param name="key">全局数据的Key</param>
		/// <param name="defaultValue">默认值</param>
		/// <returns>返回的JsonObject数据</returns>
		public static JSONObject GetData(string key, JSONObject defaultValue = null)
		{
			JSONObject value;
			return JsonObjectDataSaveDic.TryGetValue(key, out value) ? value : defaultValue;
		}

		internal static void SaveAll()
		{
			Main.LogInfo("开始保存所有全局信息");
			foreach (KeyValuePair<string, JSONObject> item in JsonObjectDataSaveDic)
			{
				Save(item.Key, ((object)item.Value).ToString());
			}
			Main.LogInfo("保存所有全局信息完毕");
		}

		private static void Save(string key, string data)
		{
			File.WriteAllText(_savePath + key, data);
		}

		private static void LoadAll()
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			FileInfo[] files = new DirectoryInfo(_savePath).GetFiles();
			FileInfo[] array = files;
			foreach (FileInfo fileInfo in array)
			{
				string name = fileInfo.Name;
				JSONObject value = new JSONObject(File.ReadAllText(fileInfo.FullName), -2, false, false);
				JsonObjectDataSaveDic[name] = value;
			}
		}
	}
	public static class WuDaoTools
	{
		public static void RemoveWuDao(this Avatar player, int wudaoId)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			foreach (int item in WuDaoJson.DataDict[wudaoId].Type)
			{
				JSONObject wuDaoStudy = player.wuDaoMag.getWuDaoStudy(item);
				if (!wuDaoStudy.HasItem(wudaoId))
				{
					continue;
				}
				JSONObject val = new JSONObject((Type)4);
				foreach (int item2 in wuDaoStudy.ToList())
				{
					if (item2 != wudaoId)
					{
						val.Add(item2);
					}
				}
				player.wuDaoMag.getWuDaoTypeJson(item).SetField("study", val);
			}
			WuDaoStaticSkill.resetWuDaoSeid(player);
		}
	}
}
namespace UniqueCream.MCSExtraTools.Interfaces
{
	/// <summary>
	/// 自动化存档事件，通过继承并重写这个方法来实现自动化存档
	/// </summary>
	public class AutoSaveEvent
	{
		/// <summary>
		/// 游戏保存存档时触发的事件，会自动在存档之前触发
		/// </summary>
		public virtual void OnLoadSave()
		{
		}

		public virtual void OnEndLoadSave()
		{
		}

		/// <summary>
		/// 游戏加载存档时触发的事件，会自动在存档之后触发
		/// </summary>
		public virtual void OnSaveGame()
		{
		}
	}
}
namespace UniqueCream.MCSExtraTools.ExcelSupport
{
	/// <summary>
	/// Excel表格处理工具
	/// </summary>
	public static class ExcelTools
	{
		private enum ExcelDataType
		{
			/// 整数
			Int,
			/// 浮点数
			Float,
			/// 字符串
			String,
			/// 布尔值
			Bool,
			/// 未知类型
			Unknown,
			Enum
		}

		private class PropertyDataInfo : DataInfo
		{
			public PropertyInfo PropertyInfo;

			public PropertyDataInfo(PropertyInfo propertyInfo)
			{
				PropertyInfo = propertyInfo;
				Type = propertyInfo.PropertyType;
			}
		}

		private class FieldDataInfo : DataInfo
		{
			public FieldInfo FieldInfo;

			public FieldDataInfo(FieldInfo fieldInfo)
			{
				FieldInfo = fieldInfo;
				Type = fieldInfo.FieldType;
			}
		}

		private class DataInfo
		{
			public Type Type;

			public ExcelDataType DataType;

			public bool IsList;
		}

		/// <summary>
		/// 加载一个Excel表格
		/// </summary>
		/// <param name="filePath">表格路径</param>
		/// <returns>表格</returns>
		/// <exception cref="T:System.Exception">表格文件类型错误</exception>
		public static IWorkbook LoadExcel(string filePath)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			try
			{
				using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				if (filePath.EndsWith(".xls"))
				{
					return (IWorkbook)new HSSFWorkbook((Stream)fileStream);
				}
				if (filePath.EndsWith(".xlsx"))
				{
					return (IWorkbook)new XSSFWorkbook((Stream)fileStream);
				}
				throw new Exception("错误的文件后缀！请加载正确的Excel表格！");
			}
			catch (Exception ex)
			{
				Main.LogError("加载Excel表格时出现异常！文件路径：" + filePath + "\n" + ex);
				throw;
			}
		}

		/// <summary>
		/// 根据表格获取所有反序列化
		/// </summary>
		/// <param name="sheet">对应的表格</param>
		/// <typeparam name="T1">数据类型</typeparam>
		/// <returns>反序列化结束的实例列表</returns>
		public static List<TClass> MapData<TClass>(ISheet sheet) where TClass : class
		{
			if (sheet == null)
			{
				return new List<TClass>();
			}
			sheet.ForceFormulaRecalculation = true;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			IRow row = sheet.GetRow(0);
			for (int i = 0; i < row.LastCellNum; i++)
			{
				ICell cell = row.GetCell(i);
				if (cell != null && !string.IsNullOrEmpty(cell.StringCellValue))
				{
					dictionary.Add(cell.StringCellValue, i);
				}
			}
			List<PropertyDataInfo> list = new List<PropertyDataInfo>();
			PropertyInfo[] properties = typeof(TClass).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo propertyInfo in properties)
			{
				string name = propertyInfo.Name;
				PropertyDataInfo propertyDataInfo = new PropertyDataInfo(propertyInfo);
				GetDataType(propertyDataInfo);
				if (propertyDataInfo.DataType != ExcelDataType.Unknown && dictionary.ContainsKey(name))
				{
					list.Add(propertyDataInfo);
				}
			}
			List<FieldDataInfo> list2 = new List<FieldDataInfo>();
			foreach (FieldInfo item in from x in typeof(TClass).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				where !x.Name.Contains("k__BackingField")
				select x)
			{
				string name2 = item.Name;
				FieldDataInfo fieldDataInfo = new FieldDataInfo(item);
				GetDataType(fieldDataInfo);
				if (fieldDataInfo.DataType != ExcelDataType.Unknown && dictionary.ContainsKey(name2))
				{
					list2.Add(fieldDataInfo);
				}
			}
			List<TClass> list3 = new List<TClass>();
			for (int k = 2; k <= sheet.LastRowNum; k++)
			{
				IRow row2 = sheet.GetRow(k);
				if (row2 == null || row2.Cells.Count == 0 || row2.Cells.All((ICell x) => (int)x.CellType == 3))
				{
					continue;
				}
				TClass val = Activator.CreateInstance(typeof(TClass)) as TClass;
				foreach (PropertyDataInfo item2 in list)
				{
					if (IsNullCell(row2.GetCell(dictionary[item2.PropertyInfo.Name]), out var text))
					{
						try
						{
							item2.PropertyInfo.SetValue(val, (!item2.IsList && item2.DataType == ExcelDataType.String) ? "" : GetDefaultValueByType(item2.PropertyInfo.PropertyType));
						}
						catch (Exception ex)
						{
							Main.LogError($"设置默认值时出现异常！当前属性：{item2.PropertyInfo.Name}，文本：{text}，数据类型：{item2.DataType}\n" + ex);
							throw;
						}
					}
					else
					{
						try
						{
							item2.PropertyInfo.SetValue(val, GetValueByDataType(text, item2));
						}
						catch (Exception ex2)
						{
							Main.LogError($"设置值时出现异常！当前属性：{item2.PropertyInfo.Name}，文本：{text}，数据类型：{item2.DataType}\n" + ex2);
							throw;
						}
					}
				}
				foreach (FieldDataInfo item3 in list2)
				{
					if (item3.DataType == ExcelDataType.Unknown || !dictionary.ContainsKey(item3.FieldInfo.Name))
					{
						continue;
					}
					if (IsNullCell(row2.GetCell(dictionary[item3.FieldInfo.Name]), out var text2))
					{
						try
						{
							item3.FieldInfo.SetValue(val, (!item3.IsList && item3.DataType == ExcelDataType.String) ? "" : GetDefaultValueByType(item3.FieldInfo.FieldType));
						}
						catch (Exception ex3)
						{
							Main.LogError($"设置默认值时出现异常！当前字段：{item3.FieldInfo.Name}，文本：{text2}，数据类型：{item3.DataType}\n" + ex3);
							throw;
						}
					}
					else
					{
						try
						{
							item3.FieldInfo.SetValue(val, GetValueByDataType(text2, item3));
						}
						catch (Exception ex4)
						{
							Main.LogError($"设置值时出现异常！当前字段：{item3.FieldInfo.Name}，文本：{text2}，数据类型：{item3.DataType}\n" + ex4);
							throw;
						}
					}
				}
				list3.Add(val);
			}
			return list3;
		}

		private static bool IsNullCell(ICell cell, out string text)
		{
			if (cell == null)
			{
				text = null;
				return true;
			}
			text = ((object)cell).ToString();
			return string.IsNullOrEmpty(text);
		}

		private static object GetValueByDataType(string text, DataInfo dataInfoInfo)
		{
			if (dataInfoInfo.IsList)
			{
				switch (dataInfoInfo.DataType)
				{
				case ExcelDataType.Int:
					return (from x in text.Split(',')
						where !string.IsNullOrEmpty(x)
						select x).Select(int.Parse).ToList();
				case ExcelDataType.Float:
					return (from x in text.Split(',')
						where !string.IsNullOrEmpty(x)
						select x).Select(float.Parse).ToList();
				case ExcelDataType.String:
					return (from x in text.Split(',')
						where !string.IsNullOrEmpty(x)
						select x).ToList();
				case ExcelDataType.Bool:
					return (from x in text.Split(',')
						where !string.IsNullOrEmpty(x)
						select x).Select(bool.Parse).ToList();
				case ExcelDataType.Enum:
				{
					IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(dataInfoInfo.Type));
					foreach (string item in from x in text.Split(',')
						where !string.IsNullOrEmpty(x)
						select x)
					{
						object value = Enum.Parse(dataInfoInfo.Type, item);
						list.Add(value);
					}
					return list;
				}
				default:
					throw new Exception($"未知的数据类型：{dataInfoInfo.DataType}");
				}
			}
			return dataInfoInfo.DataType switch
			{
				ExcelDataType.Int => int.Parse(text), 
				ExcelDataType.Float => float.Parse(text), 
				ExcelDataType.String => text, 
				ExcelDataType.Bool => bool.Parse(text), 
				ExcelDataType.Enum => Enum.Parse(dataInfoInfo.Type, text), 
				_ => throw new Exception($"未知的数据类型：{dataInfoInfo.DataType}"), 
			};
		}

		private static TObjectType ToObject<TObjectType>(object obj)
		{
			return (TObjectType)obj;
		}

		private static object GetDefaultValueByType(Type type)
		{
			return Activator.CreateInstance(type);
		}

		private static void GetDataType(DataInfo dataInfo)
		{
			Type type = dataInfo.Type;
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
			{
				dataInfo.IsList = true;
				Type type2 = type.GetGenericArguments()[0];
				Type type3 = type2;
				Type type4 = type3;
				if ((object)type4 != null)
				{
					if (type4 == typeof(int))
					{
						dataInfo.DataType = ExcelDataType.Int;
						return;
					}
					Type type5 = type4;
					if (type5 == typeof(float))
					{
						dataInfo.DataType = ExcelDataType.Float;
						return;
					}
					Type type6 = type4;
					if (type6 == typeof(string))
					{
						dataInfo.DataType = ExcelDataType.String;
						return;
					}
					Type type7 = type4;
					if (type7 == typeof(bool))
					{
						dataInfo.DataType = ExcelDataType.Bool;
						return;
					}
					Type type8 = type4;
					if (type8.IsEnum)
					{
						dataInfo.DataType = ExcelDataType.Enum;
						dataInfo.Type = type2;
						return;
					}
				}
				dataInfo.DataType = ExcelDataType.Unknown;
				return;
			}
			dataInfo.IsList = false;
			Type type9 = type;
			Type type10 = type9;
			if ((object)type10 != null)
			{
				if (type10 == typeof(int))
				{
					dataInfo.DataType = ExcelDataType.Int;
					return;
				}
				Type type11 = type10;
				if (type11 == typeof(float))
				{
					dataInfo.DataType = ExcelDataType.Float;
					return;
				}
				Type type12 = type10;
				if (type12 == typeof(string))
				{
					dataInfo.DataType = ExcelDataType.String;
					return;
				}
				Type type13 = type10;
				if (type13 == typeof(bool))
				{
					dataInfo.DataType = ExcelDataType.Bool;
					return;
				}
				Type type14 = type10;
				if (type14.IsEnum)
				{
					dataInfo.DataType = ExcelDataType.Enum;
					return;
				}
			}
			dataInfo.DataType = ExcelDataType.Unknown;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Modding;
using Duckov.Options.UI;
using Duckov.Utilities;
using SodaCraft.Localizations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

namespace ModConfig;

public class ModBehaviour : ModBehaviour
{
	public static int VERSION = 1;

	private static bool createdModTab = false;

	private static OptionsPanel_TabButton? modTabButton = null;

	private static GameObject? modContent = null;

	private static GameObject? dropdownListPrefab;

	private static GameObject? inputWithSliderPrefab;

	private static Queue<Action> pendingConfigActions = new Queue<Action>();

	private static void AddConfig(Action configAction)
	{
		if ((Object)(object)modTabButton != (Object)null && (Object)(object)modContent != (Object)null)
		{
			configAction();
			return;
		}
		pendingConfigActions.Enqueue(configAction);
		Debug.Log((object)$"配置项已加入队列，等待mod菜单初始化。当前队列长度: {pendingConfigActions.Count}");
	}

	private void ProcessPendingConfigs()
	{
		if ((Object)(object)modTabButton == (Object)null || (Object)(object)modContent == (Object)null)
		{
			Debug.Log((object)"modTabButton 或 modContent 尚未初始化，暂不处理配置项");
		}
		else
		{
			if (pendingConfigActions.Count <= 0)
			{
				return;
			}
			Debug.Log((object)$"开始处理等待的配置项，剩余数量: {pendingConfigActions.Count}");
			while (pendingConfigActions.Count > 0)
			{
				Action action = pendingConfigActions.Dequeue();
				try
				{
					action();
					Debug.Log((object)"配置项执行成功");
				}
				catch (Exception ex)
				{
					Debug.LogError((object)("执行配置项时出错: " + ex.Message + "\n" + ex.StackTrace));
				}
			}
		}
	}

	public static void AddDropdownList(string modName, string key, string description, SortedDictionary<string, object> options, Type valueType, object defaultValue)
	{
		string description2 = description;
		string key2 = key;
		SortedDictionary<string, object> options2 = options;
		Type valueType2 = valueType;
		object defaultValue2 = defaultValue;
		string modName2 = modName;
		AddConfig(delegate
		{
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Expected O, but got Unknown
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)dropdownListPrefab == (Object)null)
			{
				Debug.LogError((object)"dropdownListPrefab 为 null，无法创建下拉列表");
			}
			else
			{
				if (!((Object)(object)modContent == (Object)null))
				{
					try
					{
						GameObject val = Object.Instantiate<GameObject>(dropdownListPrefab);
						val.SetActive(true);
						OptionsUIEntry_Dropdown component = val.GetComponent<OptionsUIEntry_Dropdown>();
						if ((Object)(object)component == (Object)null)
						{
							Debug.LogError((object)"无法获取 OptionsUIEntry_Dropdown 组件");
							Object.Destroy((Object)(object)val);
						}
						else
						{
							TextMeshProUGUI instanceFieldValue = ReflectionHelper.GetInstanceFieldValue<TextMeshProUGUI>(component, "label");
							if ((Object)(object)instanceFieldValue != (Object)null)
							{
								((TMP_Text)instanceFieldValue).SetText(description2, true);
							}
							DropDownOptionsProvider dropDownOptionsProvider = ComponentHolderProtocol.AddComponent<DropDownOptionsProvider>((Object)(object)component);
							dropDownOptionsProvider.init(key2, description2, options2, valueType2, defaultValue2);
							ReflectionHelper.SetInstanceFieldValue(component, "provider", dropDownOptionsProvider);
							Transform val2 = modContent.transform.Find(modName2);
							if ((Object)(object)val2 == (Object)null)
							{
								GameObject val3 = Object.Instantiate<GameObject>(dropdownListPrefab, modContent.transform);
								val3.SetActive(false);
								((Object)val3).name = modName2;
								TrasnformExtensions.DestroyAllChildren(val3.transform);
								Object.Destroy((Object)(object)val3.GetComponent<OptionsUIEntry_Dropdown>());
								GameObject val4 = new GameObject("TitleText");
								val4.transform.SetParent(val3.transform);
								TextMeshProUGUI val5 = val4.AddComponent<TextMeshProUGUI>();
								((TMP_Text)val5).SetText(modName2, true);
								((TMP_Text)val5).margin = new Vector4(10f, 10f, 10f, 10f);
								val3.SetActive(true);
								val2 = val3.transform;
							}
							val.transform.SetParent(modContent.transform, false);
							int siblingIndex = val2.GetSiblingIndex();
							val.transform.SetSiblingIndex(siblingIndex + 1);
							((Object)val).name = "UI_" + modName2 + "_" + key2;
							Debug.Log((object)("已成功添加下拉选项: " + description2));
						}
						return;
					}
					catch (Exception ex)
					{
						Debug.LogError((object)("创建下拉列表时出错: " + ex.Message + "\n" + ex.StackTrace));
						return;
					}
				}
				Debug.LogError((object)"modContent 为 null，无法添加下拉列表");
			}
		});
	}

	public static void AddBoolDropdownList(string modName, string key, string description, bool defaultValue)
	{
		SortedDictionary<string, object> options = new SortedDictionary<string, object>
		{
			{ "True", true },
			{ "False", false }
		};
		AddDropdownList(modName, key, description, options, typeof(bool), defaultValue);
	}

	public static void AddInputWithSlider(string modName, string key, string description, Type valueType, object defaultValue, Vector2? sliderRange = null)
	{
		string key2 = key;
		string description2 = description;
		Type valueType2 = valueType;
		object defaultValue2 = defaultValue;
		string modName2 = modName;
		AddConfig(delegate
		{
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Expected O, but got Unknown
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)inputWithSliderPrefab == (Object)null)
			{
				Debug.LogError((object)"inputWithSliderPrefab 为 null，无法创建滑条");
			}
			else if ((Object)(object)dropdownListPrefab == (Object)null)
			{
				Debug.LogError((object)"dropdownListPrefab 为 null，无法创建滑条");
			}
			else
			{
				if (!((Object)(object)modContent == (Object)null))
				{
					try
					{
						GameObject val = Object.Instantiate<GameObject>(inputWithSliderPrefab);
						val.SetActive(true);
						OptionsUIEntry_Slider_Mod component = val.GetComponent<OptionsUIEntry_Slider_Mod>();
						if ((Object)(object)component == (Object)null)
						{
							Debug.LogError((object)"无法获取 OptionsUIEntry_Slider_Mod 组件");
							Object.Destroy((Object)(object)val);
						}
						else
						{
							component.Init(key2, description2, valueType2, defaultValue2, sliderRange);
							Transform val2 = modContent.transform.Find(modName2);
							if ((Object)(object)val2 == (Object)null)
							{
								GameObject val3 = Object.Instantiate<GameObject>(dropdownListPrefab, modContent.transform);
								val3.SetActive(false);
								((Object)val3).name = modName2;
								TrasnformExtensions.DestroyAllChildren(val3.transform);
								Object.Destroy((Object)(object)val3.GetComponent<OptionsUIEntry_Dropdown>());
								GameObject val4 = new GameObject("TitleText");
								val4.transform.SetParent(val3.transform);
								TextMeshProUGUI val5 = val4.AddComponent<TextMeshProUGUI>();
								((TMP_Text)val5).SetText(modName2, true);
								((TMP_Text)val5).margin = new Vector4(10f, 10f, 10f, 10f);
								val3.SetActive(true);
								val2 = val3.transform;
							}
							val.transform.SetParent(modContent.transform, false);
							int siblingIndex = val2.GetSiblingIndex();
							val.transform.SetSiblingIndex(siblingIndex + 1);
							((Object)val).name = "UI_" + modName2 + "_" + key2;
							Debug.Log((object)("已成功添加滑条选项: " + description2));
						}
						return;
					}
					catch (Exception ex)
					{
						Debug.LogError((object)("创建滑条时出错: " + ex.Message + "\n" + ex.StackTrace));
						return;
					}
				}
				Debug.LogError((object)"modContent 为 null，无法添加下拉列表");
			}
		});
	}

	public static void AddOnOptionsChangedDelegate(Action<string> action)
	{
		OptionsManager_Mod.OnConfigSaved += action;
	}

	public static void RemoveOnOptionsChangedDelegate(Action<string> action)
	{
		OptionsManager_Mod.OnConfigSaved -= action;
	}

	private void Awake()
	{
		Debug.Log((object)"ModConfig Mod Awake");
	}

	private void OnDestroy()
	{
		pendingConfigActions.Clear();
	}

	private void OnEnable()
	{
		Debug.Log((object)"ModConfig Enabled");
	}

	private void OnDisable()
	{
		Debug.Log((object)"ModConfig Disabled");
	}

	private void Update()
	{
		ProcessPendingConfigs();
	}

	private void OnMenuAwake()
	{
		Debug.Log((object)"ModConfig: OnMenuAwake");
		TryCreatingModSettingTab();
	}

	private void TryCreatingModSettingTab()
	{
		if (!createdModTab)
		{
			CreateModSettingTab();
			createdModTab = true;
		}
	}

	private void CreateModSettingTab()
	{
		Debug.Log((object)"开始创建Mod设置标签页");
		OptionsPanel val = ((IEnumerable<OptionsPanel>)Object.FindObjectsOfType<OptionsPanel>(true)).FirstOrDefault((Func<OptionsPanel, bool>)delegate(OptionsPanel panel)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			Scene scene = ((Component)panel).gameObject.scene;
			return ((Scene)(ref scene)).name == "DontDestroyOnLoad";
		});
		if ((Object)(object)val == (Object)null)
		{
			Debug.LogError((object)"OptionsPanel Not Found!!!");
			return;
		}
		Debug.Log((object)"OptionsPanel Found");
		List<OptionsPanel_TabButton> instanceFieldValue = ReflectionHelper.GetInstanceFieldValue<List<OptionsPanel_TabButton>>(val, "tabButtons");
		if (instanceFieldValue == null)
		{
			Debug.LogError((object)"无法反射获取tabButtons!!!!");
			return;
		}
		Debug.Log((object)"已反射获取tabButtons");
		GameObject val2 = Object.Instantiate<GameObject>(((Component)instanceFieldValue[0]).gameObject, ((Component)instanceFieldValue[0]).gameObject.transform.parent);
		((Object)val2).name = "modTab";
		OptionsPanel_TabButton component = val2.GetComponent<OptionsPanel_TabButton>();
		if ((Object)(object)component == (Object)null)
		{
			Debug.LogError((object)"无法获取克隆的GameObject的OptionsPanel_TabButton组件");
			Object.Destroy((Object)(object)val2);
			return;
		}
		modTabButton = component;
		GameObject instanceFieldValue2 = ReflectionHelper.GetInstanceFieldValue<GameObject>(component, "tab");
		if ((Object)(object)instanceFieldValue2 == (Object)null)
		{
			Debug.LogError((object)"无法反射获取modTabButton的tab成员");
			Object.Destroy((Object)(object)val2);
			return;
		}
		GameObject val3 = Object.Instantiate<GameObject>(instanceFieldValue2, instanceFieldValue2.transform.parent);
		((Object)val3).name = "modContent";
		modContent = val3;
		if (!ReflectionHelper.SetInstanceFieldValue(component, "tab", val3))
		{
			Debug.LogError((object)"反射修改tab成员失败!!");
			Object.Destroy((Object)(object)val2);
			Object.Destroy((Object)(object)val3);
			return;
		}
		instanceFieldValue.Add(component);
		ReflectionHelper.InvokeInstanceMethod(val, "Setup");
		TextMeshProUGUI componentInChildren = ((Component)component).GetComponentInChildren<TextMeshProUGUI>(true);
		if ((Object)(object)componentInChildren != (Object)null)
		{
			TextLocalizor componentInChildren2 = ((Component)component).GetComponentInChildren<TextLocalizor>(true);
			if ((Object)(object)componentInChildren2 != (Object)null)
			{
				Object.Destroy((Object)(object)componentInChildren2);
			}
			((TMP_Text)componentInChildren).SetText("Mod Settings", true);
		}
		if ((Object)(object)modContent != (Object)null)
		{
			TrasnformExtensions.DestroyAllChildren(modContent.transform);
			OptionsUIEntry_Dropdown val4 = ((IEnumerable<OptionsUIEntry_Dropdown>)((Component)val3.transform.parent).GetComponentsInChildren<OptionsUIEntry_Dropdown>(true)).FirstOrDefault((Func<OptionsUIEntry_Dropdown, bool>)((OptionsUIEntry_Dropdown dropdown) => ((Object)((Component)dropdown).gameObject).name == "UI_Resolution"));
			if ((Object)(object)val4 != (Object)null)
			{
				GameObject val5 = Object.Instantiate<GameObject>(((Component)val4).gameObject, modContent.transform);
				((Object)val5).name = "dropDownPrefab";
				val5.SetActive(false);
				dropdownListPrefab = val5;
				Debug.Log((object)"成功创建下拉列表预制体");
			}
			else
			{
				Debug.LogError((object)"未找到分辨率下拉列表作为模板");
			}
		}
		MakeInputWithSliderPrefab();
		Debug.Log((object)"Mod设置标签页创建完成");
		ProcessPendingConfigs();
	}

	private void MakeInputWithSliderPrefab()
	{
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)modContent == (Object)null)
		{
			Debug.LogError((object)"modContent为空! MakeInputWithSliderPrefab failed!");
		}
		OptionsUIEntry_Slider obj = Object.FindObjectsOfType<OptionsUIEntry_Slider>().First((OptionsUIEntry_Slider component) => ((Object)((Component)component).gameObject).name == "UI_MouseSensitivity");
		GameObject val = ((obj != null) ? ((Component)obj).gameObject : null);
		if ((Object)(object)val == (Object)null)
		{
			Debug.LogError((object)"无法找到鼠标灵敏度设置选项, 将无法使用InputWithSlider类型config");
			return;
		}
		GameObject val2 = Object.Instantiate<GameObject>(val);
		val2.SetActive(true);
		Behaviour[] componentsInChildren = val2.GetComponentsInChildren<Behaviour>(true);
		foreach (Behaviour val3 in componentsInChildren)
		{
			if ((Object)(object)val3 != (Object)null)
			{
				val3.enabled = true;
			}
		}
		Object.DontDestroyOnLoad((Object)(object)val2);
		OptionsUIEntry_Slider component2 = val2.GetComponent<OptionsUIEntry_Slider>();
		TextMeshProUGUI instanceFieldValue = ReflectionHelper.GetInstanceFieldValue<TextMeshProUGUI>(component2, "label");
		Slider instanceFieldValue2 = ReflectionHelper.GetInstanceFieldValue<Slider>(component2, "slider");
		TMP_InputField instanceFieldValue3 = ReflectionHelper.GetInstanceFieldValue<TMP_InputField>(component2, "valueField");
		Object.DestroyImmediate((Object)(object)component2);
		val2.SetActive(false);
		OptionsUIEntry_Slider_Mod optionsUIEntry_Slider_Mod = val2.AddComponent<OptionsUIEntry_Slider_Mod>();
		optionsUIEntry_Slider_Mod.label = instanceFieldValue;
		optionsUIEntry_Slider_Mod.slider = instanceFieldValue2;
		optionsUIEntry_Slider_Mod.valueField = instanceFieldValue3;
		((TMP_Text)optionsUIEntry_Slider_Mod.label).SetText("OptionsUIEntry_Slider_Mod", true);
		val2.SetActive(true);
		val2.transform.localScale = Vector3.one;
		inputWithSliderPrefab = val2;
	}

	private void TestAddDropDownlist()
	{
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		SortedDictionary<string, object> options = new SortedDictionary<string, object>
		{
			{ "选项1", 1 },
			{ "选项2", 2 },
			{ "选项3", 3 },
			{ "选项4", 4 },
			{ "选项5", 5 }
		};
		AddDropdownList("Mod A", "testA1", "Dropdown1", options, typeof(int), 1);
		AddDropdownList("Mod B", "testB1", "Dropdown1", options, typeof(int), 2);
		AddDropdownList("Mod A", "testA2", "Dropdown2", options, typeof(int), 3);
		AddDropdownList("Mod A", "testA3", "Dropdown3", options, typeof(int), 4);
		AddDropdownList("Mod B", "testB2", "Dropdown2", options, typeof(int), 5);
		AddInputWithSlider("Mod C", "testSlider1", "Float Slider no range", typeof(float), 33.89);
		AddInputWithSlider("Mod C", "testSlider2", "Float Slider with range(1.0-5.0)", typeof(float), (object)2.6, (Vector2?)new Vector2(1f, 5f));
		AddInputWithSlider("Mod C", "testSlider3", "Int Slider no range", typeof(int), 4);
		AddInputWithSlider("Mod C", "testSlider4", "Int Slider with range(5-20)", typeof(int), (object)12, (Vector2?)new Vector2(5f, 20f));
		AddInputWithSlider("Mod C", "testSlider5", "String without slider", typeof(string), "testStr");
	}

	protected override void OnAfterSetup()
	{
		Debug.Log((object)"已添加MainMenuAwake事件");
		MainMenu.OnMainMenuAwake = (Action)Delegate.Combine(MainMenu.OnMainMenuAwake, new Action(OnMenuAwake));
		if (Object.FindAnyObjectByType(typeof(MainMenu)) != (Object)null)
		{
			Debug.Log((object)"MainMenu 已存在，尝试创建设置标签页");
			TryCreatingModSettingTab();
		}
	}

	protected override void OnBeforeDeactivate()
	{
		MainMenu.OnMainMenuAwake = (Action)Delegate.Remove(MainMenu.OnMainMenuAwake, new Action(OnMenuAwake));
	}
}

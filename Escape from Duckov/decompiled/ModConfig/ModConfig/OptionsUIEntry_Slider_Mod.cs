using System;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ModConfig;

internal class OptionsUIEntry_Slider_Mod : MonoBehaviour
{
	private bool initDone = false;

	private string key = "";

	[Space]
	private object? defaultValue = null;

	public TextMeshProUGUI? label;

	public Slider? slider;

	public TMP_InputField? valueField;

	private Type valueType = typeof(float);

	private Vector2? sliderRange = null;

	public object Value
	{
		get
		{
			try
			{
				Type oldTypeOfKeyCached = ES3Helper.getOldTypeOfKeyCached(key);
				if (oldTypeOfKeyCached != valueType)
				{
					Debug.LogWarning((object)$"检测到{key}旧配置项类型{oldTypeOfKeyCached}与当前类型{valueType}不同, 已重置为新的默认值");
					ES3Helper.DeleteKey(key);
				}
				if (valueType == typeof(int))
				{
					return OptionsManager_Mod.Load(key, (int)defaultValue);
				}
				if (valueType == typeof(float))
				{
					Debug.Log((object)"正在读取float配置默认值");
					float num = Convert.ToSingle(defaultValue);
					Debug.Log((object)("float配置默认值为" + num.ToString("F1")));
					float num2 = OptionsManager_Mod.Load(key, num);
					Debug.Log((object)("float配置数值已读取=" + num2.ToString("F1")));
					return num2;
				}
				if (valueType == typeof(string))
				{
					Debug.Log((object)"正在读取string配置默认值");
					return OptionsManager_Mod.Load(key, (string)defaultValue);
				}
				if (valueType == typeof(bool))
				{
					Debug.Log((object)"正在读取bool配置默认值");
					return OptionsManager_Mod.Load(key, (bool)defaultValue);
				}
				Debug.LogError((object)$"不支持的配置值类型: {valueType}");
				return defaultValue;
			}
			catch (Exception ex)
			{
				Debug.LogError((object)$"获取配置值时发生错误: {ex.Message} valueType={valueType} key={key}");
				return defaultValue;
			}
		}
		set
		{
			try
			{
				Debug.Log((object)"正在设置配置值");
				if (valueType == typeof(int))
				{
					int obj = Convert.ToInt32(value);
					OptionsManager_Mod.Save(key, obj);
				}
				else if (valueType == typeof(float))
				{
					float obj2 = Convert.ToSingle(value);
					OptionsManager_Mod.Save(key, obj2);
				}
				else if (valueType == typeof(string))
				{
					OptionsManager_Mod.Save(key, value?.ToString() ?? "");
				}
				else if (valueType == typeof(bool))
				{
					bool obj3 = Convert.ToBoolean(value);
					OptionsManager_Mod.Save(key, obj3);
				}
				else
				{
					Debug.LogError((object)$"不支持保存的配置值类型: {valueType}");
				}
			}
			catch (Exception ex)
			{
				Debug.LogError((object)("保存配置值时发生错误: " + ex.Message));
			}
		}
	}

	private void Awake()
	{
		if ((Object)(object)slider != (Object)null)
		{
			((UnityEvent<float>)(object)slider.onValueChanged).AddListener((UnityAction<float>)OnSliderValueChanged);
		}
		if ((Object)(object)valueField != (Object)null)
		{
			((UnityEvent<string>)(object)valueField.onEndEdit).AddListener((UnityAction<string>)OnFieldEndEdit);
		}
		RefreshLable();
	}

	private void Start()
	{
		RefreshValues();
	}

	private void OnDestroy()
	{
		LocalizationManager.OnSetLanguage -= OnLanguageChanged;
	}

	private void OnLanguageChanged(SystemLanguage language)
	{
		RefreshLable();
	}

	private void RefreshLable()
	{
		if (initDone)
		{
		}
	}

	private void OnFieldEndEdit(string arg0)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (valueType == typeof(int))
			{
				if (int.TryParse(arg0, out var result))
				{
					if (sliderRange.HasValue)
					{
						result = Mathf.Clamp(result, (int)sliderRange.Value.x, (int)sliderRange.Value.y);
					}
					Value = result;
				}
				else
				{
					Debug.LogWarning((object)"请输入有效的整数");
					RefreshValues();
				}
			}
			else if (valueType == typeof(float))
			{
				if (float.TryParse(arg0, out var result2))
				{
					if (sliderRange.HasValue)
					{
						result2 = Mathf.Clamp(result2, sliderRange.Value.x, sliderRange.Value.y);
					}
					Value = result2;
				}
				else
				{
					Debug.LogWarning((object)"请输入有效的数字");
					RefreshValues();
				}
			}
			else if (valueType == typeof(string))
			{
				Value = arg0;
			}
			else if (valueType == typeof(bool))
			{
				if (bool.TryParse(arg0, out var result3))
				{
					Value = result3;
				}
				else
				{
					string text = arg0.ToLower();
					switch (text)
					{
					default:
						if (!(text == "on"))
						{
							switch (text)
							{
							default:
								if (!(text == "off"))
								{
									Debug.LogWarning((object)"请输入 true/false, 1/0, yes/no, on/off");
									RefreshValues();
									break;
								}
								goto case "false";
							case "false":
							case "0":
							case "no":
								Value = false;
								break;
							}
							break;
						}
						goto case "true";
					case "true":
					case "1":
					case "yes":
						Value = true;
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("处理输入框值时发生错误: " + ex.Message));
			RefreshValues();
		}
		RefreshValues();
	}

	private void OnEnable()
	{
		if (initDone)
		{
			RefreshValues();
		}
	}

	private void OnSliderValueChanged(float value)
	{
		if (initDone)
		{
			Value = value;
			RefreshValues();
		}
	}

	private void RefreshValues()
	{
		if (!initDone)
		{
			return;
		}
		try
		{
			object value = Value;
			if (valueType == typeof(int))
			{
				int num = Convert.ToInt32(value);
				if ((Object)(object)valueField != (Object)null)
				{
					valueField.SetTextWithoutNotify(num.ToString());
				}
				if ((Object)(object)slider != (Object)null)
				{
					slider.SetValueWithoutNotify((float)num);
				}
			}
			else if (valueType == typeof(float))
			{
				float valueWithoutNotify = Convert.ToSingle(value);
				if ((Object)(object)valueField != (Object)null)
				{
					valueField.SetTextWithoutNotify(valueWithoutNotify.ToString("F2"));
				}
				if ((Object)(object)slider != (Object)null)
				{
					slider.SetValueWithoutNotify(valueWithoutNotify);
				}
			}
			else if (valueType == typeof(string))
			{
				if ((Object)(object)valueField != (Object)null)
				{
					valueField.SetTextWithoutNotify((string)value);
				}
				if ((Object)(object)slider != (Object)null)
				{
					((Component)slider).gameObject.SetActive(false);
				}
			}
			else if (valueType == typeof(bool))
			{
				bool flag = Convert.ToBoolean(value);
				if ((Object)(object)valueField != (Object)null)
				{
					valueField.SetTextWithoutNotify(flag ? "True" : "False");
				}
				if ((Object)(object)slider != (Object)null)
				{
					((Component)slider).gameObject.SetActive(false);
				}
			}
			else
			{
				Debug.LogError((object)$"不支持Refresh的配置值类型: {valueType}");
			}
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("刷新UI值时发生错误: " + ex.Message));
		}
	}

	public void Init(string key, string description, Type valueType, object defaultValue, Vector2? sliderRange)
	{
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		if (defaultValue == null)
		{
			Debug.LogError((object)("配置项默认值不能为null: " + key));
			return;
		}
		if ((Object)(object)valueField == (Object)null)
		{
			Debug.LogError((object)"valueField为空!!");
			return;
		}
		valueField.contentType = (ContentType)0;
		valueField.characterLimit = 1000;
		if ((Object)(object)slider == (Object)null)
		{
			Debug.LogError((object)"slider为空!!");
			return;
		}
		this.key = key;
		this.defaultValue = defaultValue;
		this.valueType = valueType;
		this.sliderRange = sliderRange;
		if ((Object)(object)label != (Object)null)
		{
			((TMP_Text)label).SetText(description, true);
		}
		if (sliderRange.HasValue)
		{
			slider.minValue = sliderRange.Value.x;
			slider.maxValue = sliderRange.Value.y;
			if (valueType == typeof(int))
			{
				slider.wholeNumbers = true;
			}
			else
			{
				slider.wholeNumbers = false;
			}
			((Component)slider).gameObject.SetActive(true);
			try
			{
				if (valueType == typeof(float))
				{
					float num = Convert.ToSingle(defaultValue);
					if (num < sliderRange.Value.x || num > sliderRange.Value.y)
					{
						Debug.LogError((object)$"配置项{key}的默认值{num}超出设置的范围[{sliderRange.Value.x}, {sliderRange.Value.y}]");
					}
				}
				else if (valueType == typeof(int))
				{
					int num2 = Convert.ToInt32(defaultValue);
					if ((float)num2 < sliderRange.Value.x || (float)num2 > sliderRange.Value.y)
					{
						Debug.LogError((object)$"配置项{key}的默认值{num2}超出设置的范围[{sliderRange.Value.x}, {sliderRange.Value.y}]");
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError((object)("验证默认值范围时出错: " + ex.Message));
			}
		}
		else
		{
			if (valueType == typeof(int))
			{
				slider.minValue = -2.1474836E+09f;
				slider.maxValue = 2.1474836E+09f;
				slider.wholeNumbers = true;
			}
			else if (valueType == typeof(float))
			{
				slider.minValue = -100000f;
				slider.maxValue = 100000f;
				slider.wholeNumbers = false;
			}
			((Component)slider).gameObject.SetActive(true);
			Debug.Log((object)$"已解除slider范围限制 - Key: {key}, 新范围: [{slider.minValue}, {slider.maxValue}]");
		}
		initDone = true;
		RefreshValues();
	}
}

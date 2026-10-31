using System;
using System.Globalization;
using UnityEngine;

public abstract class ConfigEntry
{
	public string Key { get; internal set; }

	public string Section { get; internal set; }

	public Func<string> Label { get; internal set; }

	public Func<string> Tooltip { get; internal set; }

	public bool RequiresRestart { get; internal set; }

	public ModConfig Config { get; internal set; }

	public bool HasRange { get; private set; }

	public float Min { get; private set; }

	public float Max { get; private set; }

	public string[] Choices { get; internal set; }

	public abstract Type ValueType { get; }

	public abstract object BoxedValue { get; }

	public abstract object BoxedDefault { get; }

	internal void SetRange(float min, float max)
	{
		HasRange = true;
		Min = Mathf.Min(min, max);
		Max = Mathf.Max(min, max);
	}

	public string Serialize()
	{
		return Format(BoxedValue);
	}

	internal abstract bool TryDeserialize(string text);

	internal abstract bool ApplyBoxed(object value);

	public static string Format(object value)
	{
		if (value != null)
		{
			if (!(value is bool))
			{
				if (!(value is int num))
				{
					if (!(value is float num2))
					{
						if (value is Enum @enum)
						{
							return @enum.ToString();
						}
						return value.ToString();
					}
					return num2.ToString("R", CultureInfo.InvariantCulture);
				}
				return num.ToString(CultureInfo.InvariantCulture);
			}
			if (!(bool)value)
			{
				return "false";
			}
			return "true";
		}
		return "";
	}
}
You are not using the latest version of the tool, please update.
Latest version is '11.2.0.10050' (yours is '8.2.0.7535-95108c96')

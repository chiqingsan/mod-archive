using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SmartInventoryArrange.Core;

internal sealed class FrozenPreferenceModel
{
	private readonly string[] _names;

	private readonly double[] _scale;

	private readonly double[] _weights;

	private static readonly Lazy<FrozenPreferenceModel> Embedded = new Lazy<FrozenPreferenceModel>(LoadCore);

	private FrozenPreferenceModel(string[] names, double[] scale, double[] weights)
	{
		_names = names;
		_scale = scale;
		_weights = weights;
	}

	public static FrozenPreferenceModel LoadEmbedded()
	{
		return Embedded.Value;
	}

	private static FrozenPreferenceModel LoadCore()
	{
		Stream val = typeof(FrozenPreferenceModel).Assembly.GetManifestResourceStream("SmartInventoryArrange.directional21.json") ?? throw new InvalidOperationException("Frozen preference resource missing.");
		try
		{
			using var reader = new StreamReader(val);
			string text = reader.ReadToEnd();
			if (!text.Contains("\"kind\": \"nonnegative-regularized-pair-logistic\"") || !text.Contains("\"profile\": \"semantic\""))
				throw new InvalidOperationException("Frozen model schema mismatch.");
			string[] array = ReadStrings(text, "featureNames");
			double[] array2 = ReadNumbers(text, "scale");
			double[] array3 = ReadNumbers(text, "weights");
			if (array.Length != array2.Length || array.Length != array3.Length || Enumerable.Count(Enumerable.Distinct(array)) != array.Length || Enumerable.Any(array2, (double v) => !double.IsFinite(v) || v <= 0.0) || Enumerable.Any(array3, (double v) => !double.IsFinite(v) || v < 0.0))
			{
				throw new InvalidOperationException("Frozen model numeric domain invalid.");
			}
			return new FrozenPreferenceModel(array, array2, array3);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static string[] ReadStrings(string text, string name)
	{
		Match match = Regex.Match(text, "\\\"" + name + "\\\"\\s*:\\s*\\[(.*?)\\]", RegexOptions.Singleline);
		if (!match.Success) throw new InvalidOperationException("Frozen model property missing: " + name);
		return Regex.Matches(match.Groups[1].Value, "\\\"((?:\\\\.|[^\\\"])*)\\\"")
			.Cast<Match>().Select(x => x.Groups[1].Value).ToArray();
	}

	private static double[] ReadNumbers(string text, string name)
	{
		Match match = Regex.Match(text, "\\\"" + name + "\\\"\\s*:\\s*\\[(.*?)\\]", RegexOptions.Singleline);
		if (!match.Success) throw new InvalidOperationException("Frozen model property missing: " + name);
		return Regex.Matches(match.Groups[1].Value, "[-+]?(?:\\d+\\.?\\d*|\\.\\d+)(?:[eE][-+]?\\d+)?")
			.Cast<Match>().Select(x => double.Parse(x.Value, System.Globalization.CultureInfo.InvariantCulture)).ToArray();
	}

	public double Value(IReadOnlyDictionary<string, double> features)
	{
		double num = 0.0;
		for (int i = 0; i < _names.Length; i++)
		{
			if (!features.TryGetValue(_names[i], out var value) || !double.IsFinite(value))
			{
				throw new InvalidOperationException("Preference feature missing or not finite: " + _names[i]);
			}
			num += value / _scale[i] * _weights[i];
		}
		return num;
	}
}

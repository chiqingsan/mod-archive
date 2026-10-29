using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

internal static class StableText
{
    internal static string Serialize(object? value)
    {
        var builder = new StringBuilder();
        Append(builder, value, new HashSet<object>(ReferenceEqualityComparer.Instance));
        return builder.ToString();
    }

    private static void Append(StringBuilder builder, object? value, HashSet<object> seen)
    {
        if (value == null) { builder.Append("null"); return; }
        if (value is string text) { AppendString(builder, text); return; }
        if (value is bool boolean) { builder.Append(boolean ? "true" : "false"); return; }
        if (value is IFormattable formattable && value is not IEnumerable)
        {
            builder.Append(formattable.ToString(null, CultureInfo.InvariantCulture));
            return;
        }
        if (value is IEnumerable enumerable)
        {
            builder.Append('[');
            var first = true;
            foreach (var item in enumerable)
            {
                if (!first) builder.Append(',');
                first = false;
                Append(builder, item, seen);
            }
            builder.Append(']');
            return;
        }
        if (!value.GetType().IsValueType && !seen.Add(value))
            throw new InvalidOperationException("Stable serialization encountered a reference cycle.");

        builder.Append('{');
        var members = value.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public)
            .Where(m => m.MemberType is MemberTypes.Field or MemberTypes.Property)
            .Where(m => m is not PropertyInfo p || p.GetIndexParameters().Length == 0)
            .OrderBy(m => m.Name, StringComparer.Ordinal)
            .ToArray();
        for (var i = 0; i < members.Length; i++)
        {
            if (i != 0) builder.Append(',');
            AppendString(builder, members[i].Name);
            builder.Append(':');
            Append(builder, members[i] is FieldInfo field ? field.GetValue(value) : ((PropertyInfo)members[i]).GetValue(value), seen);
        }
        builder.Append('}');
    }

    private static void AppendString(StringBuilder builder, string value)
    {
        builder.Append('"');
        foreach (var character in value)
        {
            if (character == '"' || character == '\\') builder.Append('\\');
            builder.Append(character);
        }
        builder.Append('"');
    }

    private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        internal static readonly ReferenceEqualityComparer Instance = new();
        public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);
        public int GetHashCode(object obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
    }
}

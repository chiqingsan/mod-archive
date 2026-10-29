using System;
using System.Globalization;
using System.Text;

public struct DefaultInterpolatedStringHandler
{
    private StringBuilder _builder;

    public DefaultInterpolatedStringHandler(int literalLength, int formattedCount)
    {
        _builder = new StringBuilder(literalLength);
    }

    public void AppendLiteral(string value) => _builder.Append(value);
    public void AppendFormatted<T>(T value) => _builder.Append(value);
    public void AppendFormatted<T>(T value, string? format) => _builder.Append(value is IFormattable f ? f.ToString(format, CultureInfo.InvariantCulture) : value?.ToString());
    public string ToStringAndClear()
    {
        var value = _builder.ToString();
        _builder.Clear();
        return value;
    }
}

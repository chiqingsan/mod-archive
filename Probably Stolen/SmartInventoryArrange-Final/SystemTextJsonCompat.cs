using System.Text;

namespace System.Text.Json;

internal static class JsonSerializer
{
    internal static string Serialize<T>(T value) => StableText.Serialize(value);
    internal static byte[] SerializeToUtf8Bytes<T>(T value) => Encoding.UTF8.GetBytes(StableText.Serialize(value));
}

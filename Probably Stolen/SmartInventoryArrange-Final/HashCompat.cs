using System;
using System.Security.Cryptography;

internal static class HashCompat
{
    internal static string Sha256Hex(byte[] data)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(data);
        return BitConverter.ToString(bytes).Replace("-", "", StringComparison.Ordinal);
    }

    internal static int SingleBits(float value) => BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
    internal static long DoubleBits(double value) => BitConverter.ToInt64(BitConverter.GetBytes(value), 0);
}

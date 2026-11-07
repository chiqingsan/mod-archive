using System;
using System.Globalization;

namespace CustomerCreditBoost;

/// <summary>
/// 顾客介绍文本在顾客创建时就已把当时的预算数字格式化并固化，事后无法重新本地化，
/// 所以只能把文本里的旧预算数字整体替换成加成后的新数字。替换要求数字独立成词，
/// 避免把 “1200” 里的 “200” 或 “1,200” 里的 “200” 误改。
/// </summary>
internal static class BudgetTextSync
{
    internal static string ReplaceBudget(string text, int oldBudget, int newBudget)
    {
        if (string.IsNullOrEmpty(text) || oldBudget == newBudget)
            return text;

        var oldDigits = oldBudget.ToString(CultureInfo.InvariantCulture);
        var newDigits = newBudget.ToString(CultureInfo.InvariantCulture);
        var oldGrouped = oldBudget.ToString("N0", CultureInfo.InvariantCulture);
        var newGrouped = newBudget.ToString("N0", CultureInfo.InvariantCulture);

        var result = text;
        if (!string.Equals(oldGrouped, oldDigits, StringComparison.Ordinal))
            result = ReplaceStandaloneNumber(result, oldGrouped, newGrouped);

        return ReplaceStandaloneNumber(result, oldDigits, newDigits);
    }

    private static string ReplaceStandaloneNumber(string text, string oldNumber, string newNumber)
    {
        var index = 0;
        while (index < text.Length)
        {
            var found = text.IndexOf(oldNumber, index, StringComparison.Ordinal);
            if (found < 0)
                return text;

            if (!IsStandaloneNumber(text, found, oldNumber.Length))
            {
                index = found + 1;
                continue;
            }

            text = text.Substring(0, found) + newNumber + text.Substring(found + oldNumber.Length);
            index = found + newNumber.Length;
        }

        return text;
    }

    private static bool IsStandaloneNumber(string text, int start, int length)
    {
        var end = start + length;
        return !IsNumberBoundary(text, start - 1, -1) && !IsNumberBoundary(text, end, 1);
    }

    private static bool IsNumberBoundary(string text, int index, int direction)
    {
        if (index < 0 || index >= text.Length)
            return false;

        var c = text[index];
        if (char.IsDigit(c))
            return true;

        // 分隔符只有在另一侧仍是数字时才算数字的一部分（如 “1,200” 中的 “200”）。
        if (c != ',' && c != '.' && c != '\u00A0' && c != '\u202F')
            return false;

        var neighbor = index + direction;
        return neighbor >= 0 && neighbor < text.Length && char.IsDigit(text[neighbor]);
    }
}

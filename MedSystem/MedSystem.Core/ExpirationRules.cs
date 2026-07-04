using System.Globalization;

namespace MedSystem.Core;

/// <summary>
/// Бизнес-правила по срокам (перенос logic из python validators.py).
/// Даты хранятся строками "дд.мм.гггг". Сравнение по дням:
/// документ или лекарство со сроком «сегодня» ещё действует.
/// </summary>
public static class ExpirationRules
{
    /// <summary>За сколько дней предупреждать об истечении.</summary>
    public const int WarningDays = 14;

    public const string DateFormat = "dd.MM.yyyy";

    public static bool TryParseDate(string? value, out DateOnly date)
    {
        return DateOnly.TryParseExact(
            value ?? "", DateFormat, CultureInfo.InvariantCulture,
            DateTimeStyles.None, out date);
    }

    /// <summary>
    /// Статус документов человека (санминимум/медосмотр/флюорография).
    /// Каждый документ действует 1 год с даты прохождения.
    /// </summary>
    public static (bool IsExpired, bool IsExpiring) GetPersonStatus(IEnumerable<string> checkupDates)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var warningEdge = today.AddDays(WarningDays);

        var isExpired = false;
        var isExpiring = false;
        foreach (var value in checkupDates)
        {
            if (!TryParseDate(value, out var passed))
                continue;
            var expires = passed.AddYears(1);  // 29 февраля → 28 февраля автоматически
            if (expires < today)
                isExpired = true;
            else if (expires <= warningEdge)
                isExpiring = true;
        }
        return (isExpired, isExpiring);
    }

    /// <summary>Статус срока годности лекарства.</summary>
    public static (bool IsExpired, bool IsExpiring) GetMedicineStatus(string expirationDate)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var warningEdge = today.AddDays(WarningDays);

        if (!TryParseDate(expirationDate, out var expires))
            return (false, false);

        return (expires < today, expires >= today && expires <= warningEdge);
    }
}

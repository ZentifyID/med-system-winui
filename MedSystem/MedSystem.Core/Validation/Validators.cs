using MedSystem.Core.Models;

namespace MedSystem.Core.Validation;

/// <summary>
/// Валидация форм (перенос правил из python validators.py).
/// Возвращает список сообщений об ошибках; пустой список — всё в порядке.
/// </summary>
public static class Validators
{
    /// <summary>Буквы, дефис и пробел — для двойных фамилий и составных имён.</summary>
    public static bool IsValidName(string value)
    {
        var stripped = value.Replace("-", "").Replace(" ", "");
        return stripped.Length > 0 && stripped.All(char.IsLetter);
    }

    public static bool IsValidDate(string value) =>
        ExpirationRules.TryParseDate(value, out _);

    public static List<string> ValidateEmployee(Employee e)
    {
        var errors = new List<string>();

        CheckName(errors, e.LastName, "Фамилия");
        CheckName(errors, e.FirstName, "Имя");
        CheckName(errors, e.MiddleName, "Отчество");
        CheckDate(errors, e.BirthDate, "Дата рождения");

        if (e.Affiliation is not ("основной" or "внешний"))
            errors.Add("Поле 'Принадлежность' должно быть 'основной' или 'внешний'.");

        CheckDigits(errors, e.PassportSeries, "Паспорт: серия", 4);
        CheckDigits(errors, e.PassportNumber, "Паспорт: номер", 6);
        CheckRequired(errors, e.PassportIssuedBy, "Паспорт: кем выдан", maxLength: 255);
        CheckDate(errors, e.PassportIssueDate, "Паспорт: дата выдачи");
        CheckDigits(errors, e.PassportDepartmentCode, "Паспорт: код подразделения", 6);

        CheckDigits(errors, e.Oms, "ОМС", 16);
        CheckRequired(errors, e.Address, "Адрес проживания", maxLength: 255);
        CheckDate(errors, e.SanminimumDate, "Дата санминимума");
        CheckDate(errors, e.MedicalExamDate, "Дата медосмотра");
        CheckDate(errors, e.FluorographyDate, "Дата флюорографии");

        return errors;
    }

    public static List<string> ValidateStudent(Student s)
    {
        var errors = new List<string>();

        if (s.GroupId <= 0)
            errors.Add("Поле 'Группа' обязательно.");

        CheckName(errors, s.LastName, "Фамилия");
        CheckName(errors, s.FirstName, "Имя");
        CheckName(errors, s.MiddleName, "Отчество");
        CheckDate(errors, s.BirthDate, "Дата рождения");

        CheckDigits(errors, s.Oms, "ОМС", 16);
        CheckRequired(errors, s.Address, "Адрес проживания", maxLength: 255);
        CheckDate(errors, s.SanminimumDate, "Дата санминимума");
        CheckDate(errors, s.MedicalExamDate, "Дата медосмотра");
        CheckDate(errors, s.FluorographyDate, "Дата флюорографии");

        return errors;
    }

    public static List<string> ValidateMedicine(Medicine m)
    {
        var errors = new List<string>();
        CheckRequired(errors, m.Name, "Название", maxLength: 255);
        CheckRequired(errors, m.Dosage, "Дозировка", maxLength: 64);
        if (m.Quantity < 0)
            errors.Add("Поле 'Количество' не может быть отрицательным.");
        CheckDate(errors, m.ExpirationDate, "Срок годности");
        return errors;
    }

    public static List<string> ValidateAppeal(Appeal a)
    {
        var errors = new List<string>();
        if (a.Number <= 0)
            errors.Add("Номер обращения должен быть положительным числом.");
        CheckDate(errors, a.CreatedAt, "Дата обращения");
        CheckRequired(errors, a.Sender, "Отправитель", maxLength: 255);
        CheckRequired(errors, a.Complaints, "Жалобы", maxLength: 2000);
        return errors;
    }

    // ── Вспомогательные проверки ─────────────────────────────────────

    private static void CheckRequired(List<string> errors, string value, string label, int maxLength = 0)
    {
        if (string.IsNullOrWhiteSpace(value))
            errors.Add($"Поле '{label}' обязательно.");
        else if (maxLength > 0 && value.Length > maxLength)
            errors.Add($"Поле '{label}' не должно превышать {maxLength} символов.");
    }

    private static void CheckName(List<string> errors, string value, string label)
    {
        if (string.IsNullOrWhiteSpace(value))
            errors.Add($"Поле '{label}' обязательно.");
        else if (value.Length > 50)
            errors.Add($"Поле '{label}' не должно превышать 50 символов.");
        else if (!IsValidName(value))
            errors.Add($"Поле '{label}' должно содержать только буквы (допускаются дефис и пробел).");
    }

    private static void CheckDate(List<string> errors, string value, string label)
    {
        if (string.IsNullOrWhiteSpace(value))
            errors.Add($"Поле '{label}' обязательно.");
        else if (!IsValidDate(value))
            errors.Add($"Поле '{label}' должно быть в формате ДД.ММ.ГГГГ и корректной датой.");
    }

    private static void CheckDigits(List<string> errors, string value, string label, int exactLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            errors.Add($"Поле '{label}' обязательно.");
        else if (value.Length != exactLength || !value.All(char.IsDigit))
            errors.Add($"Поле '{label}' должно содержать ровно {exactLength} цифр.");
    }
}

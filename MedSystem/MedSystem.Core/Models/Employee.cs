namespace MedSystem.Core.Models;

/// <summary>Сотрудник. Даты хранятся строками "дд.мм.гггг" — как в python-версии.</summary>
public class Employee
{
    public long Id { get; set; }
    public string LastName { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string MiddleName { get; set; } = "";
    public string BirthDate { get; set; } = "";
    /// <summary>"основной" или "внешний" (в БД действует CHECK-ограничение).</summary>
    public string Affiliation { get; set; } = "основной";
    public string PassportSeries { get; set; } = "";
    public string PassportNumber { get; set; } = "";
    public string PassportIssuedBy { get; set; } = "";
    public string PassportIssueDate { get; set; } = "";
    public string PassportDepartmentCode { get; set; } = "";
    public string Oms { get; set; } = "";
    public string Address { get; set; } = "";
    public string SanminimumDate { get; set; } = "";
    public string MedicalExamDate { get; set; } = "";
    public string FluorographyDate { get; set; } = "";

    public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
}

namespace MedSystem.Data.Models;

public class Appeal
{
    public long Id { get; set; }
    public long Number { get; set; }
    public string CreatedAt { get; set; } = "";
    public string Sender { get; set; } = "";
    public string BirthDate { get; set; } = "";
    public string ParentPhone { get; set; } = "";
    public string GroupName { get; set; } = "";
    public string Complaints { get; set; } = "";
    public string Diagnosis { get; set; } = "";
    public string ActionsRecommendations { get; set; } = "";
}

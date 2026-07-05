namespace MedSystem.Core.Models;

public class Medicine
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string Dosage { get; set; } = "";
    public long Quantity { get; set; }
    public string ExpirationDate { get; set; } = "";
}

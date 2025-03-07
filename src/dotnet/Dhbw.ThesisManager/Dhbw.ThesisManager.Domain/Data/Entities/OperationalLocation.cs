namespace Dhbw.ThesisManager.Domain.Data.Entities;

public class OperationalLocation
{
    public long Id { get; set; }
    public string CompanyName { get; set; }
    public string Department { get; set; }
    
    public long AddressId { get; set; }
    public Address Address { get; set; }
    
    public long ThesisId { get; set; }
    public Thesis Thesis { get; set; }
}

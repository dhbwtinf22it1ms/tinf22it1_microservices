namespace Dhbw.ThesisManager.Domain.Data.Entities;

public class PartnerCompany
{
    public long Id { get; set; }
    public string Name { get; set; }
    
    public long AddressId { get; set; }
    public Address Address { get; set; }
    
    public long ThesisId { get; set; }
    public Thesis Thesis { get; set; }
}

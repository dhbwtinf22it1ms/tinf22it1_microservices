namespace Dhbw.ThesisManager.Domain.Data.Entities;

public class Address
{
    public long Id { get; set; }
    public string Street { get; set; }
    public int ZipCode { get; set; }
    public string City { get; set; }
    public string? Country { get; set; }
}

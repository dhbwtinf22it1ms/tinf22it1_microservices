namespace Dhbw.ThesisManager.Api.Data.Entities;

public class InCompanySupervisor
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string AcademicTitle { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string AcademicDegree { get; set; }
    
    public long ThesisId { get; set; }
    public Thesis Thesis { get; set; }
}

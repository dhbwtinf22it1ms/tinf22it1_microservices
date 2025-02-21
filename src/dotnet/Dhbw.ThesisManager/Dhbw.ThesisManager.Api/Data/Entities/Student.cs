namespace Dhbw.ThesisManager.Api.Data.Entities;

public class Student
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string RegistrationNumber { get; set; }
    public string Course { get; set; }
    
    public Thesis? Thesis { get; set; }
}

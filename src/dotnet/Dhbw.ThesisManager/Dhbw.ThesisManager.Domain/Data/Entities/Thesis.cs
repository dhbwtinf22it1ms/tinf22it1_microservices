namespace Dhbw.ThesisManager.Domain.Data.Entities;

public class Thesis
{
    public long Id { get; set; }
    public string Topic { get; set; }
    
    public long StudentId { get; set; }
    public Student Student { get; set; }
    
    public DateTime PreparationPeriodFrom { get; set; }
    public DateTime PreparationPeriodTo { get; set; }
    
    public PartnerCompany PartnerCompany { get; set; }
    public OperationalLocation OperationalLocation { get; set; }
    public InCompanySupervisor InCompanySupervisor { get; set; }
    
    public List<string> ExcludeSupervisorsFromCompanies { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
}

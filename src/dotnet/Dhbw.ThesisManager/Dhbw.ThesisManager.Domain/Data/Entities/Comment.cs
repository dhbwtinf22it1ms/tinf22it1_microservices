namespace Dhbw.ThesisManager.Domain.Data.Entities;

public class Comment
{
    public long Id { get; set; }
    public long Author { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public long ThesisId { get; set; }
    public Thesis Thesis { get; set; }
}

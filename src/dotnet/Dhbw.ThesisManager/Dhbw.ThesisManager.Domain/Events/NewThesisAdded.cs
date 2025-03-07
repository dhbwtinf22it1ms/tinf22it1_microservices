namespace Dhbw.ThesisManager.Domain.Events;

public record NewThesisAdded
{
    public Guid ThesisId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string StudentName { get; init; } = string.Empty;
    public string StudentEmail { get; init; } = string.Empty;
    public string SupervisorName { get; init; } = string.Empty;
    public string SupervisorEmail { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
}

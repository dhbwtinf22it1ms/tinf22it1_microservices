namespace Dhbw.ThesisManager.Api.Services;

public interface IEventPublisher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default);
}

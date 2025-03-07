using EasyNetQ;

namespace Dhbw.ThesisManager.Api.Services;

public class EventPublisher : IEventPublisher
{
    private readonly IBus _bus;

    public EventPublisher(IBus bus)
    {
        _bus = bus;
    }

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        return _bus.PubSub.PublishAsync(message, cancellationToken);
    }
}

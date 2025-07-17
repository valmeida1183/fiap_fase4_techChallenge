using Core.Message.Interface;
using MassTransit;

namespace Infraestructure.Message;
public class MessagePublisher : IMessagePublisher
{
    private readonly IBus _bus;

    public MessagePublisher(IBus bus)
    {
        _bus = bus;
    }

    public async Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        await _bus.Publish(message, cancellationToken);
    }

    public async Task Send<T>(Uri destinationAddress, T message, CancellationToken cancellationToken = default) where T : class
    {
        var endpoint = await _bus.GetSendEndpoint(destinationAddress);
        await endpoint.Send(message, cancellationToken);
    }
}

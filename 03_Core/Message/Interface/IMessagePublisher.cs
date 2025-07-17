namespace Core.Message.Interface;
public interface IMessagePublisher
{
    Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : class;
    Task Send<T>(Uri destinationAddress,  T message, CancellationToken cancellationToken = default) where T : class;
}

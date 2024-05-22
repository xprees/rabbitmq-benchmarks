using MassTransit;
using Messaging.RabbitMQ.MassTransit;

namespace Messaging.RabbitMQ;

public class MassTransitHelper : IDisposable
{
    private readonly IBusControl _busControl;

    public MassTransitHelper(bool useInMemoryBus = false)
    {
        if (useInMemoryBus)
        {
            _busControl = SetupBusControlInMemory();
            return;
        }

        _busControl = SetupBusControlWithRabbit();
    }

    public TestingConsumer Consumer { get; } = new();

    private IBusControl SetupBusControlInMemory() => Bus.Factory
        .CreateUsingInMemory(config =>
        {
            config.ReceiveEndpoint(queueName: $"MassTransit-{RabbitConstants.ReceiveQueue}",
                endpoint => { endpoint.Handler<TestRequest>(context => Consumer.Consume(context)); });
        });


    private IBusControl SetupBusControlWithRabbit() => Bus.Factory
        .CreateUsingRabbitMq(config =>
        {
            config.Host("localhost", hostConfig =>
            {
                hostConfig.Username("guest");
                hostConfig.Password("guest");
            });

            config.ReceiveEndpoint(queueName: $"MassTransit-{RabbitConstants.ReceiveQueue}",
                endpoint => { endpoint.Handler<TestRequest>(context => Consumer.Consume(context)); });
        });

    public void Start() => _busControl.Start();

    public Task PublishMessage(TestRequest request) => _busControl.Publish(request);

    public void Dispose() => _busControl?.Stop();
}
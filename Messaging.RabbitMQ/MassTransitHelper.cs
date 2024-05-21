using MassTransit;
using Messaging.RabbitMQ.MassTransit;

namespace Messaging.RabbitMQ;

public class MassTransitHelper : IDisposable
{
    private readonly IBusControl _busControl = Bus.Factory
        .CreateUsingRabbitMq(config =>
        {
            config.Host("localhost", hostConfig =>
            {
                hostConfig.Username("guest");
                hostConfig.Password("guest");
            });

            config.ReceiveEndpoint(queueName: $"MassTransit-{RabbitConstants.ReceiveQueue}", endpoint =>
            {
                endpoint.Handler<TestRequest>(async context =>
                {
                    var consumer = new TestingConsumer();
                    await consumer.Consume(context);
                });
            });
        });

    public void Start() => _busControl.Start();

    public Task PublishMessage(TestRequest request) => _busControl.Publish(request);

    public void Dispose() => _busControl?.Stop();
}
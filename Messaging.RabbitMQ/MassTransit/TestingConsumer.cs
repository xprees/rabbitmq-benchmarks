using MassTransit;

namespace Messaging.RabbitMQ.MassTransit;

public class TestingConsumer : IConsumer<TestRequest>
{
    public Task Consume(ConsumeContext<TestRequest> context)
    {
        var message = context.Message.Message;
        Console.WriteLine($"MassTransit Received: {message}");
        return Task.CompletedTask;
    }
}
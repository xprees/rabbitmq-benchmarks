using MassTransit;

namespace Messaging.RabbitMQ.MassTransit;

public class TestingConsumer : IConsumer<TestRequest>
{
    private TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> WaitForMessage() => _taskCompletionSource.Task;

    public void Reset() => _taskCompletionSource = new TaskCompletionSource<bool>();

    public Task Consume(ConsumeContext<TestRequest> context)
    {
        _taskCompletionSource.SetResult(true);
        return Task.CompletedTask;
    }
}
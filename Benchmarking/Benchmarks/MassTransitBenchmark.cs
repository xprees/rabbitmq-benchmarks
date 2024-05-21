using BenchmarkDotNet.Engines;
using Messaging.RabbitMQ;
using Messaging.RabbitMQ.MassTransit;
using RabbitMQ.Client.Events;

namespace Benchmarking.Benchmarks;

[MarkdownExporter]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
[SimpleJob(RunStrategy.Throughput, warmupCount: 100)]
public class MassTransitBenchmark
{
    private MassTransitHelper _massTransitHelper = null!;

    private RabbitTestingHelper _sendTestingHelper = null!;
    private RabbitTestingHelper _receiveTestingHelper = null!;
    private TaskCompletionSource<bool> _rabbitMessageReceivedSource = null!;

    [GlobalSetup]
    public void Setup()
    {
        _sendTestingHelper = new RabbitTestingHelper(RabbitConstants.SendQueue);
        _receiveTestingHelper =
            new RabbitTestingHelper(RabbitConstants.SendQueue, onMessageReceived: OnConsumerOnReceived);

        _massTransitHelper = new MassTransitHelper();
        _massTransitHelper.Start();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _massTransitHelper?.Dispose();
        _sendTestingHelper?.Dispose();
        _receiveTestingHelper?.Dispose();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _massTransitHelper.Consumer.Reset();
    }


    [Benchmark(Baseline = true, Description = "RabbitClient", OperationsPerInvoke = 100)]
    public async Task MessageRabbitWaitForReply()
    {
        _rabbitMessageReceivedSource = new TaskCompletionSource<bool>(); // Reset the source
        _sendTestingHelper.SendMessage("Test"u8.ToArray());

        // Wait for the reply
        await _rabbitMessageReceivedSource.Task
            .ConfigureAwait(false);
    }

    [Benchmark(Description = "MassTransit", OperationsPerInvoke = 100)]
    public async Task PublishMessage()
    {
        await _massTransitHelper.PublishMessage(new TestRequest("Test"));
        await _massTransitHelper.Consumer.WaitForMessage()
            .ConfigureAwait(false);
    }

    private void OnConsumerOnReceived(object? model, BasicDeliverEventArgs ea) =>
        _rabbitMessageReceivedSource?.SetResult(true);
}
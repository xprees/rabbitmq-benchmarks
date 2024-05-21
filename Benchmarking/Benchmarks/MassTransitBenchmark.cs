using System.Text;
using Messaging.RabbitMQ;
using Messaging.RabbitMQ.MassTransit;
using RabbitMQ.Client.Events;

namespace Benchmarking.Benchmarks;

[MarkdownExporter]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class MassTransitBenchmark
{
    private MassTransitHelper _massTransitHelper = null!;

    private RabbitTestingHelper _sendTestingHelper = null!;
    private RabbitTestingHelper _receiveTestingHelper = null!;
    private TaskCompletionSource<bool> _rabbitMessageReceivedSource = null!;

    // 1KB, 100KB, 512KB, 1MB, 8MB, 16MB
    [Params(1024, 1024 * 100, 1024 * 512, 1_048_576, 8_388_608, 16_777_216)]
    public int MessageSize { get; set; }

    private byte[] _message = null!;
    private string _messageAsString = null!;

    [GlobalSetup]
    public void Setup()
    {
        _message = new byte[MessageSize];
        _messageAsString = Encoding.UTF8.GetString(_message);

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

    [Benchmark(Baseline = true, Description = "RabbitClient", OperationsPerInvoke = 10)]
    public async Task MessageRabbitWaitForReply()
    {
        _rabbitMessageReceivedSource = new TaskCompletionSource<bool>(); // Reset the source
        _sendTestingHelper.SendMessage(_message);

        // Wait for the reply
        await _rabbitMessageReceivedSource.Task
            .ConfigureAwait(false);
    }

    [Benchmark(Description = "MassTransit", OperationsPerInvoke = 10)]
    public async Task PublishMessage()
    {
        await _massTransitHelper.PublishMessage(new TestRequest(_messageAsString));
        await _massTransitHelper.Consumer.WaitForMessage()
            .ConfigureAwait(false);
    }

    private void OnConsumerOnReceived(object? model, BasicDeliverEventArgs ea) =>
        _rabbitMessageReceivedSource?.SetResult(true);
}